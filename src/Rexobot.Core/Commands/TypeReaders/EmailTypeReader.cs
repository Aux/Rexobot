using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Rexobot.Commands
{
    public class EmailTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var email = new Email(input);
            if (email.IsValid)
                return Task.FromResult(TypeReaderResult.FromSuccess(email));
            else
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"`{input}` is not a valid email address"));
        }
    }
}
