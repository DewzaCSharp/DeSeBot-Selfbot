using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SimpleDiscordBot;
using System.Collections.Generic;

public class Misc
{
    public static async Task checkifOwner(CommandContext ctx)
    {
        if (MyCommands.ownercheck)
        {
            if (ctx.Message.Author.IsCurrent)
            {
                await Task.CompletedTask;
            }
            else
            {
                await ctx.RespondAsync("Sorry, but you cannot Execute Any Command!\n||You may ask if he could change that..||");
            }
        }
        else if (!MyCommands.ownercheck)
        {
            await Task.CompletedTask;
        }
    }
}
public class MyCommands
{
    public static bool ownercheck = false;
    private static readonly List<string> AllowedUsers = new List<string>
    {
        "dewzacsharp",
        "j4y404"
    };
    private bool IsUserAllowed(CommandContext ctx)
    {
        return AllowedUsers.Contains(ctx.User.Username);
    }

    [Command("information")]
    [Description("Gives you information about me.")]
    public async Task Information(CommandContext ctx)
    {
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        Console.WriteLine("[i] Information Command was Executed!");
        await ctx.Message.Channel.SendMessageAsync("> # Info\n" +
            "> **Version: **`1.0.0`\n" +
            "> **Founder of DeSeBot: **`DewzaCSharp`\n");
    }

