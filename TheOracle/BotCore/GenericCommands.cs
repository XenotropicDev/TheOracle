using Discord;
using Fergun.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.GameCore.Oracle;

namespace TheOracle.BotCore
{
    public class GenericCommands : ModuleBase
    {
        public GenericCommands(IServiceProvider services)
        {
            Services = services;
            Interactive = services.GetRequiredService<InteractiveService>();

        }

        public InteractiveService Interactive { get; set; }
        public IServiceProvider Services { get; }

        [Command("ReplaceField", RunMode = RunMode.Async)]
        [Alias("Field", "SetField", "EditField")]
        [Summary("Uses inline replies to replace the content of a field on an embed in the message replied to.")]
        [Remarks("**This command comes with no warranty, and can break some bot features.**" +
            "\nThe FieldName parameter just needs to match up to the first space. Use a FieldName of 'Description' or 'Title' to modify those fields.")]
        public async Task EditField(string FieldName, [Remainder] string FieldValue)
        {
            var message = Context.Message.ReferencedMessage;
            if (message == null || message.Author.Id != Context.Client.CurrentUser.Id || message.Embeds.Count() != 1)
            {
                await ReplyAsync(GenericCommandResources.CannotEditFields).ConfigureAwait(false);
                return;
            }
            var builder = message.Embeds.First().ToEmbedBuilder();

            var descNames = GenericCommandResources.DescriptionNames.Split(',');
            var titleNames = GenericCommandResources.TitleNames.Split(',');

            var match = Regex.Match(FieldValue, @"^\[\[([\w\s]+)\]\]$");
            if (match.Success)
            {
                var cs = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
                var oracles = Services.GetRequiredService<OracleService>();
                FieldValue = oracles.RandomRow(match.Groups[1].Value, cs.GetDefaultGame(false)).Description;
            }

            if (descNames.Any(desc => desc.Trim().Equals(FieldName, StringComparison.OrdinalIgnoreCase)))
            {
                builder.WithDescription(FieldValue);
            }
            else if (titleNames.Any(desc => desc.Trim().Equals(FieldName, StringComparison.OrdinalIgnoreCase)))
            {
                builder.WithTitle(FieldValue);
            }
            else
            {
                if (!builder.Fields.Any(fld => fld.Name.Contains(FieldName, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException($"Unknown Field '{FieldName}'");

                var matchingFields = builder.Fields.Where(fld => fld.Name.Contains(FieldName, StringComparison.OrdinalIgnoreCase));
                int fieldToEdit = 0;
                if (matchingFields.Count() > 1)
                {
                    fieldToEdit = await AskForFieldNumber(matchingFields);
                    if (fieldToEdit < 0) return;
                }
                matchingFields.ElementAt(fieldToEdit).Value = FieldValue;
            }

            await message.ModifyAsync(msg => msg.Embed = builder.Build()).ConfigureAwait(false);
        }

        [Command("RemoveField", RunMode = RunMode.Async)]
        [Alias("DeleteField")]
        [Summary("Uses inline replies to remove a field on an embed in the message replied to.")]
        [Remarks("**This command comes with no warranty, and can break some bot features.**" +
            "\nThe FieldName parameter just needs to match up to the first space. Use a FieldName of 'Description' to remove the embed's description.")]
        public async Task RemoveField(string FieldName, int FieldNumber = 0)
        {
            var message = Context.Message.ReferencedMessage;
            if (message == null || message.Author.Id != Context.Client.CurrentUser.Id || message.Embeds.Count() != 1)
            {
                await ReplyAsync(GenericCommandResources.CannotEditFields).ConfigureAwait(false);
                return;
            }
            var builder = message.Embeds.First().ToEmbedBuilder();

            var descNames = GenericCommandResources.DescriptionNames.Split(',');
            if (descNames.Any(desc => desc.Trim().Equals(FieldName, StringComparison.OrdinalIgnoreCase)))
            {
                builder.Description = null;
            }
            else
            {
                if (!builder.Fields.Any(fld => fld.Name.Contains(FieldName, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException($"Unknown Field '{FieldName}'");

                var matchingFields = builder.Fields.Where(fld => fld.Name.Contains(FieldName, StringComparison.OrdinalIgnoreCase));
                if (matchingFields.Count() > 1 && (FieldNumber <= 0 || FieldNumber > matchingFields.Count()))
                {
                    FieldNumber = await AskForFieldNumber(matchingFields);
                    if (FieldNumber < 0) return;
                }
                else if (FieldNumber > 0) FieldNumber--;
                builder.Fields.Remove(matchingFields.ElementAt(FieldNumber));
            }

            await message.ModifyAsync(msg => msg.Embed = builder.Build()).ConfigureAwait(false);
        }

        [Command("AddField", RunMode = RunMode.Async)]
        [Summary("Uses inline replies to add a field on an embed in the message replied to.")]
        [Remarks("**This command comes with no warranty, and can break some bot features.**" +
            "Use quotes around the field name if it is more than one word. Use [[Oracle Name]] to roll an oracle for the new field")]
        public async Task AddField(string FieldName, [Remainder] string FieldValue)
        {
            var message = Context.Message.ReferencedMessage;
            if (message == null || message.Author.Id != Context.Client.CurrentUser.Id || message.Embeds.Count() != 1)
            {
                await ReplyAsync(GenericCommandResources.CannotEditFields).ConfigureAwait(false);
                return;
            }
            var builder = message.Embeds.First().ToEmbedBuilder();

            var match = Regex.Match(FieldValue, @"^\[\[([\w\s]+)\]\]$");
            if (match.Success)
            {
                var cs = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
                var oracles = Services.GetRequiredService<OracleService>();
                FieldValue = oracles.RandomRow(match.Groups[1].Value, cs.GetDefaultGame(false)).Description;
            }

            builder.AddField(FieldName, FieldValue, true);

            await message.ModifyAsync(msg => msg.Embed = builder.Build()).ConfigureAwait(false);
        }

        [Command("MovePost", RunMode = RunMode.Async)]
        [Alias("MoveMessage")]
        [Summary("Uses inline replies to move a message from one channel to another channel on the server. Make sure to Tag (#channel-name) the channel you want.")]
        [Remarks("")]
        public async Task MoveMessage(IGuildChannel mentionedChannel)
        {
            var message = Context.Message.ReferencedMessage;
            if (message == null || message.Author.Id != Context.Client.CurrentUser.Id)
            {
                await ReplyAsync($"I can only move my own messages.").ConfigureAwait(false);
                return;
            }


            if (mentionedChannel == Context.Channel) return;
            if (!(mentionedChannel is ITextChannel))
            {
                await ReplyAsync("Mentioned channel must be a text channel");
                return;
            }

            var user = await mentionedChannel.GetUserAsync(Context.Client.CurrentUser.Id);
            var permissions = user?.GetPermissions(mentionedChannel);
            if (permissions == null || !permissions.HasValue || !permissions.Value.SendMessages || !permissions.Value.AddReactions)
            {
                await ReplyAsync("I don't have permissions for that channel.");
                return;
            }

            var newMsg = await (mentionedChannel as IMessageChannel)
                .SendMessageAsync(message.Content, 
                    embed: message.Embeds.FirstOrDefault().ToEmbedBuilder().Build(), 
                    messageReference: message.Reference, 
                    components: ComponentBuilder.FromMessage(message).Build());

            await Task.Run(async () => { 
                var fetchedMessage = await Context.Channel.GetMessageAsync(message.Id); //This is to get around a bug in discord.net
                var reactionsToAdd = fetchedMessage.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
                foreach (var reaction in reactionsToAdd)
                {
                    await newMsg.AddReactionAsync(reaction);
                    await Task.Delay(300); //Manual delay to avoid the rate limiter
                }
                
                if (message.IsPinned)
                {
                    await newMsg.PinAsync();
                }
                
                await message.DeleteAsync();
            }).ConfigureAwait(false);
        }

        [Command("MakeEmbed")]
        [Alias("AddEmbed", "Note")]
        [Summary("A blank embed for use with other commands to make custom game objects, or keep notes.")]
        [Remarks("Use `!AddField` and `!EditField` commands to add items to this message")]
        public async Task BlankEmbed([Remainder] string Title)
        {
            await ReplyAsync(embed: new EmbedBuilder().WithTitle(Title).Build()).ConfigureAwait(false);
        }

        private async Task<int> AskForFieldNumber(IEnumerable<EmbedFieldBuilder> matchingFields)
        {
            int fieldToEdit;
            string fieldNumbersAndValues = string.Empty;
            for (int i = 0; i < matchingFields.Count(); i++)
            {
                fieldNumbersAndValues += $"{i + 1} - {matchingFields.ElementAt(i).Name} : {matchingFields.ElementAt(i).Value}\r\n";
            }
            var helperMessage = await ReplyAsync(string.Format(GenericCommandResources.SpecifyField, fieldNumbersAndValues)).ConfigureAwait(false);
            var userResponse = await Interactive.NextMessageAsync(x => x.Channel.Id == Context.Channel.Id, timeout: TimeSpan.FromMinutes(1));
            if (!userResponse.IsSuccess) return -1;
            if (!int.TryParse(userResponse.Value.Content, out fieldToEdit) || fieldToEdit < 1 || fieldToEdit > matchingFields.Count())
            {
                await ReplyAsync(string.Format(GenericCommandResources.UnknownFieldNumber, userResponse.Value.Content));
                return -1;
            }
            await helperMessage.DeleteAsync().ConfigureAwait(false);
            await userResponse.Value.DeleteAsync().ConfigureAwait(false);
            return fieldToEdit - 1;
        }
    }
}