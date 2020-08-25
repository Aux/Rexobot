using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rexobot.Gumroad;
using System;
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
        }

        public async Task LinkUserAsync(ulong userId, RexoProduct product)
        {
            var user = _discord.GetUser(userId) as SocketGuildUser;
            if (user == null)
                return;

            var guild = _discord.GetGuild(product.GuildId);
            var dm = await user.GetOrCreateDMChannelAsync();

            Email email = default;
            GumroadSale foundSale = default;

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
            
            int attempt = 0;
            while (!email.IsValid && attempt < 5)
            {
                if (attempt == 0)
                {
                    await dm.SendMessageAsync($"Hey {user.Username}! " +
                        $"It looks like you want to link your purchase of {product.Name} to get a special role in {guild.Name}. " +
                        $"If this is correct, please reply with your email so I can confirm this information on Gumroad.");
                }

                attempt++;
                var emailReplyMsg = await _responsive.WaitForMessageAsync((msg) => true, TimeSpan.FromMinutes(1));
                email = new Email(emailReplyMsg.Content);
                if (!email.IsValid)
                {
                    await dm.SendMessageAsync($"Sorry, it doesn't look like that's a valid email. Check for typos and try again.");
                    continue;
                }

                foundSale = await GetSaleAsync(dm, product, email);
                if (foundSale != null)
                    break;
                
                await dm.SendMessageAsync($"Sorry, I couldn't find any sales for {product.Name} matching that email. Check for typos and try again.");
            }

            var rexoRole = new RexoSyncedRole
            {
                OrderId = foundSale.OrderId,
                ProductId = product.Id
            };
            _db.SyncedRoles.Add(rexoRole);
            _db.SaveChanges();

            var role = guild.GetRole(product.RoleId);
            await user.AddRoleAsync(role);
            await dm.SendMessageAsync($"I have successfully gave you the {role.Name} role in {guild.Name}, have fun!");
        }

        private async Task<GumroadSale> GetSaleAsync(IDMChannel dm, RexoProduct product, Email email)
        {
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
