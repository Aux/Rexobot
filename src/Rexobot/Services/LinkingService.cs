using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rexobot.Gumroad;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rexobot
{
    public class LinkingService
    {
        private readonly ILogger<LinkingService> _logger;
        private readonly ResponsiveService _responsive;
        private readonly DiscordSocketClient _discord;
        private readonly IConfiguration _config;
        private readonly IGumroadApi _gumroad;
        private readonly RootDatabase _db;

        private IReadOnlyList<GumroadSale> _cachedSales;
        private ConcurrentDictionary<ulong, DateTime> _ratelimitedUsers;
        private readonly TimeSpan _ratelimitLength = TimeSpan.FromMinutes(30);

        public LinkingService(
            ILogger<LinkingService> logger,
            ResponsiveService responsive,
            DiscordSocketClient discord,
            IConfiguration config,
            IGumroadApi gumroad,
            RootDatabase db)
        {
            _logger = logger;
            _responsive = responsive;
            _discord = discord;
            _config = config;
            _gumroad = gumroad;
            _db = db;

            _cachedSales = new List<GumroadSale>();
            _ratelimitedUsers = new ConcurrentDictionary<ulong, DateTime>();
        }

        public async Task LinkUserAsync(ulong userId, RexoProduct product)
        {
            // Check if user is in a shared guild
            var user = _discord.GetUser(userId) as SocketGuildUser;
            if (user == null)
                return;

            // Check if user is being ratelimited
            if (_ratelimitedUsers.TryGetValue(userId, out DateTime ratelimitStart))
            {
                var ratelimitRemaining = DateTime.UtcNow - ratelimitStart;

                // If yes and time is remaining, silently drop the request
                if (ratelimitRemaining < _ratelimitLength)
                {
                    _logger.LogInformation($"User `{userId}` was stopped by the ratelimiter: {Math.Round(ratelimitRemaining.TotalMinutes, 2)} minutes remaining");
                    return;
                } else
                {
                    // Otherwise, remove them from ratelimiter and continue
                    _ratelimitedUsers.Remove(userId, out DateTime _);
                    _logger.LogInformation($"User `{userId}` removed from ratelimiter");
                }
            }

            // Get guild and dm objects
            var guild = _discord.GetGuild(product.GuildId);
            var dm = await user.GetOrCreateDMChannelAsync();

            Email email = default;
            GumroadSale foundSale = default;

            // Check if an email is already stored for the user
            var rexoUser = _db.Users.SingleOrDefault(x => x.Id == userId);
            if (rexoUser != null)
            {
                email = new Email(rexoUser.Email);
                foundSale = await GetSaleAsync(dm, product, email);
                if (foundSale == null)
                {
                    await dm.SendMessageAsync($"Sorry, I couldn't find any sales for this product matching your linked email.");
                    return;
                }
            }
            
            // Loop over several attempts allowing some space for user error
            int attempt = 0;
            while (!email.IsValid && attempt < 5)
            {
                // First attempt instruction message
                if (attempt == 0)
                {
                    await dm.SendMessageAsync($"Hey {user.Username}! " +
                        $"It looks like you want to link your purchase of {product.Name} to get a special role in {guild.Name}. " +
                        $"If this is correct, please reply with your email so I can confirm this information on Gumroad.");
                }

                // Wait for user response
                attempt++;
                var emailReplyMsg = await _responsive.WaitForMessageAsync((msg) => true, TimeSpan.FromMinutes(1));
                email = new Email(emailReplyMsg.Content);
                if (!email.IsValid)
                {
                    await dm.SendMessageAsync($"Sorry, it doesn't look like that's a valid email. Check for typos and try again.");
                    continue;
                }

                // Try to pull a sale matching this email from gumroad
                foundSale = await GetSaleAsync(dm, product, email);
                if (foundSale != null)
                    break;
                
                await dm.SendMessageAsync($"Sorry, I couldn't find any sales for {product.Name} matching that email. Check for typos and try again.");
            }
            // If the loop breaks due to attempts, add the user to the ratelimiter and break.
            if (attempt >= 5)
            {
                _logger.LogInformation($"User `{userId}` is now being ratelimited");
                _ratelimitedUsers.TryAdd(userId, DateTime.UtcNow);
                await dm.SendMessageAsync($"You have submitted 5 invalid emails and are now being ratelimited. Please try again in 30 minutes.");
                return;
            }

            // Save order information to the database
            var rexoRole = new RexoSyncedRole
            {
                OrderId = foundSale.OrderId,
                ProductId = product.Id
            };
            _db.SyncedRoles.Add(rexoRole);
            _db.SaveChanges();

            // Give the user their special role
            var role = guild.GetRole(product.RoleId);
            await user.AddRoleAsync(role);
            await dm.SendMessageAsync($"I have successfully gave you the {role.Name} role in {guild.Name}, have fun!");
        }

        private async Task<GumroadSale> GetSaleAsync(IDMChannel dm, RexoProduct product, Email email)
        {
            // Check if the user's data was provided in a previous gumroad request
            // Do I even use this?
            var foundSale = _cachedSales.FirstOrDefault(x => x.PurchaseEmail.ToLower() == email.ToString().ToLower());
            if (foundSale == null)
            {
                var allSales = await _gumroad.GetSalesAsync(_config["gumroad:token"], new GetSalesParams
                {
                    Email = email.ToString()
                });

                if (allSales.IsSuccess)
                    return allSales.Sales.FirstOrDefault();
                else
                    return null;
            }
            return foundSale;
        }
    }
}
