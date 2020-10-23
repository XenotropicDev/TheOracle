# TheOracle

A bot written with [Discord.Net](https://discord.foxbot.me/stable/) for [Ironsworn](https://www.ironswornrpg.com/) and its derived games, with a focus on play-by-post games.

## Features
#### Oracle Tables
TheOracle includes oracles tables for Ironsworn, and can easily be extended to include additional tables. It even supports rolling multiple tables at once, and oracles that roll other oracle tables.
#### Rules Reference
You don't have to lookup rules in the quick reference PDF any more, just ask the bot and it will post the rules text for you.
#### Progress Tracker
TheOracle can track the progress for just about anything you want. Track short term game goals like journeys, or long term goals like epic character vows.
#### Game Element Generators
Quickly and easily add things like settlements, and NPCs to your game. TheOracle uses interactive posts to help you create a rich world quickly.
#### Initiative Tracker
Easily keep track of who has initiative, and who is in trouble.
#### Localization Support (Full support scheduled for v1.0.0)
TheOracle is written in a way that makes it easier to localize into your native language. Just fork the project, edit the resource files, and add a pull request and you localization will be added.

## Commands
Most commands and oracle tables names have shorter hand versions of them, but command clarity is one of the goals of this bot so overly cryptic and short commands are avoided.
#### Help
Shows all the commands. Use `!Help ModuleName` to get more specific details about a command.
 * Command Aliases: None
 * Sample usage: `!Help ActionCommand`
#### Action
Rolls the action die, adds a modifier (if one is provided), compares it to the challenge die, and reports the strength of your hit/miss/match. Anything after modifier is just fluff. 
 * Command Aliases: Act
 * Parameters: Modifier (optional, can be negative), Fluff (optional, useful for role playing)
 * Sample usage: `!Action +1 Swing at the darkness`
#### OracleTable
Rolls the provided oracle table. Use `!OracleTable [table1/table2]` to roll multiple tables at once. If multiple games use the same table (like action in Starforged and Ironsworn) you will have to provide the game you want to roll a table for, or use a distinct alias.
 * Command Aliases: Table, Oracle
 * Parameters: TableName (required)
 * Sample usage: `!OracleTable Pay The Price`
#### OracleList
Lists the available oracle tables, and their aliases
 * Command Aliases: List
 * Sample usage: `!OracleList`
#### QuickReference
Lists the detailed rules about how to make, resolve a move. 
 * Command Aliases: Reference, Library, Ref
 * Parameters: RuleName (optional)
 * Sample usage: `!QuickReference Swear an Iron Vow`
#### ProgressTracker
Builds an interactive post to keep track any progress trackers in game (Iron vows, Combats, Journeys). If no difficulty is provided a helper post will let you select one.
 * Command Aliases: Track, Tracker, Progress
 * Parameters: Difficulty (optional), Description (optional)
 * Reactions: 
  1. **Left Arrow** - Decreases the progress track by the difficulty amount. 
  2. **Right Arrow** - Increases the progress track by the difficulty amount.
  3. **Check Mark** - Increases the progress track by a single full box (four ticks).
  4. **Game Die** - Rolls the action and challenge die for the progress tracker.
 * Sample usage: `!ProgressTracker Epic Find the source of the evil`
#### InitiativeTracker
Builds an interactive post to keep track of players who do and don't have advantage.
 * Command Aliases: Initiative, IniTracker
 * Parameters: Description (optional)
 * Reactions: 
  1. **Left Arrow** - Assigns/moves you to the Advantage track. 
  2. **Right Arrow** - Assigns/moves you to the Disadvantage track. 
#### CreateNPC
Creates a NPC with a name (given or random), a goal, a description, and a role/job.
 * Command Aliases: NewNPC, NPC
 * Parameters: NPC Name (optional)
 * Sample usage: `!CreateNPC Tom Bombadil`
#### Bring Post To Bottom
This is not really a command but more of a feature. Simply react to any bot post with the ⏬ `arrow_double_down:` reaction to move it to the bottom of your chat. This is useful for moving things like progress trackers, NPCs, as an alternative to needing separate channels, or multiple pinned messages.

## Starforged Commands
#### GeneratePlanet
Creates a planet with a name (given or a random P-number), and the planet features that detectable from space. It also adds reaction buttons for revealing more features as you explore the planet
 * Command Aliases: Planet
 * Parameters: SpaceRegion (optional), Name (optional [generates a name like P-123456 if none is provided])
 * Reactions:
 🔍 - Closer look
 🦖 - Reveals the planet's life (if any)
 🌍 - Reveals the planet's biomes (only displayed if the planet type can have biomes)
 * Sample usage: `!Planet Expanse Hoth`
#### GenerateSettlement
Creates a Starforged settlement with a name (given, or random from the settlement name list), and the features in the settlement oracles.
 * Command Aliases: Settlement
 * Parameters: SpaceRegion (optional), Name (optional)
 * Reactions:
 :tools: - Adds/reveals a settlement project
 * Sample usage: `!Settlement Outlands Deep Space Nine`
#### GenerateStarship
Creates a starforged starship, *Note: this command is still a work in progress*
 * Command Aliases: Starship, Spaceship, Ship
 * Parameters: SpaceRegion (optional), Name (optional [Adds a name from a small random list until official starship names are added])
 * Reactions:
 :exclamation: - Adds/reveals the starship's mission
 * Sample usage: `!Starship Terminus Serenity`
#### CreateNPC Starforged
Same as the Ironsworn command, but with the character traits in the starforged oracles