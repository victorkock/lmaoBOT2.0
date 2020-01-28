using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            
            var json = string.Empty;
            
            using(var fs = File.OpenRead("./playlist.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            
            var playlistJson = JsonConvert.DeserializeObject<Playlist>(json);
            var songList = playlistJson.Songs.ToList();
            songList.Add(request.Result.Content);
            var newSongs = songList.ToArray();
            
            var result = new Playlist
            {
                User = request.Result.Author.Username,
                Songs = newSongs
            };

            string jsonResult = JsonSerializer.Serialize(result);
            System.IO.File.WriteAllText(@"./playlist.json", jsonResult);
            
            await ctx.Channel
                .SendMessageAsync("Added '" + request.Result.Content + "' from the playlist")
                .ConfigureAwait(false);
        }

        [Command("removeSong")]
        [Description("Removes a song to the users personal playlist")]
        //[RequireRoles(RoleCheckMode.SpecifiedOnly, "DJ")]
        public async Task RemoveSong(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var request = await interactivity
                .WaitForMessageAsync(x => x.Channel == ctx.Channel)
                .ConfigureAwait(false);
            
            var json = string.Empty;
            
            using(var fs = File.OpenRead("./playlist.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            
            var playlistJson = JsonConvert.DeserializeObject<Playlist>(json);
            var songList = playlistJson.Songs.ToList();

            if (songList.Contains(request.Result.Content)) {
                songList.Remove(request.Result.Content);
                await ctx.Channel
                    .SendMessageAsync("Remove '" + request.Result.Content + "' to the playlist")
                    .ConfigureAwait(false);
            } else {
                await ctx.Channel
                    .SendMessageAsync("Song doesn't exist in the playlist try another")
                    .ConfigureAwait(false);
            }
            
            var newSongs = songList.ToArray();
            var result = new Playlist
            {
                User = request.Result.Author.Username,
                Songs = newSongs
            };
            
            string jsonResult = JsonSerializer.Serialize(result);
            System.IO.File.WriteAllText(@"./playlist.json", jsonResult);
        }

        [Command("show")]
        [Description("Shows the current playlist to far")]
        public async Task ShowPlaylist(CommandContext ctx)
        {
            var json = string.Empty;
            
            using(var fs = File.OpenRead("./playlist.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            
            var playlistJson = JsonConvert.DeserializeObject<Playlist>(json);
            
            await ctx.Channel
                .SendMessageAsync("Songs on playlist: ")
                .ConfigureAwait(false);

            playlistJson.Songs.ToList().ForEach(i => ctx.Channel
                .SendMessageAsync(i)
                .ConfigureAwait(false));
        }

        [Command("csgo")]
        [Description("Moves everyone in the channel to the CSGO channel")]
        public async Task CSGO(CommandContext ctx)
        {
        }
    }
}
    

    