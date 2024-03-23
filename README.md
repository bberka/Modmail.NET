# Modmail.NET
An Open-Source Modmail Discord Bot made with .NET 6 for self hosting

This bot can be easily setup and used on your server for managing community communication on a single way.

Bot creates tickets and users message with the bot directly instead of sending message to mods

# Contributing
Create a pull request by using semantic commits and proper explanation

# Warning
This project is may not be ready for production needs more polishing.

If you wish to use it be aware of potential errors

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
- Ticket type system, users can select ticket types to tell mods what the ticket is about

# Multiple Servers Usage
Before trying to use the bot on multiple servers you must now about the limitations;

1. This bot can only be setup once and only for main server
2. Ticket management/commands only allowed on the main server, which id provided in configuration
3. Team management/commands is locked to main server only, meaning teams and members can only be managed in main server
4. Modmail setting management/commands is locked to main server only

You can simply invite bot to multiple servers and you can start using the bot

Ticket channels will created in main server

# Installation
1. Install .NET 6
2. Download project build and publish for your desired platform
3. Visit Discord Developer Portal and create a new application
4. Configure app.settings
5. Run the app

# Possible Upcoming Features
- Web UI and/or API to manage tickets and see transcript information
- Encrypt message information
- Confirmation for close and opening tickets
- Language file support
- Editing embeds and colors and maybe more (need web ui for this) 
- Improve ticket type system, force ticket type selection before opening ticket etc.

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