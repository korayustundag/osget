using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace osget
{
    internal static class App
    {
        internal static List<string> OsIds = new List<string>();
        internal static List<string> OsNames = new List<string>();
        internal static List<string> OsFileSize = new List<string>();
        internal static List<string> OsDownloadUrl = new List<string>();

        internal static void Exit(int exitCode)
        {
            Console.Write("Press enter to exit...");
            Console.ReadLine();
            Environment.Exit(exitCode);
        }

        internal static void ShowHelp()
        {
            Console.WriteLine("Argument List:");
            Console.WriteLine("  -h or --help    :: Showing Help");
            Console.WriteLine("  -v or --version :: Showing App Version Information");
            Console.WriteLine("  -i or --install :: Install Command");
            Console.WriteLine("  -l or --list    :: Showing Installable Operating Systems");
            Console.WriteLine("=====");
            Console.WriteLine("Usage:");
            if (OperatingSystem.IsWindows())
            {
                Console.WriteLine("osget.exe [osname] [path]");
                Console.WriteLine("Example 1: osget.exe -i debian C:\\folder\\debian.iso");
                Console.WriteLine("Example 2: osget.exe --install debian \"C:\\my folder\\debian.iso\"");
                Console.WriteLine("Example 3: osget.exe -i debian debian.iso");
            }
            else
            {
                Console.WriteLine("./osget [osname] [path]");
                Console.WriteLine("Example 1: ./osget -i debian /home/user/folder/debian.iso");
                Console.WriteLine("Example 2: ./osget --install debian \"/home/user/my folder/debian.iso\"");
                Console.WriteLine("Example 3: ./osget -i debian debian.iso");
            }
            Console.WriteLine("=====");
            Environment.Exit(0);
        }

        internal static void ShowVersion()
        {
            Console.WriteLine("Welcome to OsGet");
            Console.WriteLine("Version  : 1.0.0");
            Console.WriteLine("Source   : https://github.com/korayustundag/osget");
            Environment.Exit(0);
        }

        internal static void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        internal static void CheckNetworkConnection()
        {
            try
            {
                Ping ping = new Ping();
                PingReply result = ping.Send("www.githubusercontent.com");
                if (result.Status != IPStatus.Success)
                {
                    ShowError("Error: Could not connect to server! Please check your internet connection.");
                    Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while connecting to the server:: {ex.Message}");
                Exit(1);
            }
        }

    }
}
