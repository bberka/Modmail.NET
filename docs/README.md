# Modmail.NET

An Open-Source feature rich Modmail Discord Bot made with .NET 

This bot can be easily setup and used on your server for managing community communication on a single way.

Bot creates tickets and users message with the bot directly instead of sending message to mods.

This bot is only available through self hosting, you can host it anywhere.

# Features

- Open tickets by sending a private message to bot
- Close tickets by using command or deleting the channel or web ui
- Logging to messages/transcript to database and modmail log channel (sensitive logging)
- Admins or mod team can respond tickets by sending message to created channel
- Toggle anonymous messages by mods, allows responding tickets anonymously
- Setting up teams adding roles and members to team for ticket management and access
- Setting priority for tickets (adds emoji to ticket channel name)
- Adding private mod notes to tickets
- Pinging team roles/members on ticket open
- Caching user information on certain guild events
- Blacklist, blocking users from opening tickets to avoid spam
- Getting feedback from user after ticket is closed. User can give up to 5 stars and reason.
- Ticket type system, users can select ticket types to select what the ticket is about.
- Moderators can talk with each other in ticket channel with by starting messages with bot prefix. To avoid message
  being sent to user.
- Web UI for configuring and seeing bot information


# Installation

1. Install .NET 9 SDK
2. Download project build and publish for your desired platform or download build from github
3. Visit Discord Developer Portal and create a new application
4. Configure appsettings.json
5. Run the app in your server

# Multiple Servers Usage

Before trying to use the bot on multiple servers you must now about the limitations;

1. This bot can only be setup once and only for main server
2. Ticket management/commands only allowed on the main server, which id provided in configuration
3. Team management/commands is locked to main server only, meaning teams and members can only be managed in main server
4. Modmail setting management/commands is locked to main server only
5. Ticket types management/commands is locked to main server only

You can simply invite bot to multiple servers and you can start using the bot

Ticket channels will created in main server


# [Discord Commands](COMMANDS.md)
# [Changelog](CHANGELOG.md)
# [Roadmap and Bugs](ROADMAP.md)
# [Web UI Images](img)

# Warning

This project may come with bugs, missing features and problems.

We are using this as our main ticket system actively however it is not fully released.

# Contributing

Project is open to contributing if you willing to spend time fixing things or adding features. Much appreciated.

# Disclaimer

You are responsible for data safety of users and messages when you are using this bot.