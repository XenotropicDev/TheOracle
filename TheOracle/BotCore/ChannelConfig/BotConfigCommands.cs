using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class BotConfigCommands : ModuleBase<SocketCommandContext>
    {
        public BotConfigCommands(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        //[Command("Language")]
        //public async Task SetChannelLocalization(string name)
        //{
        //    await ReplyAsync("Saved.");
        //}

        [Command("SetDefaultGame", ignoreExtraArgs: true)]
        [Summary("Sets the default game for a channel.")]
        public async Task SetDefaultGame(GameName game)
        {
            using var db = new DiscordChannelContext();
            var existingSettings = await db.ChannelSettings.FirstOrDefaultAsync(cs => cs.ChannelID == Context.Channel.Id);

            if (existingSettings != null)
            {
                existingSettings.DefaultGame = game;
                await db.SaveChangesAsync().ConfigureAwait(false);
                await Context.Message.AddReactionAsync(new Emoji("🆗")).ConfigureAwait(false);
                return;
            }
            else
            {
                ChannelSettings cs = new ChannelSettings
                {
                    ChannelID = Context.Channel.Id,
                    DefaultGame = game
                };
                db.ChannelSettings.Add(cs);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await Context.Message.AddReactionAsync(new Emoji("🆗")).ConfigureAwait(false);
            }
        }

        [Command("SetRerollDuplicates", ignoreExtraArgs: true)]
        [Alias("RerollDuplicates")]
        [Summary("Sets the rolling behavior for a channel.")]
        public async Task SetRerollDuplicates(bool value)
        {
            using var db = new DiscordChannelContext();
            var existingSettings = await db.ChannelSettings.FirstOrDefaultAsync(cs => cs.ChannelID == Context.Channel.Id);

            if (existingSettings != null)
            {
                existingSettings.RerollDuplicates = value;
                await db.SaveChangesAsync().ConfigureAwait(false);
                await Context.Message.AddReactionAsync(new Emoji("🆗")).ConfigureAwait(false);
                return;
            }
            else
            {
                ChannelSettings cs = new ChannelSettings
                {
                    ChannelID = Context.Channel.Id,
                    RerollDuplicates = value
                };
                db.ChannelSettings.Add(cs);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await Context.Message.AddReactionAsync(new Emoji("🆗")).ConfigureAwait(false);
            }
        }

        [Command("GetDefaultGame", ignoreExtraArgs: true)]
        [Summary("Posts the default game for the channel.")]
        public async Task GetDefaultGame()
        {
            using var db = new DiscordChannelContext();
            var existingSettings = await db.ChannelSettings.FirstOrDefaultAsync(cs => cs.ChannelID == Context.Channel.Id);

            if (existingSettings != null)
            {
                await ReplyAsync($"Your default game is: {existingSettings.DefaultGame}").ConfigureAwait(false);
            }
            else
            {
                await ReplyAsync($"Your default game not set.").ConfigureAwait(false);
            }
        }
    }
}