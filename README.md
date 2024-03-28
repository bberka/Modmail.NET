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
- Toggle anonymous messages by mods, allows responding tickets anonymously
- Setting up teams adding roles and members to team for ticket management and access
- Setting priority for tickets (adds emoji to ticket channel name)
- Adding private mod notes to tickets
- Pinging team roles/members on ticket open
- Caching user information 
- Blacklist, blocking users from opening tickets 
- Getting feedback from user after ticket is closed
- Ticket type system, users can select ticket types to select what the ticket is about
- Moderators can talk with each other in ticket channel with by starting messages with bot prefix

# Multiple Servers Usage
Before trying to use the bot on multiple servers you must now about the limitations;

1. This bot can only be setup once and only for main server
2. Ticket management/commands only allowed on the main server, which id provided in configuration
3. Team management/commands is locked to main server only, meaning teams and members can only be managed in main server
4. Modmail setting management/commands is locked to main server only
5. Ticket types management/commands is locked to main server only

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
- Improve ticket type system, add optional feature forcing ticket type selection to be set before opening ticket etc.
- Add form modals when selecting ticket type and send information to ticket channel

# Commands
Parameter types with '*' are required

## Modmail Group Slash Commands 
Requires TeamPermissionLevel.Admin or higher

Only available for main server id in configuration

### `/modmail setup`

- **Description**: Setup the modmail bot.
- **Parameters**:
  - `sensitive-logging`: Whether to log modmail messages.
  - `take-feedback`: Whether to take feedback after closing tickets.
  - `greeting-message`: The greeting message.
  - `closing-message`: The closing message.

### `/modmail configure`

- **Description**: Configure the modmail bot.
- **Parameters**:
  - `sensitive-logging`: Whether to log modmail messages. (Optional)
  - `take-feedback`: Whether to take feedback after closing tickets. (Optional)
  - `greeting-message`: The greeting message. (Optional)
  - `closing-message`: The closing message. (Optional)

### `/modmail get-settings`

- **Description**: Get the modmail bot settings.


## Ticket Group Slash Commands 
Requires TeamPermissionLevel.Moderator or higher

Only available in main server id set in configuration

Only available in ticket channel

This set of commands allows moderators or higher-level users to manage tickets in the Discord server.

### `/ticket close`

- **Description**: Close a ticket.
- **Parameters**:
  - `reason`: Ticket closing reason. (Optional)

### `/ticket set-priority`

- **Description**: Set the priority of a ticket.
- **Parameters**:
  - `priority`: Priority of the ticket.

### `/ticket add-note`

- **Description**: Add a note to a ticket.
- **Parameters**:
  - `note`: Note to add.

### `/ticket toggle-anonymous`

- **Description**: Toggle anonymous mode for a ticket.

### `/ticket set-type`

- **Description**: Set the type of a ticket.
- **Parameters**:
  - `type`: Type of the ticket.

### `/ticket get-type`

- **Description**: Gets the ticket type for the current ticket channel.
- **Usage**: `/ticket-type get`


## Team Group Slash Commands
Requires TeamPermissionLevel.Admin or higher

Only available in main server id set in configuration

This set of commands allows admins or higher-level users to manage teams in the Discord server.
### `/team list`

- **Description**: List all teams.

### `/team create`

- **Description**: Create a new team.
- **Parameters**:
  - `teamName`: Team name.
  - `permissionLevel`: Permission level.

### `/team remove`

- **Description**: Remove a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)

### `/team add-user`

- **Description**: Add a user to a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)
  - `member`: Member to add to the team.

### `/team remove-user`

- **Description**: Remove a user from a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)
  - `member`: Member to remove from the team.

### `/team add-role`

- **Description**: Adds a role to a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)
  - `role`: Role to add to the team.

### `/team remove-role`

- **Description**: Removes a role from a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)
  - `role`: Role to remove from the team.

### `/team rename`

- **Description**: Rename a team.
- **Parameters**:
  - `teamName`: Team name. (Auto-completed)
  - `newName`: New team name.


## Blacklist Group Slash Commands
Requires TeamPermissionLevel.Moderator or higher

This set of commands allows moderators or higher-level users to manage the blacklist in the Discord server.

### `/blacklist add`

- **Description**: Add a user to the blacklist.
- **Parameters**:
    - `user`: The user to blacklist.
    - `[notify-user]`: Whether to notify the user about the blacklist. Default is `True`.
    - `[reason]`: The reason for blacklisting. Default is "No reason provided."
- **Usage**: `/blacklist add [user] [notify-user] [reason]`

### `/blacklist remove`

- **Description**: Remove a user from the blacklist.
- **Parameters**:
    - `user`: The user to remove from the blacklist.
    - `[notify-user]`: Whether to notify the user about the removal. Default is `True`.
- **Usage**: `/blacklist remove [user] [notify-user]`

### `/blacklist status`

- **Description**: Check if a user is blacklisted.
- **Parameters**:
    - `user`: The user to check.
- **Usage**: `/blacklist status [user]`

### `/blacklist view`

- **Description**: View all blacklisted users.
- **Usage**: `/blacklist view`



## Ticket Type Group Slash Commands
Requires TeamPermissionLevel.Admin or higher

### `/ticket-type create`

- **Description**: Create a new ticket type.
- **Parameters**:
  - `name`: The name of the ticket type.
  - `embed-message-title`: The title of the embed message.
  - `embed-message-content`: The content of the embed message.
  - `emoji`: The emoji used for this ticket type.
  - `description` (optional): The description of the ticket type.
  - `order` (optional): The order of the ticket type. Default is 0.
- **Usage**: `/ticket-type create [name] [embed-message-title] [embed-message-content] [emoji] [description] [order]`

### `/ticket-type update`

- **Description**: Update existing ticket type.
- **Parameters**:
  - `name`: The name of the ticket type. (Auto-completed)
  - `embed-message-title`: The title of the embed message.
  - `embed-message-content`: The content of the embed message.
  - `emoji`: The emoji used for this ticket type.
  - `description` (optional): The description of the ticket type.
  - `order` (optional): The order of the ticket type. Default is 0.
- **Usage**: `/ticket-type update [name] [embed-message-title] [embed-message-content] [emoji] [description] [order]`

### `/ticket-type delete`

- **Description**: Delete a ticket type.
- **Parameters**:
  - `name`: The name of the ticket type. (Auto-completed)
- **Usage**: `/ticket-type delete [name]`

### `/ticket-type list`

- **Description**: List all ticket types.
- **Usage**: `/ticket-type list`



# Disclaimer
You are responsible for data safety of users and messages when you are using this bot.