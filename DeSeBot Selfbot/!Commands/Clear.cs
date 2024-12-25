using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

public class ClearMsg
{
    [Command("clear")]
    [Description("Usage: clear [amount to clear]")]
    public static async Task Clear(CommandContext ctx, int limit)
    {
        if (limit <= 0)
        {
            await ctx.RespondAsync("please enter a limit.\nUsage: clear [amount to clear]");
        }
        if (limit == 0 || limit == null)
        {
            await ctx.Channel.SendMessageAsync("[ i ] Invalid Number Entered!");
        }
        else
        {
            var messages = await ctx.Channel.GetMessagesAsync(limit);
            foreach (var msg in messages)
            {
                if (msg.Author.Id == ctx.Client.CurrentUser.Id)
                {
                    await msg.DeleteAsync();
                }
            }

            await ctx.RespondAsync($"[ i ] Deleted Messages: {limit}");
        }
    }
}