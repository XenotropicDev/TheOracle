using Discord;
using Discord.Commands;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class GenericCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ReplaceField")]
        [Alias("Field", "SetField", "EditField")]
        [Summary("Uses inline replies to replace the content of a field on an embed in the message replied to.")]
        [Remarks("**This command comes with no warranty, and can break some bot features.**" +
            "\nThe FieldName parameter just needs to match up to the first space. Use a FieldName of 'Description' to modify the embed's description.")]
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
            if (descNames.Any(desc => desc.Trim().Equals(FieldName, StringComparison.OrdinalIgnoreCase)))
            {
                builder.WithDescription(FieldValue);
            }
            else
            {
                if (!builder.Fields.Any(fld => fld.Name.Contains(FieldName))) throw new ArgumentException($"Unknown Field '{FieldName}'");
                if (builder.Fields.Count(fld => fld.Name.Contains(FieldName)) > 1) throw new ArgumentException($"Too many fields with name '{FieldName}'");

                builder.Fields.FirstOrDefault(fld => fld.Name.Contains(FieldName)).Value = FieldValue;
            }

            await message.ModifyAsync(msg => msg.Embed = builder.Build()).ConfigureAwait(false);
        }
    }
}