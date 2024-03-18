# Modmail.NET
An Open-Source Modmail Discord Bot made with .NET 6 for self hosting

This bot can be easily setup and used on your server for managing community communication on a single way.

Bot creates tickets and users message with the bot directly instead of sending message to mods

# Warning
This project is not ready for production needs more features and polishing.

If you wish to use it be aware of potential errors

# Features
- Open tickets by sending a private message to bot
- Close tickets by using command or deleting the channel
- Logging to messages/transcript to database
- Admins or mod team can respond tickets by sending message to created channel
- Toggle sensitive logging (Logging messages to tickets)
- Toggle anonymous messages by mods
- Setting up teams adding roles and members to teams (partly done)
- Setting priority for tickets (adds emoji to tikcet channel name)
  
# Contributing
Create a pull request by using semantic commits and proper explanation

# Installation
1. Install .NET 6
2. Download project build and publish for your desired platform
3. Visit Discord Developer Portal and create a new application
4. Setup your bot configuration
5. Run the app

# Possible Upcoming Features
- :white_check_mark: Snippets or tags system
- :white_check_mark: Advanced message logging to logging channel (optional)
- :white_check_mark: Make db message logging optional
- :white_check_mark: Responding tickets anonymously
- :white_check_mark: Setting priority for tickets
- Web UI and/or API to manage tickets and see transcript information
- Support for multiple servers (groundwork is done)
- Getting feedback from user when ticket closed
- Encrpyt message information
- Role and team system for allowing channel access
- Pinging roles/members on ticket open
- Teams and permission management
- Setting up greeting/closing messages
- Confirmation for close and opening tickets
- Confirmation for sending attachments
- Blacklist
- Setup some filter to open tickets


# Disclaimer
You are responsible for data safety of users and messages when you are using this bot.
