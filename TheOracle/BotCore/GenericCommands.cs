using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class GenericCommands : InteractiveBase
    {
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

        private async Task<int> AskForFieldNumber(IEnumerable<EmbedFieldBuilder> matchingFields)
        {
            int fieldToEdit;
            string fieldNumbersAndValues = string.Empty;
            for (int i = 0; i < matchingFields.Count(); i++)
            {
                fieldNumbersAndValues += $"{i + 1} - {matchingFields.ElementAt(i).Name} : {matchingFields.ElementAt(i).Value}\r\n";
            }
            var helperMessage = await ReplyAsync(string.Format(GenericCommandResources.SpecifyField, fieldNumbersAndValues)).ConfigureAwait(false);
            var userResponse = await NextMessageAsync(false, timeout: TimeSpan.FromMinutes(1));
            if (userResponse == null) return -1;
            if (!int.TryParse(userResponse.Content, out fieldToEdit) || fieldToEdit < 1 || fieldToEdit > matchingFields.Count())
            {
                await ReplyAsync(string.Format(GenericCommandResources.UnknownFieldNumber, userResponse.Content));
                return -1;
            }
            await helperMessage.DeleteAsync().ConfigureAwait(false);
            await userResponse.DeleteAsync().ConfigureAwait(false);
            return fieldToEdit - 1;
        }
    }
}