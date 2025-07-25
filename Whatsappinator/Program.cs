using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using JetBrains.Annotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Whatsappinator;

[UsedImplicitly]
public class Program
{
    public static System.Drawing.Color WhatsAppColor { get; } = System.Drawing.Color.FromArgb(alpha: 200,
        red: 42,
        green: 181,
        blue: 64);
    public static string CurrentDirectory => Directory.GetCurrentDirectory();
    
    private static int Main(string[] args)
    {
        TryPlayWhatsappSound();

        var filePath = args.ElementAtOrDefault(0);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("No file specified, either pass a file path as an argument or drag an image file onto the executable.");
            return SayGoodBye(exitCode:1);
        }
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File {filePath} does not exist.");
            return SayGoodBye(exitCode:1);
        }

        try
        {
            ProcessImage(filePath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Why so whatsapp??");
            Console.ResetColor();
            return SayGoodBye(exitCode: 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error whatsappinating image: {ex.Message}");
            Console.WriteLine("Press X to rethrow the exception or any other key to exit.");
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.X)
            {
                throw; // Rethrow the exception for debugging purposes
            }
            return 1;
        }
    }

    private static void TryPlayWhatsappSound()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Whatsappinator.Assets.whatsapp.wav";
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var player = new SoundPlayer(stream);
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Failed to play the WhatsApp sound. Continuing without sound. :,(");
        }
    }

    private static void ProcessImage(string filePath)
    {
        var oldFileName = Path.GetFileNameWithoutExtension(filePath);
        var fileExt = Path.GetExtension(filePath);
        var newFileName = $"{oldFileName}_whatsappinated{fileExt}";
        var outputPath = Path.Combine(CurrentDirectory, newFileName);
        
        using var image = Image.Load<Rgba32>(filePath);
        var colorMatrix = ColorMatrix.Identity with
        {
            M11 = WhatsAppColor.R / 255f,
            M22 = WhatsAppColor.G / 255f,
            M33 = WhatsAppColor.B / 255f,
            M44 = 1
        };
        image.Mutate(ctx =>
        {
            ctx.BackgroundColor(Color.Transparent); // Reset alpha blending
            ctx.Filter(colorMatrix);
        });
        
        image.Save(outputPath);
    }

    private static int SayGoodBye(int exitCode = 0)
    {
        Console.WriteLine("Press any key to exit");
        Console.ReadKey(intercept: true);
        return exitCode;
    }
}