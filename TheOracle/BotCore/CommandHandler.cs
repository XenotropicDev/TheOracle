using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TheOracle
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private IServiceProvider _service = null;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync(IServiceProvider service = null)
        {
            _service = service;
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix("! ", ref argPos) ||
                    message.HasCharPrefix('!', ref argPos) ||                    
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)
                ) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Commands    {message.Author} entered the command: {message.Content}");

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _service);

            // Inform the user if the command fails to be executed;
            // however, this may not always be desired, as it may clog up the request queue should a user spam a command.
            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand) return;

                var commandSearch = _commands.Search(context, argPos);
                var triggeredCommand = commandSearch.Commands.FirstOrDefault();
                if (commandSearch.Commands.Count > 0 && triggeredCommand.Command.Name == "Roll" && result.Error == CommandError.BadArgCount) return;

                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}