using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rexobot.Commands
{
    public class RexoProductTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var db = (RootDatabase)services.GetService(typeof(RootDatabase));

            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == input || x.Name.ToLower() == input.ToLower());
            if (product == null)
                return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Sorry, I couldn't find any products matching that ID.");
            else
                return TypeReaderResult.FromSuccess(product);
        }
    }
}
