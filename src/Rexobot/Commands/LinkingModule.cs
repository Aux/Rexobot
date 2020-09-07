using Discord.Commands;
using System.Threading.Tasks;

namespace Rexobot.Commands
{
    public class LinkingModule : BotModuleBase
    {
        private readonly LinkingService _linking;

        public LinkingModule(LinkingService linking)
        {
            _linking = linking;
        }

        [Command("link")]
        [Summary("Start the linking process with a command instead of a reaction")]
        [Remarks("1st Parameter: The name or id of a product")]
        public async Task LinkAsync([Remainder]RexoProduct product)
        {
            await _linking.LinkUserAsync(Context.User.Id, product);
        }
    }
}
