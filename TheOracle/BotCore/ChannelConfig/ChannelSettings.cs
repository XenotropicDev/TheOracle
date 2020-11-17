using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
            var settings = await context.ChannelSettings.FirstOrDefaultAsync(item => item.ChannelID == channelID);
            if (settings == null)
            {
                settings = new ChannelSettings
                {
                    ChannelID = channelID,
                    RerollDuplicates = true,
                    DefaultGame = GameName.Ironsworn
                };
                context.ChannelSettings.Add(settings);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            return settings;
        }
    }
}