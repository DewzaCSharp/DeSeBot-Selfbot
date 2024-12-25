using DSharpPlus.CommandsNext;
using DSharpPlus;
using SimpleDiscordBot;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class Start
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

    public static async Task Main()
    {
        IntPtr hWnd = GetConsoleWindow();
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetLayeredWindowAttributes(hWnd, 0, 205, LWA_ALPHA);

        Console.Title = "DeSeBot Selfbot | V1.0.0 | Made by DewzaCSharp";
        Console.SetWindowSize(94, 20);
        Console.SetBufferSize(94, Console.BufferHeight);

        string text1 = "Checking Token and Stuff..";
        string text2 = "Done!";
        string text3 = "Connecting..";


        if (File.Exists("skipanim"))
        {
            await DisplayTextWithBlinkingDot("Connecting...", 30, clearAfter: false);
            await Program.RealMain();
        }
        else
        {
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
            await DisplayTextWithBlinkingDot(text1, 30, clearAfter: true);
            await DisplayTextWithBlinkingDot(text2, 30, clearAfter: true);

            await DisplayTextWithBlinkingDot(text3, 30, clearAfter: false);

            await Program.RealMain();
        }
    }

    private static async Task DisplayTextWithBlinkingDot(string text, int delay, bool clearAfter)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            await Task.Delay(delay);
        }

        var cancellationTokenSource = new CancellationTokenSource();
        var blinkTask = BlinkDotAsync(cancellationTokenSource.Token);

        await Task.Delay(2000);
        cancellationTokenSource.Cancel();
        await blinkTask;

        if (clearAfter)
        {
            for (int i = text.Length; i >= 0; i--)
            {
                Console.SetCursorPosition(i, Console.CursorTop);
                Console.Write(' ');
                await Task.Delay(delay);
            }
        }
    }

    private static async Task BlinkDotAsync(CancellationToken token)
    {
        bool showDot = true;
        int dotPosition = Console.CursorLeft;

        while (!token.IsCancellationRequested)
        {
            Console.SetCursorPosition(dotPosition, Console.CursorTop);
            Console.Write(showDot ? "." : " ");
            showDot = !showDot;

            await Task.Delay(500);
        }

        Console.SetCursorPosition(dotPosition, Console.CursorTop);
        Console.Write(" ");
    }
}