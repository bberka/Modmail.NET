using Ardalis.SmartEnum;

namespace Modmail.NET.Static;

public class HangfireQueueName : SmartEnum<HangfireQueueName>
{
  public static readonly HangfireQueueName Default = new(nameof(Default), 0);
  public static readonly HangfireQueueName AttachmentDownload = new(nameof(AttachmentDownload), 1);
  private HangfireQueueName(string name, int value) : base(name, value) { }
}