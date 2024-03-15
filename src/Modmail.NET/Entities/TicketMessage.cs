﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Modmail.NET.Entities;

public class TicketMessage
{
  [Key]
  public Guid Id { get; set; }
  
  public ulong AuthorId { get; set; }

  public string Discriminator { get; set; }

  public string Username { get; set; }

  public string MessageContent { get; set; }
  
  public ulong GuildId { get; set; }
  
  public ulong ChannelId { get; set; }
  
  public ulong MessageId { get; set; }
  
  //FK
  public virtual Guid TicketId { get; set; }
  public virtual List<TicketMessageAttachment> TicketAttachments { get; set; }
}