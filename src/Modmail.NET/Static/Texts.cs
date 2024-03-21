﻿using Modmail.NET.Common;

namespace Modmail.NET.Static;

public static class Texts
{
  public const string SERVER_NOT_SETUP = "Server not setup!";
  public const string SETUP_SERVER_BEFORE_USING = "Please setup the server before using this command.";
  public const string OPENED_BY_USER = "Opened By User";
  public const string OPENED_BY_USER_ID = "Opened By User Id";
  public const string OPENED_BY_USERNAME = "Opened By Username";
  public const string TICKET_ID = "Ticket Id";
  public const string OPENED_AT = "Opened At";
  public const string CLOSED_BY = "Closed By";
  public const string CLOSE_REASON = "Close Reason";
  public const string TICKET_CLOSED = "Ticket Closed";
  public const string NO_REASON_PROVIDED = "No reason provided";
  public const string OLD_PRIORITY = "Old Priority";
  public const string NEW_PRIORITY = "New Priority";
  public const string TICKET_PRIORITY_CHANGED = "Ticket priority has been changed.";
  public const string MESSAGE_SENT_BY_MOD = "Message Sent by Mod";
  public const string USER_ID = "User Id";
  public const string CHANNEL_ID = "Channel Id";
  public const string ANONYMOUS = "Anonymous";
  public const string THIS_MESSAGE_SENT_ANONYMOUSLY = "This message was sent anonymously.";
  public const string ATTACHMENT = "Attachment";
  public const string NEW_TICKET_CREATED = "New Ticket Created";
  public const string USER = "User";
  public const string USERNAME = "Username";
  public const string TICKET_CREATED_AT = "Ticket Created At";
  public const string MESSAGE_SENT_BY_USER = "Message Sent by User";
  public const string NOTE_ADDED = "Note Added";
  public const string FEEDBACK = "Feedback";
  public const string FEEDBACK_DESCRIPTION = "Please rate the answers you receive. This helps us improve our moderation team.";
  public const string FEEDBACK_RECEIVED = "Feedback received";
  public const string STAR = "Star";
  public const string ANONYMOUS_TOGGLED =  "Anonymous Toggled";
  public const string TOGGLED_BY = "Toggled By";
  public const string TICKET_SET_ANONYMOUS_DESCRIPTION = "This ticket is now anonymous. The member will not know who is responding to their messages.";
  public const string TICKET_SET_NOT_ANONYMOUS_DESCRIPTION = "This ticket is no longer anonymous. The member can see who is responding to their messages.";
  public const string USER_BLACKLISTED = "User blacklisted";
  public const string USER_BLACKLIST_REMOVED = "User removed from blacklist";
  public const string STAR_EMOJI = ":star:";
  public const string ANONYMOUS_MESSAGE = "Anonymous Message";
  public const string NEW_TICKET = "New Ticket";
  public static string NEW_TICKET_DESCRIPTION_MESSAGE = "New ticket has been created. Please respond to this message to continue the conversation."
                                                       + Environment.NewLine
                                                       + Environment.NewLine
                                                       + "If you want to close the ticket, you can use the `/ticket close` command."
                                                       + Environment.NewLine
                                                       + Environment.NewLine
                                                       + "If you want to change the priority of the ticket, you can use the `/ticket set-priority` command."
                                                       + Environment.NewLine
                                                       + Environment.NewLine
                                                       + "If you want to add a note to the ticket, you can use the `/ticket add-note` command."
                                                       + Environment.NewLine
                                                       + Environment.NewLine
                                                       + "If you want to toggle anonymous response, you can use the `/ticket toggle-anonymous` command."
                                                       + Environment.NewLine
                                                       + Environment.NewLine
                                                       + $"Messages starting with bot prefix `{MMConfig.This.BotPrefix}` are ignored, can be used for staff discussion. "
    ;

  public const string ROLES = "Roles";
  public const string YOU_HAVE_BEEN_BLACKLISTED = "You have been blacklisted";
  public const string YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION = "You have been blacklisted from using the modmail system. Your messages will not be received.";
  public const string YOUR_TICKET_HAS_BEEN_CLOSED = "Your ticket has been closed";
  public const string YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";
  public const string YOU_HAVE_CREATED_NEW_TICKET = "You have created a new ticket";
  public const string MODMAIL_SETTINGS = "Modmail Settings";
  public const string ENABLED = "Enabled";
  public const string SENSITIVE_LOGGING = "Sensitive Logging";
  public const string TAKE_FEEDBACK_AFTER_CLOSING = "Take Feedback After Closing";
  public const string SHOW_CONFIRMATIONS = "Show Confirmations";
  public const string LOG_CHANNEL = "Log Channel";
  public const string TICKET_CATEGORY = "Tickets Category";
  public const string GREETING_MESSAGE = "Greeting Message";
  public const string CLOSING_MESSAGE = "Closing Message";  
  public const string TEAM_LIST = "Team List";
  public const string PERMISSION_LEVEL = "Permission Level";
  public const string MEMBERS = "Members";
  public const string ROLE = "Role";
  public const string THANK_YOU_FOR_FEEDBACK = "Thank you for your feedback!";
  public const string? CHANNEL_WAS_DELETED = "Channel was deleted";
}