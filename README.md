# TheOracle

A bot written with [Discord.Net](https://discord.foxbot.me/stable/) for [Ironsworn](https://www.ironswornrpg.com/) and its derived games, with a focus on play-by-post games.

## Features

### Oracle Tables
TheOracle includes oracles tables for Ironsworn, and can easily be extended to include additional tables. It even supports rolling multiple tables at once, and oracles that roll other oracle tables.

### Rules Reference
You don't have to lookup rules in the quick reference PDF any more, just ask the bot and it will post the rules text for you.

### Progress Tracker
TheOracle can track the progress for just about anything you want. Track short term game goals like journeys, or long term goals like epic character vows.

### Game Element Generators
Quickly and easily add things like settlements, and NPCs to your game. TheOracle uses interactive posts to help you create a rich world quickly.

### Initiative Tracker
Easily keep track of who has initiative, and who is in trouble.

### Localization Support (Full support scheduled for v1.0.0)
TheOracle is written in a way that makes it easier to localize into your native language. Just fork the project, edit the resource files, and add a pull request and you localization will be added.

## Joining the bot to your discord server
To add TheOracle to a discord server click [this link](https://discordapp.com/oauth2/authorize?&client_id=704480988561932389&scope=bot&permissions=523328) and then select the server you wish to add the bot to.

## Commands
Most commands and oracle tables names have shorter hand versions of them, but command clarity is one of the goals of this bot so overly cryptic and short commands are avoided. If a command needs more information such as a progress tracker difficulty a helper post will give you options to select from.

### `!Help`
Shows all the commands. Use `!Help ModuleName` to get more specific details about a command.
* *Command Aliases:* None
* *Sample usage:* `!Help ActionCommand`

### `!SetDefaultGame`
Allows players to set the default game for a discord channel. This command only needs to be run once per channel, or if you ever want to change the default game.
* *Command Aliases:* None
* *Sample usage:* `!SetDefaultGame Ironsworn`

### `!Action`
Rolls the action die, adds any modifier(s), compares it to the challenge die, and reports the strength of your hit/miss/match. Anything other than numbers in the command is ignored by the command as fluff.
* *Command Aliases:* `!Act`
* *Parameters:* Modifier (optional, can be negative), Fluff (optional, useful for role playing)
* *Sample usage:* `!Action +1 Swing at the darkness`

### `!OracleTable`
Rolls the provided oracle table. Use `!OracleTable [table1/table2]` to roll multiple tables at once. If multiple games use the same table (like action in Starforged and Ironsworn) you will have to provide the game you want to roll a table for, or use a distinct alias.
* *Command Aliases:* `!Table`, `!Oracle`
* *Parameters:* TableName (required)
* *Sample usage:* `!OracleTable Pay The Price`

### `!OracleList`
Lists the available oracle tables, and their aliases
* *Command Aliases:* `!List`
* *Sample usage:* `!OracleList`

### `!QuickReference`
Lists the detailed rules about how to make, resolve a move.
* *Command Aliases:* `!Reference`, `!Library`, `!Ref`
* *Parameters:* RuleName (optional)
* *Sample usage:* `!QuickReference Swear an Iron Vow`

### `!ProgressTracker`
Builds an interactive post to keep track any progress trackers in game (Iron vows, Combats, Journeys). If no difficulty is provided a helper post will let you select one.
* *Command Aliases:* `!Track`, `!Tracker`, `!Progress`
* *Parameters:* Difficulty (optional), Description (optional)
* *Reactions:*
 1. :arrow_left: | Decreases the progress track by the difficulty amount.
 2. :arrow_right: | Increases the progress track by the difficulty amount.
 3. :heavy_check_mark: | Increases the progress track by a single full box (four ticks).
 4. :game_die: | Rolls the action and challenge die for the progress tracker.
* *Sample usage:* `!ProgressTracker Epic Find the source of evil in the forest`

### `!PlayerCard`
Builds an interactive post for keeping track of player resources
* *Command Aliases:* `!StatsCard`, `!CharacterSheet`, `!CharSheet`
* *Parameters:* Character Name
* *Reactions:*
 1. :arrow_up_small: | Increases the active stat by one.
 2. :arrow_down_small: | Decreases the active stat by one.
 3. :heart: | Sets health as your active stat.
 4. :school_satchel: | Sets supply as your active stat.
 5. :sparkles: | Sets spirit as your active stat.
 6. :airplane: | Sets momentum as your active stat.
 7. :fire: | Burns/resets your momentum to 2.

### `!InitiativeTracker`
Builds an interactive post to keep track of players who do and don't have advantage.
* *Command Aliases:* `!Initiative`, `!IniTracker`
* *Parameters:* Description (optional)
* *Reactions:*
 1. :arrow_left: | Assigns/moves you to the Advantage track.
 2. :arrow_right: | Assigns/moves you to the Disadvantage track.
* *Sample usage:* `!PlayerCard Sneaky Archer`

### `!CreateNPC`
Creates a NPC with a name (given or random), a goal, a description, and a role/job.
* *Command Aliases:* `!NewNPC`, `!NPC`
* *Parameters:* NPC Name (optional)
* *Sample usage:* `!CreateNPC Tom Bombadil`

### `!Roll`
Rolls the specified dice for things like assets that let you reroll a die.
* *Command Aliases:* `!Dice`
* *Parameters:* Die notation value, Number of times to roll (optional, default is 1)
* *Sample usage:* `!Roll 1d10 2`

## Generic Message Reactions
Apply one of these reactions to any of the bot's posts to help you manage your game and keep your channel up to date and free of clutter.

#### Move Existing Post To Bottom of Chat
Simply react to any bot post with the ⏬ `:arrow_double_down:` reaction to move it to the bottom of your chat. This is useful for moving things like progress trackers and NPCs, as an alternative to needing separate channels, or multiple pinned messages.

#### Delete a Post
React with :x: `:x:` then the bot will add a :ballot_box_with_check: `:ballot_box_with_check:` reaction for you to confirm the delete. This is helpful to remove things like initiative trackers or progress trackers for little things from the chat.

#### Pin a Post
React to a bot message with :pushpin: `:pushpin:` and the bot will pin the message for you. Make sure to leave the reaction on the message until you are ready to unpin it.

## Starforged Commands

### `!GeneratePlanet`
Creates a planet with a name (given or a random P-number), and the planet features that detectable from space. It also adds reaction buttons for revealing more features as you explore the planet
* *Command Aliases:* `!Planet`
* *Parameters:* SpaceRegion (optional), Name (optional [generates a name like P-123456 if none is provided])
* *Reactions:*
 1. 🔍 | Closer look
 2. 🦖 | Reveals the planet's life (if any)
 3. 🌍 | Reveals the planet's biomes (only displayed if the planet type can have biomes)
* *Sample usage:* `!Planet Expanse Hoth`

### `!GenerateSettlement`
Creates a Starforged settlement with a name (given, or random from the settlement name list), and the features in the settlement oracles.
* *Command Aliases:* `!Settlement`
* *Parameters:* SpaceRegion (optional), Name (optional)
* *Reactions:*
 1. ️🛠️ | Adds/reveals a settlement project
* *Sample usage:* `!Settlement Outlands Deep Space Nine`

### `!GenerateStarship`
Creates a starforged starship, *Note: this command is still a work in progress*
* *Command Aliases:* `!Starship`, `!Spaceship`, `!Ship`
* *Parameters:* SpaceRegion (optional), Name (optional [Adds a name from a small random list until official starship names are added])
* *Reactions:*
 1. :exclamation: | Adds/reveals the starship's mission
* *Sample usage:* `!Starship Terminus Serenity`

### `!CreateNPC Starforged`
Same as the Ironsworn command, but with the character traits in the starforged oracles

## Advanced Commands/Features

### `!SetRerollDuplicates`
Sets the bot's re-rolling behavior for multiple oracles and game element posts.
* *Command Aliases:* `!RerollDuplicates`
* *Parameters:* True/False

### Roll multiple oracles with one command
To roll multiple oracle tables at once use the following command: `!Table [Action/Theme]`. You can even do things like `!Table [Action/Action/Action/Action]`
