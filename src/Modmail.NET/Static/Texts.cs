﻿// namespace Modmail.NET.Static;
//
// public static class Texts
// {
//   public const string SERVER_NOT_SETUP = "Server is not setup!";
//   public const string SETUP_SERVER_BEFORE_USING = "Please setup the server before using this command.";
//   public const string OPENED_BY = "Opened By";
//   public const string TICKET_ID = "Ticket Id";
//   public const string OPENED_AT = "Opened At";
//   public const string CLOSED_BY = "Closed By";
//   public const string CLOSE_REASON = "Close Reason";
//   public const string TICKET_CLOSED = "Ticket Closed";
//   public const string NO_REASON_PROVIDED = "No reason provided";
//   public const string OLD_PRIORITY = "Old Priority";
//   public const string NEW_PRIORITY = "New Priority";
//   public const string TICKET_PRIORITY_CHANGED = "Ticket priority has been changed.";
//   public const string MESSAGE_SENT_BY_MOD = "Message Sent by Mod";
//   public const string USER_ID = "User Id";
//   public const string CHANNEL_ID = "Channel Id";
//   public const string ANONYMOUS = "Anonymous";
//   public const string THIS_MESSAGE_SENT_ANONYMOUSLY = "This message was sent anonymously.";
//   public const string ATTACHMENT = "Attachment";
//   public const string NEW_TICKET_CREATED = "New Ticket Created";
//   public const string USER = "User";
//   public const string USERNAME = "Username";
//   public const string TICKET_CREATED_AT = "Ticket Created At";
//   public const string MESSAGE_SENT_BY_USER = "Message Sent by User";
//   public const string NOTE_ADDED = "Note Added";
//   public const string FEEDBACK = "Feedback";
//   public const string FEEDBACK_DESCRIPTION = "Please rate the answers you receive. This helps us improve our moderation team.";
//   public const string FEEDBACK_RECEIVED = "Feedback Received";
//   public const string STAR = "Star";
//   public const string ANONYMOUS_TOGGLED = "Anonymous Toggled";
//   public const string TOGGLED_BY = "Toggled By";
//   public const string TICKET_SET_ANONYMOUS_DESCRIPTION = "This ticket is now anonymous. The member will not know who is responding to their messages.";
//   public const string TICKET_SET_NOT_ANONYMOUS_DESCRIPTION = "This ticket is no longer anonymous. The member can see who is responding to their messages.";
//   public const string USER_BLACKLISTED = "User blacklisted";
//   public const string USER_BLACKLIST_REMOVED = "User removed from blacklist";
//   public const string STAR_EMOJI = ":star:";
//   public const string ANONYMOUS_MESSAGE = "Anonymous Message";
//   public const string NEW_TICKET = "New Ticket";
//
//   public const string ROLES = "Roles";
//   public const string YOU_HAVE_BEEN_BLACKLISTED = "You have been blacklisted";
//   public const string YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION = "You have been blacklisted from using the modmail system. Your messages will not be received.";
//   public const string YOUR_TICKET_HAS_BEEN_CLOSED = "Your ticket has been closed";
//   public const string YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";
//   public const string YOU_HAVE_CREATED_NEW_TICKET = "You have created a new ticket";
//   public const string MODMAIL_SETTINGS = "Modmail Settings";
//   public const string ENABLED = "Enabled";
//   public const string SENSITIVE_LOGGING = "Sensitive Logging";
//   public const string TAKE_FEEDBACK_AFTER_CLOSING = "Take Feedback After Closing";
//   public const string SHOW_CONFIRMATIONS = "Show Confirmations";
//   public const string LOG_CHANNEL = "Log Channel";
//   public const string TICKET_CATEGORY = "Tickets Category";
//   public const string GREETING_MESSAGE = "Greeting Message";
//   public const string CLOSING_MESSAGE = "Closing Message";
//   public const string TEAM_LIST = "Team List";
//   public const string PERMISSION_LEVEL = "Permission Level";
//   public const string MEMBERS = "Members";
//   public const string ROLE = "Role";
//   public const string THANK_YOU_FOR_FEEDBACK = "Thank you for your feedback!";
//   public const string CHANNEL_WAS_DELETED = "Channel was deleted";
//   public const string USER_HAS_ACTIVE_TICKET = "User has an active ticket!";
//   public const string PLEASE_CLOSE_THE_TICKET_BEFORE_BLACKLISTING = "Please close the ticket before blacklisting the user.";
//   public const string USER_ALREADY_BLACKLISTED = "User is already blacklisted";
//   public const string USER_IS_NOT_BLACKLISTED = "User is not blacklisted";
//   public const string USER_BLACKLIST_STATUS = "User Blacklist Status";
//   public const string USER_IS_BLACKLISTED = "User is blacklisted";
//   public const string BLACKLISTED_USERS = "Blacklisted Users";
//   public const string ANOTHER_SERVER_ALREADY_SETUP = "Another server is already setup, this bot only supports single server setup.";
//   public const string MAIN_SERVER_ALREADY_SETUP = "Main server already setup!";
//   public const string THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER = "This command can only be used in the main server.";
//   public const string MODMAIL_LOG_CHANNEL_TOPIC = "Modmail log channel";
//   public const string SERVER_SETUP_COMPLETE = "Server setup complete!";
//   public const string SERVER_CONFIG_UPDATED = "Server configuration updated!";
//   public const string MODERATION_CONCERNS = "moderation concerns";
//   public const string SYSTEM_IS_BUSY = "System is busy";
//   public const string YOUR_MESSAGE_COULD_NOT_BE_PROCESSED = "Your message could not be processed. Please try again later.";
//   public const string TICKET_TYPE_CREATED = "Ticket Type Created";
//   public const string TICKET_TYPE_CREATED_DESCRIPTION = "Ticket type `{0}` created";
//   public const string TICKET_TYPE_ALREADY_EXISTS = "Ticket Type Already Exists";
//   public const string TICKET_TYPE_EXISTS_DESCRIPTION = "Ticket type with name '{0}' already exists";
//   public const string TICKET_TYPES = "Ticket Types";
//   public const string TICKET_TYPE_NOT_FOUND = "Ticket type not found";
//   public const string TICKET_TYPE_NOT_FOUND_DESCRIPTION = "Ticket type with name '{0}' not found";
//   public const string TICKET_TYPE_DELETED = "Ticket Type Deleted";
//   public const string TICKET_TYPE_DELETED_DESCRIPTION = "Ticket type {0} deleted";
//   public const string INVALID_ORDER = "Invalid Order";
//   public const string INVALID_ORDER_DESCRIPTION = "Order value is invalid";
//   public const string PLEASE_SELECT_A_TICKET_TYPE = "Please select a ticket type";
//   public const string TICKET_NOT_FOUND = "Ticket not found";
//   public const string TICKET_TYPE = "Ticket Type";
//
//   // public const string TICKET_TYPE_SELECTION_TIMEOUT = "Ticket type selection timeout";
//   public const string TICKET_TYPE_CHANGED = "Ticket type changed";
//   public const string TICKET_TYPE_SET = "Ticket type set to {0} `{1}`";
//   public const string USER_NOT_FOUND = "User not found";
//   public const string THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL = "This command can only be used in a ticket channel";
//   public const string REASON = "Reason";
//   public const string TICKET_TYPE_UPDATED = "Ticket Type Updated";
//   public const string TICKET_TYPE_UPDATED_DESCRIPTION = "Ticket type {0} updated";
//   public const string CLOSE_TICKET = "Close Ticket";
//   public const string CLOSE_TICKET_WITH_REASON = "Close Ticket with Reason";
//   public const string TICKET_CLOSED_SUCCESSFULLY = "Ticket closed successfully";
//   public const string TICKET_ALREADY_CLOSED = "Ticket already closed";
//   public const string TICKET_ANONYMOUS_TOGGLED = "Ticket anonymous toggled";
//   public const string INVALID_USER = "Invalid User";
//   public const string CHANNEL_NOT_FOUND = "Channel not found";
//   public const string INVALID_MESSAGE_ID = "Invalid message id";
//   public const string NO_TEAM_FOUND = "No team found";
//   public const string TEAM_WITH_SAME_NAME_ALREADY_EXISTS = "Team with the same name already exists";
//   public const string TEAM_CREATED_SUCCESSFULLY = "Team created successfully";
//   public const string TEAM_NOT_FOUND = "Team not found";
//   public const string TEAM_REMOVED_SUCCESSFULLY = "Team removed successfully";
//   public const string MEMBER_ALREADY_IN_TEAM = "Member already in team";
//   public const string MEMBER_ADDED_TO_TEAM = "Member added to team";
//   public const string MEMBER_NOT_FOUND_IN_TEAM = "Member not found in team";
//   public const string MEMBER_REMOVED_FROM_TEAM = "Member removed from team";
//   public const string ROLE_ALREADY_IN_TEAM = "Role already in team";
//   public const string ROLE_ADDED_TO_TEAM = "Role added to team";
//   public const string ROLE_NOT_FOUND_IN_TEAM = "Role not found in team";
//   public const string ROLE_REMOVED_FROM_TEAM = "Role removed from team";
//   public const string TEAM_RENAMED_SUCCESSFULLY = "Team renamed successfully";
//   public const string NO_TICKET_TYPES_FOUND = "No ticket types found";
//   public const string TICKET_CLOSED_DUE_TO_BLACKLIST = "Ticket closed due to blacklist";
//   public const string PLEASE_TELL_US_REASONS_FOR_YOUR_RATING = "Please tell us reasons for your rating";
//   public const string ENTER_A_REASON_FOR_CLOSING_THIS_TICKET = "Enter a reason for closing this ticket";
//   public const string ANONYMOUS_MOD_ON = "Anonymous mod on";
//   public const string ANONYMOUS_MOD_OFF = "Anonymous mod off";
//   public const string YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST = "You have been removed from blacklist";
//   public const string YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST_DESCRIPTION = "You have been removed from the blacklist. You can now use the modmail system.";
//   public const string NO_BLACKLISTED_USERS = "There is no blacklisted users";
//   public const string LOG_CHANNEL_NOT_FOUND = "Log channel not found";
//   public const string AN_EXCEPTION_OCCURRED = "An exception occurred, please check the logs";
//   public const string YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND = "You do not have permission to use this command";
//   public const string MAIN_GUILD_NOT_FOUND = "Main guild not found";
//   public const string MAIN_GUILD_NOT_FOUND_DESC = "You must invite the bot to the main server first";
//   public const string NO_BLACKLISTED_USERS_FOUND = "No blacklisted users found";
//   public const string TEAM_ALREADY_EXISTS = "Team already exists";
//   public const string INVALID_NAME = "Invalid name";
//   public const string TEAM_CREATED = "Team created";
//   public const string TEAM_NAME = "Team Name";
//   public const string TEAM_REMOVED = "Team removed";
//   public const string TEAM_MEMBER_ADDED = "Team member added";
//   public const string TEAM_MEMBER_REMOVED = "Team member removed";
//   public const string TEAM_ROLE_ADDED = "Team role added";
//   public const string TEAM_ROLE_REMOVED = "Team role removed";
//   public const string TEAM_RENAMED = "Team renamed";
//   public const string OLD_NAME = "Old Name";
//   public const string NEW_NAME = "New Name";
//   public const string SETUP_COMPLETE = "Setup complete";
//   public const string GUILD_NAME = "Guild Name";
//   public const string GUILD_ID = "Guild Id";
//   public const string CATEGORY_ID = "Category Id";
//   public const string LOG_CHANNEL_ID = "Log Channel Id";
//   public const string CONFIGURATION_UPDATED = "Configuration updated";
//   public const string EMOJI = "Emoji";
//   public const string DESCRIPTION = "Description";
//   public const string EMBED_MESSAGE_TITLE = "Embed Message Title";
//   public const string EMBED_MESSAGE_CONTENT = "Embed Message Content";
//   public const string ORDER = "Order";
//   public const string INVALID_INTERACTION_KEY = "Invalid interaction key";
//   public const string TEAM_UPDATED = "Team updated";
//   public const string OLD_PERMISSION_LEVEL = "Old Permission Level";
//   public const string NEW_PERMISSION_LEVEL = "New Permission Level";
//   public const string OLD_PING_ON_NEW_TICKET = "Old Ping On New Ticket";
//   public const string NEW_PING_ON_NEW_TICKET = "New Ping On New Ticket";
//   public const string OLD_PING_ON_NEW_MESSAGE = "Old Ping On New Message";
//   public const string NEW_PING_ON_NEW_MESSAGE = "New Ping On New Message";
//   public const string OLD_IS_ENABLED = "Old Is Enabled";
//   public const string NEW_IS_ENABLED = "New Is Enabled";
//   public const string PING_ON_NEW_TICKET = "Ping On New Ticket";
//   public const string PING_ON_NEW_MESSAGE = "Ping On New Message";
//   public const string PERMISSION_LEVEL_UPDATED = "Permission level updated";
//   public const string PING_ON_NEW_TICKET_UPDATED = "Ping on new ticket updated";
//   public const string PING_ON_NEW_MESSAGE_UPDATED = "Ping on new message updated";
//   public const string IS_ENABLED_UPDATED = "Is enabled updated";
//   public const string TEAM_UPDATED_SUCCESSFULLY = "Team updated successfully";
//
//
//   public static string NEW_TICKET_DESCRIPTION_MESSAGE = "New ticket has been created. Please respond to this message to continue the conversation."
//                                                         + Environment.NewLine
//                                                         + Environment.NewLine
//                                                         + "If you want to close the ticket, you can use the `/ticket close` command."
//                                                         + Environment.NewLine
//                                                         + Environment.NewLine
//                                                         + "If you want to change the priority of the ticket, you can use the `/ticket set-priority` command."
//                                                         + Environment.NewLine
//                                                         + Environment.NewLine
//                                                         + "If you want to add a note to the ticket, you can use the `/ticket add-note` command."
//                                                         + Environment.NewLine
//                                                         + Environment.NewLine
//                                                         + "If you want to toggle anonymous response, you can use the `/ticket toggle-anonymous` command."
//                                                         + Environment.NewLine
//                                                         + Environment.NewLine
//                                                         + $"Messages starting with bot prefix `{BotConfig.This.BotPrefix}` are ignored, can be used for staff discussion. ";
// }
//

