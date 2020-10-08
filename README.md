# TheOracle

A bot written with [Discord.Net](https://discord.foxbot.me/stable/) for [Ironsworn](https://www.ironswornrpg.com/) and its derived games, with a focus on play-by-post games.

## Features
### Oracle Tables
TheOracle includes oracles tables for Ironsworn, and can easily be extended to include additional tables. It even supports rolling multiple tables at once, and oracles that roll other oracles.
### Rules Reference
You don't have to lookup rules in the quick reference PDF any more, just ask the bot and it will post the rules text for you.
### Game Element Generators (Scheduled for Alpha 0.4.0)
Quickly and easily add things like settlements, and NPCs to your game. TheOracle uses interactive posts to help you create 
### Progress Tracker (Scheduled for Alpha 0.3.0)
TheOracle can track the progress for just about anything you want.
### Initiative Tracker (Scheduled for Alpha 0.3.0)
Easily keep track of who has initiative, and who is in trouble.

## Commands
Most commands and oracle tables names have shorter hand versions of them, but command clarity is one of the goals of this bot so overly cryptic and short commands are avoided.
#### Help
Shows all the commands. Use `!Help ModuleName` to get more specific details about a command.
 * Command Aliases: None
 * Sample usage: `!Help ActionCommand`
#### Action
Rolls the action die, adds a modifier (if one is provided), compares it to the challenge die, and reports the strength of your hit/miss/match. Anything after modifier is just fluff. 
 * Command Aliases: Act
 * Sample usage: `!Action +1 Swing at the darkness`
#### OracleTable
Rolls the provided oracle table. Use `!OracleTable [table1/table2]` to roll multiple tables at once. If multiple games use the same table (like action in Starforged and Ironsworn) you will have to provide the game you want to roll a table for.
 * Command Aliases: Table, Oracle
 * Sample usage: `!OracleTable Pay The Price`
#### OracleList
Lists the available oracle tables, and their aliases
 * Command Aliases: List
 * Sample usage: `!OracleList`
#### QuickReference
Lists the detailed rules about how to make, resolve a move. 
 * Command Aliases: Reference, Library, Ref, QR
 * Sample usage: `!QuickReference Swear an Iron Vow`
