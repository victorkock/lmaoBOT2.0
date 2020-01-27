using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lmaoBOT.Commands
{
    public class Commands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Pong(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("roll")]
        [Description("Returns random number between 0 and the range number")]
        public async Task Roll(CommandContext ctx, int range)
        {
            var rand = new Random().Next(0, range + 1);

            await ctx.Channel
                .SendMessageAsync(rand + "")
                .ConfigureAwait(false);
        }

        [Command("addSong")]
        [Description("Adds a song to the users personal playlist")]
        //[RequireRoles(RoleCheckMode.SpecifiedOnly, "DJ")]
        public async Task AddSong(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var request = await interactivity
                .WaitForMessageAsync(x => x.Channel == ctx.Channel)
                .ConfigureAwait(false);

            var user = request.Result.Author.Username;
            
            var json = string.Empty;
            
            using(var fs = File.OpenRead("./playlist.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            
            var playlistJson = JsonConvert.DeserializeObject<Playlist>(json);
            playlistJson.Songs.Add(request.Result.Content + "");
        }
    }
}
    

    