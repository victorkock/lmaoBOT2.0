using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;    
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using DSharpPlus.VoiceNext;

namespace lmaoBOT
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }

        public CommandsNextExtension Commands { get; private set;}
        
        public InteractivityExtension InteractivityExtension { get; private set; }
        
        public VoiceNextExtension Voice { get; set; }
        
        public async Task RunAsync()
        {
            var json = string.Empty;
            
            using(var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);


            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
            };
            
            Client = new DiscordClient(config);
            
            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(10)
            });


            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false,
            };
            
            var voiceConfig = new VoiceNextConfiguration();

            Voice = Client.UseVoiceNext(voiceConfig);
            
            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Commands.FunCommands>();
            Commands.RegisterCommands<Commands.StatCommands>();
            Commands.RegisterCommands<Commands.VoiceCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);

        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}