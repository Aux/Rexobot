using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestEase;
using Rexobot.Commands;
using Rexobot.Gumroad;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Rexobot
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(string[] args)
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, "common"))
                .AddYamlFile("config.yml")
                //.AddCommandLine(args)
                .Build();
        }

        public async Task StartAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            var discord = provider.GetRequiredService<DiscordSocketClient>();
            await discord.LoginAsync(Discord.TokenType.Bot, _config["discord:token"]);
            await discord.StartAsync();

            var commands = provider.GetRequiredService<CommandService>();
            commands.AddTypeReader<Email>(new EmailTypeReader());
            commands.AddTypeReader<RexoProduct>(new RexoProductTypeReader());
            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), provider);

            provider.GetRequiredService<ILoggerFactory>().AddProvider(new BotLoggerProvider(_config));
            provider.GetRequiredService<LoggingService>().Start();
            provider.GetRequiredService<CommandHandlingService>().Start();

            await Task.Delay(-1);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 50,
                    GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    IgnoreExtraArgs = false
                }))
                .AddSingleton(RestClient.For<IGumroadApi>(GumroadConstants.ApiUrl))
                .AddSingleton(_config)
                .AddSingleton<LoggingService>()
                .AddSingleton<LinkingService>()
                .AddSingleton<CommandHandlingService>()
                .AddTransient<ResponsiveService>()
                .AddDbContext<RootDatabase>()
                .AddLogging();
        }
    }
}
