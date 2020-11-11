using Discord;
using Discord.Commands;
using System;
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
                string helperText = string.Empty;
                helperText += HelpResources.Title + "\n";
                helperText += HelpResources.AdditionalInfo + "\n\n";

                foreach (var mod in _commands.Modules.Where(m => m.Parent == null))
                {
                    helperText += AddHelp(mod);
                }

                await ReplyAsync(helperText);
            }
            else
            {
                EmbedBuilder output = new EmbedBuilder();
                var mods = _commands.Modules.Where(m => m.Name.Contains(path, StringComparison.OrdinalIgnoreCase) || m.Aliases.Any(alias => alias.Contains(path, StringComparison.OrdinalIgnoreCase)));
                if (mods.Count() == 0) mods = _commands.Modules.Where(m => m.Commands.Any(c => c.Aliases.Any(a => a.Contains(path, StringComparison.OrdinalIgnoreCase))));
                if (mods.Count() == 0) { await ReplyAsync(HelpResources.NoCommandError); return; }
                if (mods.Count() > 3) { await ReplyAsync(HelpResources.TooManyMatches); return; }

                foreach (var mod in mods)
                {
                    output.Title = mod.Name;
                    output.Description = $"{mod.Summary}\n" +
                    (!string.IsNullOrEmpty(mod.Remarks) ? $"{mod.Remarks}\n" : "") +
                    (mod.Aliases.Count() > 0 ? $"Prefix(es): {string.Join(",", mod.Aliases)}\n" : "") +
                    (mod.Submodules.Any() ? $"Submodules: {mod.Submodules.Select(m => m.Name)}\n" : "") + " ";
                    AddCommands(mod, ref output);
                }

                await ReplyAsync("", embed: output.Build());
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

        public void AddCommands(ModuleInfo module, ref EmbedBuilder builder)
        {
            foreach (var command in module.Commands)
            {
                command.CheckPreconditionsAsync(Context, _map).GetAwaiter().GetResult();
                AddCommand(command, ref builder);
            }
        }

        public void AddCommand(CommandInfo command, ref EmbedBuilder builder)
        {
            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n\n" +
                (!string.IsNullOrEmpty(command.Remarks) ? $"{command.Remarks}\n" : string.Empty) +
                (command.Aliases.Any() ? String.Format(HelpResources.AliasList, string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, command.Aliases.Select(x => $"`{x}`"))) : string.Empty) + "\n" +
                String.Format(HelpResources.Usage, $"`!{GetPrefix(command)} {GetAliases(command)}`");
            });
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