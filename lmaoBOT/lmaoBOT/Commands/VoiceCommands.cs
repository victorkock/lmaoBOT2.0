using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace lmaoBOT.Commands
{
    public class VoiceCommands : BaseCommandModule
    {
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
                .SendMessageAsync("!play "+ i)
                .ConfigureAwait(false));
        }
        
        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");

            await ctx.RespondAsync($"Connected to `{chn.Name}`").ConfigureAwait(false);
            await vnext.ConnectAsync(chn);
        }


        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");
            
            await ctx.RespondAsync($"Disconnected from `{chn.Name}`").ConfigureAwait(false);
            vnext.GetConnection(chn.Guild).Disconnect();
        }
    }
}