using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace osget
{
    internal class Program
    {
        public static string inputOsName { get; set; }
        public static string inputSavePath { get; set; }

        const string githubUrl = "https://raw.githubusercontent.com/korayustundag/osget/main/list/oslist.txt";

        static async Task Main(string[] args)
        {
            Console.Title = "OsGet";
            if (!args.Any())
            {
                App.ShowError("Argument Error! Use \"--help\" command");
                App.Exit(1);
            }
            App.CheckNetworkConnection();

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(githubUrl))
                {
                    using (HttpContent content = response.Content)
                    {
                        string fullList = await content.ReadAsStringAsync();
                        string[] lines = fullList.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string line in lines)
                        {
                            string[] data = line.Split(',');
                            App.OsIds.Add(data[0]);
                            App.OsNames.Add(data[1]);
                            App.OsFileSize.Add(data[2]);
                            App.OsDownloadUrl.Add(data[3]);
                        }
                    }
                }
            }

            string argument = args[0];
            if (string.Equals(argument, "-h") || string.Equals(argument, "--help"))
            {
                App.ShowHelp();
            }
            else if (string.Equals(argument, "-v") || string.Equals(argument, "--version"))
            {
                App.ShowVersion();
            }
            else if (string.Equals(argument, "-l") || string.Equals(argument, "--list"))
            {
                Console.WriteLine("===OS LIST=======");
                for (int i = 0; i < App.OsNames.Count(); i++)
                {
                    Console.WriteLine();
                    string name = "----" + App.OsNames[i] + "----";
                    Console.WriteLine(name);
                    Console.WriteLine("Name: " + App.OsIds[i]);
                    Console.WriteLine("File Size: " + App.OsFileSize[i]);
                    for (int z = 0; z < name.Length; z++)
                    {
                        Console.Write("-");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("=================");
                App.Exit(0);
            }
            else if (string.Equals(argument, "-i") || string.Equals(argument, "--install") || string.Equals(argument, "install"))
            {
                if (args.Count() > 1)
                {
                    inputOsName = args[1];
                    if (args.Count() > 2)
                    {
                        inputSavePath = args[2];
                    }
                    else
                    {
                        App.ShowError("Argument Error: Path");
                        App.Exit(1);
                    }
                }
                else
                {
                    App.ShowError("Argument Error: Os Name");
                    App.Exit(1);
                }
            }
            else
            {
                App.ShowError("Error: Unknown Command => " + argument);
                App.Exit(1);
            }

            Console.WriteLine("Welcome to OsGet");
            

            bool isMatch = false;
            int count = 0;

            foreach (string osid in App.OsIds)
            {
                if (osid == inputOsName)
                {
                    isMatch = true;
                    break;
                }
                count++;
            }

            if (!isMatch)
            {
                App.ShowError("Error: Invalid OS Name => "+inputOsName);
                App.ShowError("Use \"-l\" or \"--list\" command");
                App.Exit(1);
            }

            Console.WriteLine();
            Console.WriteLine("===OS INFO=======");
            Console.WriteLine("Selected OS:" + App.OsNames[count]);
            Console.WriteLine("Total File Size:" + App.OsFileSize[count]);
            Console.WriteLine("=================");
            Console.WriteLine();
            try
            {

            }
            catch (Exception ex)
            {
                App.ShowError("Error: " + ex.Message);
                throw;
            }
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(App.OsDownloadUrl[count], HttpCompletionOption.ResponseHeadersRead);
                var contentLength = response.Content.Headers.ContentLength ?? -1L;
                var buffer = new byte[81920];
                var bytesRead = 0L;
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = File.Create(inputSavePath))
                {
                    var read = 0;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        bytesRead += read;
                        var progress = (int)(100 * bytesRead / contentLength);
                        Console.Write($"\r[{new string('#', progress / 2)}{new string('-', 50 - progress / 2)}] {progress}%");
                    }
                }
            }
            Console.WriteLine(" Download completed!");
            App.Exit(0);
        }
    }
}