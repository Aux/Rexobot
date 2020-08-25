using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Rexobot.Services
{
    public class ReactionWatcherService
    {
        private readonly ILogger<ReactionWatcherService> _logger;
        private readonly DiscordSocketClient _discord;
        private readonly LinkingService _linking;
        private readonly RootDatabase _db;

        private ConcurrentDictionary<ulong, RexoProduct> _syncedProducts;

        public ReactionWatcherService(
            ILogger<ReactionWatcherService> logger,
            DiscordSocketClient discord, 
            LinkingService linking,
            RootDatabase db)
        {
            _logger = logger;
            _discord = discord;
            _linking = linking;
            _db = db;
            _syncedProducts = new ConcurrentDictionary<ulong, RexoProduct>();

            _discord.ReactionAdded += OnReactionAddedAsync;
        }

        public void AddProduct(RexoProduct product)
        {
            if (product.WatchMessageId == null) return;
            _syncedProducts.TryAdd(product.WatchMessageId.Value, product);
        }

        public RexoProduct RemoveProduct(RexoProduct product, ulong msgId)
        {
            if (_syncedProducts.TryRemove(msgId, out RexoProduct result))
                return result;
            return null;
        }

        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!_syncedProducts.TryGetValue(reaction.MessageId, out RexoProduct product))
                return;

            var syncedRole = _db.SyncedRoles.SingleOrDefault(x => x.UserId == reaction.UserId);
            if (syncedRole == null)
                return;

            await _linking.LinkUserAsync(reaction.UserId, product);
        }
    }
}
