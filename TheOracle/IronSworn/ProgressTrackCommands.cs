using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle.Core
{
    public class ProgressTrackCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Tracker")]
        [Alias("Track")]
        [Summary("Creates an objective tracking post for things like Iron Vows")]
        public async Task ProgressTracker(ChallengeRank difficulty)
        {
            ReplyAsync($"Starting a {difficulty} track");
        }
    }

    public enum ChallengeRank 
    { 
        Troublesome,
        Dangerous, 
        Formidable, 
        Extreme, 
        Epic
    }
}
