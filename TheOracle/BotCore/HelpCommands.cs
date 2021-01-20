using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle
{
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _map;

        public HelpModule(IServiceProvider map, CommandService commands)
        {
            _commands = commands;
            _map = map;
        }

        [Command("Help")]
        [Summary("Lists this bot's commands, or details about a command if one is specified")]
        public async Task Help(string path = "")
        {
            if (path == "")
            {
                await ReplyAsync(HelpResources.Title + "\n" + HelpResources.AdditionalInfo);

                string helperText = string.Empty;
                foreach (var mod in _commands.Modules.Where(m => m.Parent == null))
                {
                    helperText += AddHelp(mod);
                }

                while (true)
                {
                    if (helperText.Length > 2000)
                    {
                        var reply = helperText.Substring(0, helperText.Substring(0, 2000).LastIndexOf('\n'));
                        helperText = helperText.Substring(reply.Length);
                        await ReplyAsync(reply);
                    }
                    else
                    {
                        await ReplyAsync(helperText);
                        break;
                    }
                }
            }
            else
            {
                var commands = _commands.Modules.SelectMany(m => m.Commands).Where(c => c.Aliases.Any(a => a.Contains(path, StringComparison.OrdinalIgnoreCase)));
                if (commands.Count() == 0) { await ReplyAsync(HelpResources.NoCommandError); return; }
                if (commands.Count() > 3) { await ReplyAsync(HelpResources.TooManyMatches); return; }

                foreach (var command in commands)
                {
                    command.CheckPreconditionsAsync(Context, _map).GetAwaiter().GetResult();
                    
                    EmbedBuilder output = new EmbedBuilder()
                        .WithTitle(String.Format(HelpResources.CommandTitle, command.Name))
                        .WithDescription($"{command.Summary}\n" +
                            (command.Aliases.Any() ? String.Format(HelpResources.AliasList, string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, command.Aliases.Select(x => $"`{x}`"))) : string.Empty) + "\n" +
                            String.Format(HelpResources.Usage, $"`!{GetPrefix(command)} {GetAliases(command)}`") + "\n\n" +
                            (!string.IsNullOrEmpty(command.Remarks) ? $"{command.Remarks}\n" : string.Empty));
                    
                    await ReplyAsync("", embed: output.Build()).ConfigureAwait(false);
                }

            }
        }

        public string AddHelp(ModuleInfo module)
        {
            string helpText = string.Empty;
            foreach (var sub in module.Submodules) helpText += AddHelp(sub);

            foreach (var command in module.Commands)
            {
                helpText += $"__{command.Name}__";
                helpText += !string.IsNullOrEmpty(command.Summary) ? $" - {command.Summary}" : string.Empty;
                helpText += "\n";  //command.Summary.Contains("\n") ? "\n\n" : "\n";
            }

            return helpText;
        }



        public string GetAliases(CommandInfo command)
        {
            StringBuilder output = new StringBuilder();
            if (!command.Parameters.Any()) return output.ToString();
            foreach (var param in command.Parameters)
            {
                if (param.IsOptional) output.Append($"[{param.Name} : Default = {param.DefaultValue}] ");
                else if (param.IsMultiple) output.Append($"|{param.Name}| ");
                //else if (param.IsRemainder) output.Append($"...{param.Name} ");
                else output.Append($"<{param.Name}> ");
            }
            return output.ToString();
        }

        public string GetPrefix(CommandInfo command)
        {
            var output = GetPrefix(command.Module);
            output += $"{command.Aliases.FirstOrDefault()}";
            return output;
        }

        public string GetPrefix(ModuleInfo module)
        {
            string output = "";
            if (module.Parent != null) output = $"{GetPrefix(module.Parent)}{output}";
            if (module.Aliases.Any(a => a.Length > 0))
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            return output;
        }
    }
}