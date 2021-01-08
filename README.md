# TheOracle

A bot written with [Discord.Net](https://discord.foxbot.me/stable/) for [Ironsworn](https://www.ironswornrpg.com/) and its derived games, with a focus on play-by-post games.

## Joining the bot to your discord server
To add TheOracle to a discord server click [this link](https://discordapp.com/oauth2/authorize?&client_id=704480988561932389&scope=bot&permissions=523328) and then select the server you wish to add the bot to.

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
  3. :hash: | Increases the progress track by a single full box (four ticks).
  4. :game_die: | Rolls the action and challenge die for the progress tracker.
* *Sample usage:* `!ProgressTracker Epic Find the source of evil in the forest`

### `!PlayerCard`
Builds an interactive post for keeping track of player resources
* *Command Aliases:* `!StatsCard`, `!CharacterSheet`, `!CharSheet`, `!Character`, `!Player`
* *Parameters:* Character Name
* *Reactions:*
  1. :arrow_up_small: | Increases the active stat by one.
  2. :arrow_down_small: | Decreases the active stat by one.
  3. :heart: | Sets health as your active stat.
  4. :school_satchel: | Sets supply as your active stat.
  5. :sparkles: | Sets spirit as your active stat.
  6. :airplane: | Sets momentum as your active stat.
  7. :fire: | Burns/resets your momentum to 2.
* *Sample usage:* `!PlayerCard Sneaky Archer`
#### PlayerCard Sub Commands
  1. `!SetDebilities` - "Use an inline reply to set the number of debilities to a character card. The number of debilities are usually between 0 and 2.

### `!Asset`
Creates an interactive post for asset tracking and reference.
* *Parameters:* First asset text (optional), second asset text (optional)
* *Reactions:*
  1. :one: | Mark the first asset ability.
  2. :two: | Mark the second asset ability.
  3. :three: | Mark the third asset ability.
  4. :arrow_up_small: | Increases the asset's track (Health, Integrity, Essence, etc) by one, up to its maximum.
  5. :arrow_down_small: | Decreases the asset's track by one, down to its minimum.
  7. :heavy_plus_sign: | Increase the asset's tally by one (e.g. Snub Fighter's victory marks).
  6. :heavy_minus_sign: | Decrease the asset's tally by one.
  7. :arrow_left: | Set asset modal ability to first ability (e.g. Ironclad).
  7. :arrow_right: | Set asset modal ability to second ability.
* *Sample usage:* `!asset Devotant Wodin, Wits`

### `!AssetList`
Posts a list of assets. On its own, it shows all assets for the current default game; the list may be narrowed by category or asset name.

* *Parameters:* Game name (optional), asset category (optional), search string (optional)
* *Sample usage:* `!assetlist companion`

### `!InitiativeTracker`
Builds an interactive post to keep track of which players have Initiative (Ironsworn) or are In Control (Starforged) in combat.
* *Command Aliases:* `!Initiative`, `!IniTracker`
* *Parameters:* Description (optional)
* *Reactions:*
  1. :arrow_left: | Assigns/moves you to the `Initiative`/`In Control` track.
  2. :arrow_right: | Assigns/moves you to the `No Initiative`/`In a Bad Spot` track.
* *Sample usage:* `!InitiativeTracker`

### `!CreateNPC`
Creates a NPC with a name (given or random), a goal, a description, and a role/job.
* *Command Aliases:* `!NewNPC`, `!NPC`
* *Parameters:* NPC Name (optional)
* *Sample usage:* `!CreateNPC Tom Bombadil`

### `!DelveSite`
Facilitates creating a delve site.
* *Command Aliases:* `!Delve`
* *Parameters:* None (The bot will ask for each option it needs, just reply in the chat)
* *Reactions:*
  1. :arrow_left: | Decreases the progress track by the difficulty amount.
  2. :arrow_right: | Increases the progress track by the difficulty amount.
  3. :hash: | Increases the progress track by a single full box (four ticks).
  4. :game_die: | Rolls the action and challenge die for the Locate your Objective move.
  5. :four_leaf_clover: | Rolls a Feature for the delve site.
  6. :warning: | Rolls the Reveal a Danger table for the delve site.
* *Sample usage:* `!DelveSite`

### `!SceneChallenge`
Creates a challenge scene tracking post.
* *Command Aliases:* `!Scene`, `!Challenge`, `!SceneTracker`, `!ChallengeTracker`, `!ChallengeScene`
* *Parameters:* Description, Difficulty (optional)
* *Reactions:*
  1. :arrow_left: | Decreases the progress track by the difficulty amount.
  2. :arrow_right: | Increases the progress track by the difficulty amount.
  3. :hash: | Increases the progress track by a single full box (four ticks).
  4. :green_square: | Decreases the challenge track by one. This is mostly needed to fix any accidental clicks.
  5. :negative_squared_cross_mark: | Increases the challenge track by one.
  6. :game_die: | Resolves the scene challenge by rolling against its progress track
* *Sample usage:* `!SceneChallenge Dangerous Catch up to the iron robber's wagon`

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
  2. ☎️ | Adds/reveals initial contact
  3. 🔥 | Adds/reveals settlement trouble
* *Sample usage:* `!Settlement Outlands Deep Space Nine`

### `!GenerateStarship`
Creates a starforged starship, *Note: this command is still a work in progress*
* *Command Aliases:* `!Starship`, `!Spaceship`, `!Ship`
* *Parameters:* SpaceRegion (optional), Name (optional [Adds a name from a small random list until official starship names are added])
* *Reactions:*
  1. :exclamation: | Adds/reveals the starship's mission
* *Sample usage:* `!Starship Terminus Serenity`

### `!CreateNPC Starforged`
Similar to the Ironsworn command, but with some additional emoji reactions available to add results from Starforged's Character Oracles.
* *Reactions:*
  1. ️🔍 | Adds/reveals a Character Aspect
  2. 👋 | Adds/reveals the NPC's Disposition
  3. 👀 | Adds an additional First Look result to the NPC
  4. ❗️ | Adds/reveals the NPC's Goal
  5. 🎭 | Adds/reveals the NPC's Role

### `!generateCreature`
Creates a Starforged creature with Scale, Basic Form, First Look, and Encountered Behaviour. *Note: this command is still a work in progress*
* *Command Aliases:* `!creature`
* *Parameters:* Environment (optional)
* *Reactions:*
  1. 🦋 | Adds/reveals a Creature Aspect
* *Sample usage:* `!creature void`

## Advanced Commands/Features

### `!SetRerollDuplicates`
Sets the bot's re-rolling behavior for multiple oracles and game element posts. It is enabled by default.
* *Command Aliases:* `!RerollDuplicates`
* *Parameters:* True/False

### Roll multiple oracles with one command
To roll multiple oracle tables at once use the following command: `!Table [Action/Theme]`. You can even do things like `!Table [Action/Action/Action/Action]`
