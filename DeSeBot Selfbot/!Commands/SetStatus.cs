using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

public class Status
{
    [Command("setstatus")]
    [Description("Usage: setstatus [status](online,dnd,idle or invisible)")]
    public static async Task ChangeStatus(CommandContext ctx, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            await ctx.RespondAsync("please enter an Status.\nUsage: setstatus [status](online,dnd,idle or invisible)");
        }
        DSharpPlus.Entities.UserStatus newStatus;
        switch (status.ToLower())
        {
            case "online":
                newStatus = DSharpPlus.Entities.UserStatus.Online;
                break;
            case "dnd":
                newStatus = DSharpPlus.Entities.UserStatus.DoNotDisturb;
                break;
            case "idle":
                newStatus = DSharpPlus.Entities.UserStatus.Idle;
                break;
            case "invisible":
                newStatus = DSharpPlus.Entities.UserStatus.Invisible;
                break;
            default:
                await ctx.RespondAsync("Invalid status! available: `online`, `dnd`, `idle`, or `invisible`.");
                return;
        }

        await ctx.Client.UpdateStatusAsync(user_status: newStatus);
        await ctx.RespondAsync($"Status changed to {status}.");
    }
}