    [Command("echo")]
    [Description("say a message as me | Usage: echo [message]")]
    public async Task SayCommand(CommandContext ctx, [RemainingText] string message = null)
    {
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            await ctx.RespondAsync("Please Enter the message that i should say\nExample: .echo hi");
            return;
        }
        if (message == "ass"
            || message == "nigger"
            || message == "nigga"
            || message == "fuck"
            || message == "fuck you"
            || message == "asshole")
        {
            await ctx.RespondAsync("im sorry, but the message cannot contain bad words.");
        }
        await ctx.RespondAsync(message);
    }

    // [Command("sendmessage")]
    // [Description("send a message as me | Usage: sendmessage [channelid] [message]")]
    // public async Task SendCustomMessageAsync(CommandContext ctx, DiscordChannel channel = null, [RemainingText] string message = null)
    // {
    //     Console.WriteLine("[i] SendCustomMessage Command was Executed!");
    //     await ctx.Channel.TriggerTypingAsync();
    //     await SendMessage.sendcustommsg(ctx, channel, message);
    // }

    [Command("clear")]
    [Description("deletes an amount of messages. | Usage: clear [amount to clear]")]
    public async Task ClearMessages(CommandContext ctx, int limit)
    {
        Console.WriteLine("[i] Clear Command was Executed!");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        await ClearMsg.Clear(ctx, limit);
    }

    [Command("commands")]
    [Description("Lists all available commands.")]
    public async Task ListAvailableCommandsAsync(CommandContext ctx)
    {
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        Console.WriteLine("[i] ListCommands Command was Executed!");

        var commandsList = ctx.CommandsNext.RegisteredCommands;
        var commandsWithInfo = new List<string>();

        foreach (var commandPair in commandsList)
        {
            var command = commandPair.Value;
            var commandName = commandPair.Key;
            var description = command.Description ?? "No description available.";

            var commandString = $"{commandName} - {description}";
            commandsWithInfo.Add(commandString);
        }

        await ctx.RespondAsync($"# Available Commands:\n > {string.Join("\n> ", commandsWithInfo.OrderBy(c => c))}");
    }

    [Command("newinvite")]
    [Description("creates a new invite | Usage: newinvite [max age (days)] [max uses]")]
    public async Task CreateInvite(CommandContext ctx, int max_age = 0, int max_uses = 0)
    {
        Console.WriteLine("[i] newinvite Command was Executed!");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        if (max_age <= 0 || max_uses <= 0)
        {
            await ctx.RespondAsync("please enter maximum age / uses.\nUsage: newinvite [max age (days)] [max uses]");
        }
        var invite = await ctx.Channel.CreateInviteAsync(
            max_age: max_age,
            max_uses: max_uses
        );
        await ctx.RespondAsync($"# Invite Created!\nHere is your invite Link: ```{invite.ToString}```");
    }

    [Command("setstatus")]
    [Description("change the current status |  Usage: setstatus [status](online,dnd,idle or invisible)")]
    public async Task SetStatusCommand(CommandContext ctx, string status)
    {
        Console.WriteLine("[i] setstatus Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        await Status.ChangeStatus(ctx, status);
    }

    [Command("avatar")]
    [Description("my avatar")]
    public async Task AvatarCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] avatar Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        var avatarUrl = ctx.Client.CurrentUser.AvatarUrl;
        await ctx.RespondAsync($"{ctx.Client.CurrentUser.Username}'s Avatar\n[Avatar]({avatarUrl})");
    }

    [Command("serverinfo")]
    [Description("Gives information about the current server.")]
    public async Task ServerInfoCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] serverinfo Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        var server = ctx.Guild;
        await ctx.RespondAsync($"# {server.Name} Information\n**Server Name:** {server.Name}\n**Owner:** {server.Owner.Username}\n**Member Count:** {server.MemberCount}");
    }

    [Command("react")]
    [Description("react with an custom emoji to an specific message | Usage: react [messageid] [emojiname]")]
    public async Task ReactToMessageCommand(CommandContext ctx, ulong messageId, string emojiName)
    {
        Console.WriteLine("[i] React Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        if (messageId == 0 || string.IsNullOrWhiteSpace(emojiName))
        {
            await ctx.RespondAsync("please enter an messageId and emoji name\nUsage: react [messageid] [emojiname]");
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, $":{emojiName}:");
        if (emoji == null)
        {
            await ctx.RespondAsync("Invalid emoji name.");
            return;
        }

        var message = await ctx.Channel.GetMessageAsync(messageId);
        await message.CreateReactionAsync(emoji);
        await ctx.RespondAsync($"Reacted with {emojiName}.");
    }

    [Command("myinfo")]
    [Description("Gives some information about myself")]
    public async Task MyInfoCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] myinfo Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        var user = ctx.Client.CurrentUser;
        await ctx.RespondAsync($"# {user.Username}'s Information\n**Username:** {user.Username}\n**ID:** {user.Id}\n**Created At:** {user.CreationTimestamp}\n**Status:** {user.Presence?.Status.ToString() ?? "Offline"}");
    }

    [Command("leave")]
    [Description("LEAVES THE CURRENT SERVER")]
    public async Task LeaveServerCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] leave Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        var guild = ctx.Guild;
        if (guild != null)
        {
            await guild.LeaveAsync();
            await ctx.RespondAsync($"Left the server: {guild.Name}");
        }
        else
        {
            await ctx.RespondAsync("i am currently not in any server.");
        }
    }

    [Command("typing")]
    public async Task TypingCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] Typing Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        await ctx.RespondAsync("I am typing...");
        await ctx.Channel.TriggerTypingAsync();
    }

    // [Command("currently not working | userinfo")]
    // [Description("gets information about an user | Usage: userinfo [username]")]
    // public async Task UserInfoCommand(CommandContext ctx, DiscordMember member = null)
    // {
    //     Console.WriteLine("[i] UserInfo Command was Executed");
    //     await ctx.Channel.TriggerTypingAsync();
    //     if (member == null)
    //     {
    //         await ctx.RespondAsync("please enter an username\nUsage: userinfo [username].");
    //     }
    //     await ctx.RespondAsync($"# {member.Username}'s Information\n**Username:** {member.Username}\n**Nickname:** {member.Nickname}\n**Joined at:** {member.JoinedAt}\n**Status:** {member.Presence?.Status.ToString() ?? "Offline"}");
    // }

    [Command("ping")]
    [Description("shows the latency (for testing purpuses only.)")]
    public async Task PingCommand(CommandContext ctx)
    {
        Console.WriteLine("[i] ping Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        var latency = ctx.Client.Ping;
        await ctx.RespondAsync($"Pong! Latency: {latency}ms");
    }

    [Command("shutdown")]
    [Description("Shutdowns the Bot | Usage: Shutdown [password here]")]
    public async Task Shutdown(CommandContext ctx, int password)
    {
        Console.WriteLine("[i] Exit Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        if (password <= 0)
        {
            await ctx.RespondAsync("please enter the **Correct** password.\nUsage: exit [password here]");
        }
        if (password == 123)
        {
            await ctx.RespondAsync($"Shutdown Started by {ctx.Message.Author.Mention}");
            await ctx.RespondAsync("DeSeBot is Shutting Down in 5 Seconds.");
            Thread.Sleep(5000);
            await ctx.RespondAsync("Shutted Down! (may take up to 5 seconds to discord to realize)");
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }
        else
        {
            await ctx.RespondAsync("Sorry, but the entered password is incorrect!");
        }
    }
    //[Command("killyourself")]
    //[Description("Makes the Bot sad :(")]
    //public async Task killyourself(CommandContext ctx)
    //{
    //    Console.WriteLine("[i] Killyourself command was executed.");
    //    await ctx.TriggerTypingAsync();
    //    await ctx.RespondAsync($"Who tf do you think you are talking to huh {ctx.Message.Author.Mention}");
    //    await ctx.RespondAsync("yeah, thats what i thought ||bit**||");
    //    await ctx.RespondAsync("hahaha");
    //}

    [Command("spam")]
    [Description("spams a custom message | Usage: spam [count] [message]")]
    public async Task SpamCommand(CommandContext ctx, int count, [RemainingText] string message)
    {
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        if (count <= 0 || string.IsNullOrWhiteSpace(message))
        {
            await ctx.RespondAsync("please enter an message count and a message\nUsage: spam [count] [message]");
        }
        if (count > 100)
        {
            await ctx.RespondAsync("Please dont Spam more then 100 Messages.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            await ctx.Channel.SendMessageAsync(message);
            await Task.Delay(100);
        }
    }
    [Command("ownercheck")]
    [Description("Toggles if only the Bot Account can Execute Commands")]
    public async Task ownerchecktoggle(CommandContext ctx)
    {
        ownercheck = !ownercheck;
        await ctx.RespondAsync($"Successfully Toggled Ownercheck: {ownercheck}");
    }

    [Command("clone")]
    [Description("Clones the current server into a new one | Usage .clone [newserverid]")]
    public async Task CloneServer(CommandContext ctx, [RemainingText] ulong targetServerId)
    {
        Console.WriteLine("[i] Clone Command was Executed");
        await ctx.Channel.TriggerTypingAsync();
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        await ServerClone.CloneServer(ctx, targetServerId);
    }
    [Command("afk")]
    [Description("toggles your afk status")]
    public async Task afk(CommandContext ctx)
    {
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        Program.isAFK = !Program.isAFK;
        await ctx.RespondAsync(Program.isAFK ? "I am now AFK" : "I am no longer AFK");
    }
    [Command("RPC")]
    [Description("toggles your RPC")]
    public async Task ToggleRPC(CommandContext ctx)
    {
        if (!IsUserAllowed(ctx))
        {
            await ctx.RespondAsync("You are not authorized to use this command.");
            return;
        }
        Program.RPC = !Program.RPC;
        await ctx.RespondAsync(Program.RPC ? "RPC Toggled on!" : "RPC Toggled off!");
    }
}