## Warning
This branch is experimental, and at the time of writing this it isn't complete in anyway.

## About
TheOracle2 is a complete rework of TheOracle using new discord bot features, and more integrated data features

1. This is very much an alpha product. Expect things to not work.
2. Things within this bot are very likely to change. Don't expect any backwards compatibility as things get changed.
3. It only has Assets, Oracles, and Move references for Starforged right now.
4. It is not running on any real server hardware, so it might be slow to respond, fail, or just out right be offline for hours at a time. I plan to change this in the next week or so.
5. If you run into any bugs, have an idea, or want to contribute please make an issue here, or post in the Ironsworn #bot-discussion channel.

#### Invite link: 
https://discord.com/api/oauth2/authorize?client_id=756889936640213102&permissions=431644532800&scope=bot%20applications.commands

## Navigating slash commands
Some oracles that have subsets of oracle tables (I’m looking at you planets) use a command key of "main" for their base options.

#### Oracle categories
Unfortunately discord doesn’t show all the options in a scrollable, or well sorted manner for the oracle tables, so here’s a list of all the root options:
| Table | Table |
| --- | --- |
| Character | Miscellaneous |
| Character Creation | Move |
| Core | Planet |
| Creature | Precursor Vault |
| Derelict | Settlement |
| Faction | Space |
| Location Theme | Starship |

## Other features
#### Recreate message:
You can recreate a message, similar to the old ⏬ reaction method by right clicking a bot message and selecting recreate message from the apps menu.

![image](https://user-images.githubusercontent.com/6792312/147948167-a1b67087-5064-40e4-b4e5-9f3738ade82a.png)

## Running the bot yourself
note: This is for people that want to change the source code and run their own instance of the bot. It's not something most users will want/need to do.
* Install PostgreSQL Server
* Get a discord bot token from the discord developer portal
