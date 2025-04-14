# Commands

Parameter types with '*' are required

## Ticket Group Slash Commands

Only available in main server id set in configuration

Only available in ticket channel

This set of commands allows moderators or higher-level users to manage tickets in the Discord server.

### `/ticket close`

Requires a team member or created user if enabled in options

- **Description**: Close a ticket.
- **Parameters**:
  - `reason`: Ticket closing reason. (Optional)

### `/ticket priority`

Requires TeamPermissionLevel.Support or higher

- **Description**: Set the priority of a ticket.
- **Parameters**:
  - `priority`: Priority of the ticket.

### `/ticket note`

Requires TeamPermissionLevel.Support or higher

- **Description**: Add a note to a ticket.
- **Parameters**:
  - `note`: Note to add.

### `/ticket anonymous`

Requires TeamPermissionLevel.Moderator or higher

- **Description**: Toggle anonymous mode for a ticket.

### `/ticket type`

Requires TeamPermissionLevel.Support or higher

- **Description**: Set the type of a ticket.
- **Parameters**:
  - `type`: Type of the ticket.

## Blacklist Group Slash Commands

Requires TeamPermissionLevel.Moderator or higher

This set of commands allows moderators or higher-level users to manage the blacklist in the Discord server.

### `/blacklist add`

- **Description**: Add a user to the blacklist.
- **Parameters**:
  - `user`: The user to blacklist.
  - `[reason]`: The reason for blacklisting. Default is "No reason provided."
- **Usage**: `/blacklist add [user] [notify-user] [reason]`

### `/blacklist remove`

- **Description**: Remove a user from the blacklist.
- **Parameters**:
  - `user`: The user to remove from the blacklist.
- **Usage**: `/blacklist remove [user] [notify-user]`

### `/blacklist status`

- **Description**: Check if a user is blacklisted.
- **Parameters**:
  - `user`: The user to check.
- **Usage**: `/blacklist status [user]`

## Tag Slash Commands

### `/tag`

- **Description**: Get tag content.
- **Parameters**:
  - `name`: Tag name.
- **Usage**: `/tag [name]`