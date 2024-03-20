# Modmail.NET
An Open-Source Modmail Discord Bot made with .NET 6 for self hosting

This bot can be easily setup and used on your server for managing community communication on a single way.

Bot creates tickets and users message with the bot directly instead of sending message to mods

# Warning
This project is may not be ready for production needs more features and polishing.

If you wish to use it be aware of potential errors

This bot is currently designed to be used in single server and you must host it by yourself

Reasons being, we must give user selectbox etc to select always and cache latest and keep confirming if user wants to send it to designed bot

Since creating new bot and even hosting is not big deal, i won't be adding full support for multiple servers.

Your main server id in config is taken into consideration almost always

Only support for multiple servers is you have a discord server for just managing tickets then you can add bot on multiple servers.

However user will see main discord name/icon and all logs will be sent there

# Features
- Open tickets by sending a private message to bot
- Close tickets by using command or deleting the channel
- Logging to messages/transcript to database and modmail log channel (sensitive logging)
- Admins or mod team can respond tickets by sending message to created channel
- Toggle sensitive logging (Logging messages to tickets)
- Toggle anonymous messages by mods, allows responding tickets anonymously
- Setting up teams adding roles and members to team
- Setting priority for tickets (adds emoji to tikcet channel name)
- Adding private mod notes to tickets that can be viewed later only by mods
- Pinging team roles/members on ticket open
- Caching user information that are interacting with the ticket system  
- Blacklist, blocking users from opening tickets 
- Getting feedback from user when ticket closed

# Contributing
Create a pull request by using semantic commits and proper explanation

# Installation
1. Install .NET 6
2. Download project build and publish for your desired platform
3. Visit Discord Developer Portal and create a new application
4. Setup your bot configuration
5. Run the app

# Possible Upcoming Features
- Web UI and/or API to manage tickets and see transcript information
- Encrpyt message information
- Confirmation for close and opening tickets
- Language file support
- Editing embeds and colors and maybe more (need web ui for this) 

# Commands
Parameter types with '*' are required

## Modmail Group Slash Commands 
`/modmail setup (bool sensitiveLogging, bool takeFeedbackAfterClosing, string greetingMessage, string closingMessage)`: Setup the modmail bot

`/modmail configure (bool sensitiveLogging, bool takeFeedbackAfterClosing, string greetingMessage, string closingMessage)`: Configure the modmail bot after being setup

`/modmail get-settings`: Get the modmail bot settings 


## Ticket Group Slash Commands 
`/ticket close (string reason)`: Close a ticket

`/ticket set-priority (TicketPriority* priority)`: Set the priority of a ticket

`/ticket add-note (string* note)`: Add a note to a ticket

`/ticket toggle-anonymous`: Toggle anonymous mode for a ticke

## Team Group Slash Commands 
`/team list`: List all teams

`/team create (string* teamName, TeamPermissionLevel* permissionLevel)`: Create a new team

`/team remove (string* teamName)`: Remove a team

`/team add-user (string* teamName, DiscordUser* member)`: Add a user to a team

`/team remove-user (string* teamName, DiscordUser* member)`: Remove a user from a team

`/team add-role (string* teamName, DiscordRole* role)`: Adds a role to a team

`/team remove-role (string* teamName, DiscordRole* role)`: Removes a role from a team

## Blacklist Group Slash Commands 
`/blacklist add (DiscordUser* user, bool* notifyUser, string* reason)`: Add a user to the blacklist

`/blacklist remove (DiscordUser* user)`: The user to remove from the blacklist

`/blacklist status (DiscordUser* user)`: Check if a user is blacklisted

`/blacklist view`  View all blacklisted users


# Disclaimer
You are responsible for data safety of users and messages when you are using this bot.