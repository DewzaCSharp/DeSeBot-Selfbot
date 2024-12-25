using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

public class SendMessage
{
    [Command("sendmessage")]
    [Description("Usage: sendmessage [channelid] [message]")]
    public static async Task sendcustommsg(CommandContext ctx, DiscordChannel channel = null, string message = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            await ctx.RespondAsync("please enter a message.\nUsage: sendmessage [channel] [message]");
        }
        var channelPermissions = ctx.Member.PermissionsIn(channel);
        if (!channelPermissions.HasFlag(Permissions.SendMessages))
        {
            await ctx.RespondAsync("[ i ] You have to permissions to send messages.");
            return;
        }

        try
        {
            await channel.SendMessageAsync(message);
            await ctx.RespondAsync($"[ i ] Successfully send `{message}` in {channel.Mention}.");
        }
        catch (Exception ex)
        {
            await ctx.RespondAsync($"[ i ] Error: {ex.Message}");
        }
    }
}