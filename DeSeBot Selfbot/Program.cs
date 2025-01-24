using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SimpleDiscordBot
{
    public class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_LAYERED = 0x80000;
        private const uint LWA_ALPHA = 0x2;

        public static bool isAFK = false;
        public static bool RPC = false;
        public static async Task RealMain()
        {
            string text2 = "Connecting...";

            IntPtr hWnd = GetConsoleWindow();
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0, 205, LWA_ALPHA);

            config botConfig;

            if (File.Exists("config.json"))
            {
                string json = File.ReadAllText("config.json");
                botConfig = System.Text.Json.JsonSerializer.Deserialize<config>(json);
            }
            else
            {
                Console.WriteLine("No config.json file found!");
                return;
            }

            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = botConfig.Token,
                TokenType = DSharpPlus.TokenType.User,
                AutoReconnect = true
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = botConfig.Prefix
            });

            if (RPC)
            {
                discord.Ready += async e =>
                {
                    Game game = new Game(botConfig.RPCText, ActivityType.Watching);
                    DiscordGame dsharpgame = new DiscordGame(game.Name);
                    await discord.UpdateStatusAsync(dsharpgame, user_status: DSharpPlus.Entities.UserStatus.Idle, idle_since: null);
                };
            }
            MyCommands cmds = new MyCommands();
            discord.MessageCreated += async e =>
            {
                if (isAFK && e.Message.Author.Id != discord.CurrentUser.Id)
                {
                    await e.Message.RespondAsync("I'm currently AFK!\nI'll try my best to respond as soon as possible.");
                }
            };
            commands.RegisterCommands<MyCommands>();
            await discord.ConnectAsync();
            DateTime startTime = DateTime.Now;
            for (int i = text2.Length - 1; i >= 0; i--)
            {
                if (Console.CursorLeft > 0)
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
                Thread.Sleep(30);
            }
            Console.Clear();
            

            Console.WriteLine("""
                                ██████╗░███████╗░██████╗███████╗██████╗░░█████╗░████████╗
                                ██╔══██╗██╔════╝██╔════╝██╔════╝██╔══██╗██╔══██╗╚══██╔══╝
                                ██║░░██║█████╗░░╚█████╗░█████╗░░██████╦╝██║░░██║░░░██║░░░
                                ██║░░██║██╔══╝░░░╚═══██╗██╔══╝░░██╔══██╗██║░░██║░░░██║░░░
                                ██████╔╝███████╗██████╔╝███████╗██████╦╝╚█████╔╝░░░██║░░░
                                ╚═════╝░╚══════╝╚═════╝░╚══════╝╚═════╝░░╚════╝░░░░╚═╝░░░
                """);
            Thread.Sleep(100);
            await animate("\t\t\t\t     DeSeBot SelfBot", 7);
            Thread.Sleep(100);
            await animate($"\t\t\t\t   [i] Version: '1.0.0'", 7);
            Thread.Sleep(100);
            await animate($"\t\t\t\t   [i] Bot prefix: '{botConfig.Prefix}'", 7);
            Thread.Sleep(100);
            await animate("\t\t\t\t   [i] Made by DewzaCSharp", 7);
            Thread.Sleep(100);
            Console.WriteLine("\t\t\t\t[ ------------------------- ]");
            Thread.Sleep(100);
            await animate($"\t\t\t\t   [i] logged in as: '{discord.CurrentUser.Username}'", 7);
            Thread.Sleep(100);
            await animate($"\t\t\t\t   [i] RPC Text: '{botConfig.RPCText}'", 7);
            Thread.Sleep(100);
            await animate("\t\t\t\t   [i] Bot Online!", 7);
            Thread.Sleep(100);
            Console.WriteLine("\t\t\t\t[ ------------------------- ]");
            Thread.Sleep(100);
            await animate("[i] Logs here...", 40);
            Thread.Sleep(500);

            await Task.Delay(-1);
        }
        public static async Task animate(string text, int delay)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                await Task.Delay(delay);
            }
            Console.WriteLine();
        }
    }

    public class config
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string RPCText { get; set; }
    }
}
