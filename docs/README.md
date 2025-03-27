# Modmail.NET

**An Open-Source & Feature-Rich Modmail Discord Bot for .NET 9**

Modmail.NET provides a comprehensive solution for managing community communications within Discord servers. It's designed for easy setup and self-hosting, giving you complete control over your data and configuration.

This bot streamlines communication by creating tickets that allow users to message moderators directly, without needing to directly message them.

## Key Features

### Private Ticketing

Users open tickets by sending a private message to the bot.

### Centralized Communication

Moderators respond to tickets within dedicated channels.

### Versatile Ticket Management

*   Close tickets via command, channel deletion, or web UI.
*   Set ticket priority (adds emoji to ticket channel name).
*   Add private mod notes.

### Anonymous Responses

Moderators can toggle anonymous mode for responding to tickets.

### Team-Based Access

Set up teams, assign roles, and manage team members for efficient ticket handling.

### Event-Driven User Caching

User information is automatically cached during key Discord events.

### Blacklisting

Prevent spam by blocking users from opening tickets.

### Feedback System

Gather user feedback (ratings and reasons) after ticket closure.

### Ticket Types

Categorize tickets with a customizable ticket type system.

### Moderator-Only Chat

Moderators can communicate privately within ticket channels using a bot prefix.

### Comprehensive Web UI

Configure bot settings, manage teams, and view bot statistics through an intuitive web interface.

###  Logging

Logs messages and transcripts to a database and ticket actions to discord log channel.

## Getting Started

1.  **Prerequisites:** Install the [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
2.  **Download & Build:** Download the project source code and build/publish it for your desired platform. You can also grab a pre-built release from the [GitHub Releases](link-to-releases-when-available)  *(Currently not available)*.
3.  **Discord Application:** Create a new application on the [Discord Developer Portal](https://discord.com/developers/applications).
4.  **Configuration:** Configure the bot's settings in `appsettings.json`.
5.  **Run:** Start the application on your server.

## Using the Bot Across Multiple Discord Servers

Modmail.NET is designed with a main server in mind, which handles the core functionality. Please be aware of the following limitations when using the bot across multiple servers:

1.  The bot can only be set up once and for the designated main server.
2.  Ticket management commands are only available on the main server.
3.  Team management, modmail settings, and ticket type management are locked to the main server.

To use Modmail.NET on multiple servers, simply invite the bot to each server. Ticket channels will be created in the main server for cross-server communication.

## Explore the Documentation

*   [Discord Commands](COMMANDS.md) - Learn about the available bot commands and their usage.
*   [Changelog](CHANGELOG.md) - See what's new in the latest releases.
*   [Web UI Images](img) - Get a visual overview of the web interface.
*   [Roadmap](ROADMAP.md) - View the roadmap!

## Important: Please Read!

This project is under active development and may contain bugs, missing features, or other issues. We use Modmail.NET as our main ticket system, but it is not yet considered a fully stable release.

## Contributing

We welcome contributions from the community! If you're interested in helping fix bugs or add new features, please feel free to submit pull requests.

## Disclaimer

By using Modmail.NET, you acknowledge that you are responsible for ensuring the data safety and privacy of your users and their messages.