using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

public class ServerClone
{
    [Command("clone")]
    [Description("Clones the current server into another specified server, including roles, channels, and their permissions. Deletes all existing roles, categories, and channels in the target server first.")]
    public static async Task CloneServer(CommandContext ctx, ulong targetServerId)
    {
        try
        {
            var sourceGuild = ctx.Guild;
            if (sourceGuild == null)
            {
                await ctx.RespondAsync("This command can only be used in a server.");
                return;
            }

            var targetGuild = ctx.Client.Guilds.ContainsKey(targetServerId) ? ctx.Client.Guilds[targetServerId] : null;
            if (targetGuild == null)
            {
                await ctx.RespondAsync("Target server not found. Make sure the bot is a member of the target server.");
                return;
            }

            var botHighestRole = targetGuild.CurrentMember.Roles.OrderByDescending(r => r.Position).FirstOrDefault();
            if (botHighestRole == null)
            {
                await ctx.RespondAsync("Bot does not have any roles in the target server.");
                return;
            }

            await ctx.RespondAsync($"Preparing to clone server '{sourceGuild.Name}' into '{targetGuild.Name}'...");

            foreach (var channel in targetGuild.Channels.ToList())
            {
                if (channel.Type != ChannelType.Category && channel.Type != ChannelType.Text && channel.Type != ChannelType.Voice)
                    continue;

                try
                {
                    await channel.DeleteAsync("Preparing for server clone");
                }
                catch (Exception channelDeleteEx)
                {
                    await ctx.RespondAsync($"Error deleting channel {channel.Name}: {channelDeleteEx.Message}");
                }
            }

            foreach (var category in targetGuild.Channels.Where(c => c.Type == ChannelType.Category).ToList())
            {
                try
                {
                    await category.DeleteAsync("Preparing for server clone");
                }
                catch (Exception categoryDeleteEx)
                {
                    await ctx.RespondAsync($"Error deleting category {category.Name}: {categoryDeleteEx.Message}");
                }
            }

            foreach (var role in targetGuild.Roles
                .Where(r => !r.IsManaged &&
                            r.Id != targetGuild.Id &&
                            r.Position < botHighestRole.Position)
                .OrderByDescending(r => r.Position)
                .ToList())
            {
                try
                {
                    await targetGuild.DeleteRoleAsync(role, "Preparing for server clone");
                }
                catch (Exception roleDeleteEx)
                {
                    await ctx.RespondAsync($"Error deleting role {role.Name}: {roleDeleteEx.Message}");
                }
            }

            var roleMapping = new Dictionary<ulong, DiscordRole>();
            foreach (var role in sourceGuild.Roles.Where(r => !r.IsManaged && r.Id != sourceGuild.Id).OrderBy(r => r.Position))
            {
                var newRole = await targetGuild.CreateRoleAsync(
                    role.Name,
                    role.Permissions,
                    role.Color,
                    role.IsHoisted,
                    role.IsMentionable
                );
                roleMapping[role.Id] = newRole;
            }

            var categoryMapping = new Dictionary<ulong, DiscordChannel>();
            foreach (var category in sourceGuild.Channels.Where(c => c.Type == ChannelType.Category).OrderBy(c => c.Position))
            {
                var newCategory = await targetGuild.CreateChannelAsync(category.Name, ChannelType.Category);

                foreach (var overwrite in category.PermissionOverwrites)
                {
                    if (roleMapping.TryGetValue(overwrite.Id, out DiscordRole mappedRole))
                    {
                        await newCategory.AddOverwriteAsync(mappedRole, overwrite.Allow, overwrite.Deny);
                    }
                }

                categoryMapping[category.Id] = newCategory;
            }

            foreach (var channel in sourceGuild.Channels.OrderBy(c => c.Position))
            {
                DiscordChannel newParentChannel = null;
                if (channel.ParentId.HasValue)
                {
                    categoryMapping.TryGetValue(channel.ParentId.Value, out newParentChannel);
                }

                DiscordChannel newChannel = null;
                switch (channel.Type)
                {
                    case ChannelType.Text:
                        newChannel = await targetGuild.CreateChannelAsync(channel.Name, ChannelType.Text, newParentChannel);
                        break;
                    case ChannelType.Voice:
                        newChannel = await targetGuild.CreateChannelAsync(channel.Name, ChannelType.Voice, newParentChannel);
                        break;
                    default:
                        continue;
                }

                foreach (var overwrite in channel.PermissionOverwrites)
                {
                    if (roleMapping.TryGetValue(overwrite.Id, out DiscordRole mappedRole))
                    {
                        await newChannel.AddOverwriteAsync(mappedRole, overwrite.Allow, overwrite.Deny);
                    }
                }
            }

            await ctx.RespondAsync($"Server '{sourceGuild.Name}' successfully cloned into '{targetGuild.Name}'!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
            await ctx.RespondAsync($"An error occurred while cloning the server: {ex.Message}");
        }
    }
}