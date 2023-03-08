## About
TheOracle2 is a complete rework of TheOracle using new discord bot features, and more integrated data features

1. This is very much a beta product. Expect things to not work.
2. If you run into any bugs, have an idea, or want to contribute please make an issue here, or post in the Ironsworn #bot-discussion channel.

## Joining the bot to your discord server
To add TheOracle to a discord server click [this link](https://discord.com/api/oauth2/authorize?client_id=704480988561932389&permissions=431644532800&scope=bot%20applications.commands) and then select the server you wish to add the bot to.

If you need a discord server for your game you can use [this link](https://discord.new/hevebmEhcjCa) to get started with a preexisting discord server template.

## Getting started
1. If you want to get content for a game other than Ironsworn (Starforged or Sundering Isles) set your game with `/set-game`
2. Create a character with `/player-character create`
3. Add some assets with `/asset`
4. Roll some actions with `/roll pc-action`
5. Get the results from an oracle with `/oracle`
6. View all the commands available in the bot by typing `/` and scrolling though the options

## Other features
#### Recreate message:
You can recreate a message, similar to the old ⏬ reaction method by right clicking a bot message and selecting recreate message from the apps menu.

![image](https://user-images.githubusercontent.com/6792312/147948167-a1b67087-5064-40e4-b4e5-9f3738ade82a.png)

## Running the bot yourself
note: This is for people that want to change the source code and run their own instance of the bot. It's not something most users will want/need to do.
* Install PostgreSQL Server
* Create a new database and db user for the bot to use
* Create a database settings file named `dbSettings.json` so the bot knows how to connect. It should have a structure similar to this:
```
{
    "dbConnectionString":"Host=localhost;Port=5432;Database=NameOfDbYouCreated;Username=BotDbUser",
    "dbConnectionStringWithPort":"host=127.0.0.1 port=5432 dbname=NameOfDbYouCreated connect_timeout=10 user=BotDbUser",
    "dbConnectionStringOff":"postgresql://BotDbUser@localhost:5432/NameOfDbYouCreated",
    "dbPassword":"YourDbUserPassword"
}
```
* Get a discord bot token from the discord developer portal
* Start the bot server, paste your token when prompted. (If you need to change the token it's stored in the token.json file in your server's folder)

## Privacy
TheOracle bot doesn't store any user data of any kind, except for commands that are explicitly handled by the bot. Any data collected will not be sold or used for anything other than further developing and improvement of the bot.
