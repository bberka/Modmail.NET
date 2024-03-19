# Modmail.NET
An Open-Source Modmail Discord Bot made with .NET 6 for self hosting

This bot can be easily setup and used on your server for managing community communication on a single way.

Bot creates tickets and users message with the bot directly instead of sending message to mods

# Warning
This project is may not be ready for production needs more features and polishing.

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
- Support for multiple servers (groundwork is done)
- Getting feedback from user when ticket closed
- Encrpyt message information
- Setting up greeting/closing messages
- Confirmation for close and opening tickets
- Confirmation for sending attachments
- Blacklist

# Commands
Parameter types with '*' are required

## Modmail Group Slash Commands 
`/modmail setup (bool sensitiveLogging, bool takeFeedbackAfterClosing)`: Setup the modmail bot

`/modmail get-settings`: Get the modmail bot settings

`/modmail toggle-sensitive-logging`: Toggle sensitive logging for the modmail bot

(NOT IMPLEMENTED) `/modmail toggle-take-feedback`: Toggle taking feedback after closing tickets



## Ticket Group Slash Commands 
`/ticket close (string reason)`: Close a ticket

`/ticket set-priority (TicketPriority priority)`: Set the priority of a ticket

`/ticket add-note (string note)`: Add a note to a ticket

`/ticket toggle-anonymous`: Toggle anonymous mode for a ticke

## Team Group Slash Commands 
`/team list`: List all teams

`/team create (string* teamName, TeamPermissionLevel* permissionLevel)`: Create a new team

`/team remove (string* teamName)`: Remove a team

`/team add-user (string* teamName, DiscordUser* member)`: Add a user to a team

`/team remove-user (string* teamName, DiscordUser* member)`: Remove a user from a team

`/team add-role (string* teamName, DiscordRole* role)`: Adds a role to a team

`/team remove-role (string* teamName, DiscordRole* role)`: Removes a role from a team

# Disclaimer
You are responsible for data safety of users and messages when you are using this bot.
