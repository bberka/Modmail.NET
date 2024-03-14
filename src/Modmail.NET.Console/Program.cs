using Modmail.NET.Common;

namespace Modmail.NET.Console;

class Program
{
  static async Task Main(string[] args) {
    
    var bot = new ModmailBot();
    var taskList = new List<Task>();
    taskList.Add(bot.StartAsync());
    //Add API server start task here
    await Task.WhenAll(taskList);
  }
}