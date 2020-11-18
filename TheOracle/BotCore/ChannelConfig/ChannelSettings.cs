using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TheOracle.GameCore;

namespace TheOracle.BotCore
{
    public class ChannelSettings
    {
        [Key]
        public ulong ChannelID { get; set; }
        public GameName DefaultGame { get; set; }
        public String Culture { get; set; }
        public bool RerollDuplicates { get; set; }

        public static async Task<ChannelSettings> GetChannelSettingsAsync(ulong channelID)
        {
            using var context = new DiscordChannelContext();
            return await context.ChannelSettings.FirstOrDefaultAsync(item => item.ChannelID == channelID);
        }
    }
}