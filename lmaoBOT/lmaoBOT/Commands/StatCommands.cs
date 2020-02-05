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
    public class StatCommands : BaseCommandModule
    {
        [Command("csgo")]
        [Description("Moves everyone in the channel to the CSGO channel")]
        public async Task CsgoStats(CommandContext ctx, string user)
        {
            
        }
    }
}