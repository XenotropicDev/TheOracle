using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TheOracle
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

		private DiscordSocketClient _client;

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();

			_client.Log += Log;

			await _client.LoginAsync(TokenType.Bot,
				Environment.GetEnvironmentVariable("DiscordToken"));
			await _client.StartAsync();

			_client.MessageReceived += MessageReceived;

			CommandService cs = new CommandService();
			CommandHandler ch = new CommandHandler(_client, cs);

			CommandHandlerRegister.Handler = ch;

			await ch.InstallCommandsAsync();


			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

		private async Task MessageReceived(SocketMessage message)
		{
			if (message.Content == "!ping")
			{
				await message.Channel.SendMessageAsync("Pong!");
			}
		}

    }
}
