using Battle_Cats_Game_Modder.Mods.PackFileModding;
using Battle_Cats_Game_Modder.Mods.LibnativeModding;
using Battle_Cats_Game_Modder.Mods.GameDataModding;
using System.Net.Cache;
using System.Net;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace Battle_Cats_Game_Modder;

public static class Program
{
    public static string multipleVals = "(You can enter multiple numbers seperated by spaces to edit multiple at once)";
    public static string version = "1.0.0";
    public static async void CheckUpdate()
    {
        HttpClient client = new();
        string url = "https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Game-Modder/master/version.txt";
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url);
        }
        catch
        {
            ColouredText("&An error has occurred when checking for a new update\n", New: ConsoleColor.Red);
            return;
        }
        if (!response.IsSuccessStatusCode)
        {
            ColouredText("&An error has occurred when checking for a new update\n", New: ConsoleColor.Red);
            return;
        }
        response.EnsureSuccessStatusCode();
        response.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
        string responseBody = await response.Content.ReadAsStringAsync();
        responseBody = responseBody.TrimEnd('\n');
        if (responseBody != version)
        {
            ColouredText("&A new version is available from the github\n", New: ConsoleColor.Green);
        }
    }

    public static string MakeRequest(WebRequest request)
    {
        request.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
        HttpRequestCachePolicy noCachePolicy = new(HttpRequestCacheLevel.NoCacheNoStore);
        request.CachePolicy = noCachePolicy;
        WebResponse response = request.GetResponse();
        string result;
        using (Stream dataStream = response.GetResponseStream())
        {
            StreamReader reader = new(dataStream);
            string responseFromServer = reader.ReadToEnd();
            result = responseFromServer;
        }
        return result;
    }
    [STAThread]
    public static void Main()
    {
        Console.WindowWidth = 200;
        Console.WindowHeight = 48;
        CheckUpdate();
        Options();
    }
    public static void Options()
    {
        string[] options =
        {
            "Decrypt .pack files",
            "Encrypt folder to .pack and .list files",
            "Patch libnative-lib.so to remove md5 sum check",
            "Modify game data"
        };
        ColouredText($"&What do you want to do?\n&{CreateOptionsList<string>(options)}");
        int answer = Inputed();
        switch (answer)
        {
            case 1: DecryptPack.Decrypt(); break;
            case 2: EncryptPack.EncryptData(); break;
            case 3: PatchLibnative.MD5Lib(); break;
            case 4: ModifyGameData(); break;
            default: Console.WriteLine("Please enter a recognised number"); break;
        }
        Console.WriteLine("Press enter to continue...");
        Console.ReadLine();
        Options();
    }
    public static int[] ConvertCharArrayToIntArray(char[] input)
    {
        return Array.ConvertAll(input, (char x) => int.Parse(string.Format("{0}", x)));
    }
    public static void Error(string text = "Error, a position couldn't be found, please report this in #bug-reports on discord")
    {
        Console.WriteLine(text + "\nPress enter to continue");
        Console.ReadLine();
        Options();
    }
    public static double ConvertBytesToMegabytes(long bytes)
    {
        return Math.Round(bytes / 1024f / 1024f, 2);
    }
    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
    }
    public static int Inputed()
    {
        int input = 0;
        try
        {
            input = Convert.ToInt32(Console.ReadLine());
        }
        catch (OverflowException)
        {
            ColouredText("Input number was too large\n", ConsoleColor.White, ConsoleColor.DarkRed);
            Options();
        }
        catch (FormatException)
        {
            ColouredText("Input given was not a number or it wasn't an integer\n", ConsoleColor.White, ConsoleColor.DarkRed);
            Options();
        }
        return input;
    }
    public static void ModifyGameData()
    {
        string[] options =
        {
            "Modify unit.csv (cat data)",
            "Modify stage.csv (stage data)",
            "Modify t_unit.csv (enemy data)"
        };
        ColouredText($"&What do you want to do?\n&{CreateOptionsList<string>(options)}");
        int answer = Inputed();
        switch (answer)
        {
            case 1: UnitMod.Unitcsv(); break;
            case 2: StageMod.Stagecsv(); break;
            case 3: EnemyMod.EnemyCSV(); break;
            default: Console.WriteLine("Please enter a recognised number"); break;
        }
    }
    public static string CreateOptionsList<T>(string[] options, T[]? extravalues = null, bool numerical = true, bool skipZero = false, string? first = null, bool color = true)
    {
        string toOutput = "";
        for (int i = 0; i < options.Length; i++)
        {
            if (extravalues != null && skipZero)
            {
                int val = Convert.ToInt32(extravalues[i]);
                if (val == 0)
                {
                    continue;
                }
            }
            if (numerical)
            {
                if (first != null)
                {
                    if (i == 0)
                    {
                        toOutput += $"{i}.& ";
                        toOutput += first;
                        options[i] = "&";
                    }
                    else
                    {
                        toOutput += $"&{i}.& ";
                    }
                }
                else
                {
                    toOutput += $"&{i + 1}.& ";
                }
            }
            toOutput += options[i];
            if (extravalues != null)
            {
                try
                {
                    toOutput += $" &: {extravalues[i]}&";
                }
                catch
                {
                    break;
                }
            }
            toOutput += "\n";

        }
        if (color)
        {
            return toOutput;
        }
        return toOutput.Replace("&", "");
    }
    public static int[] Search(string path, byte[] conditions, bool negative = false, int startpoint = 0, byte[] mult = null, int endpoint = -1, int stop_after = -1)
    {
        byte[] allData = File.ReadAllBytes(path);
        if (mult == null)
        {
            mult = new byte[conditions.Length];
        }
        if (endpoint == -1)
        {
            endpoint = allData.Length - conditions.Length;
        }

        int pos = 0;
        int num = 1;
        List<int> values = new();
        int stop_count = 0;
        if (negative)
        {
            num = -1;
        }
        for (int i = startpoint; i < endpoint; i += num)
        {
            if (stop_after > -1 && stop_count >= stop_after && values.Count > 0)
            {
                break;
            }
            int count = 0;
            for (int j = 0; j < conditions.Length; j++)
            {
                if (negative)
                {
                    try
                    {
                        if (allData[i - j] == conditions[conditions.Length - 1 - j] || mult[conditions.Length - 1 - j] == 1)
                        {
                            if (stop_count > 0)
                            {
                                stop_count = 0;
                            }
                            count++;
                            pos = i;
                        }
                        else
                        {
                            if (count > 0)
                            {
                                stop_count++;
                            }
                            count = 0;
                        }
                    }
                    catch
                    {
                        if (values[0] > 0)
                        {
                            i = allData.Length;
                            break;
                        }
                    }
                }
                else
                {
                    if (allData[i + j] == conditions[j] || mult[j] == 1)
                    {
                        count++;
                        pos = i;
                    }
                    else
                    {
                        if (count > 0)
                        {
                            stop_count++;
                        }
                        count = 0;
                    }
                }
            }
            if (count >= conditions.Length)
            {
                values.Add(pos);
                stop_count = 0;
            }
        }
        if (values.Count == 0)
        {
            values.Add(0);
        }
        return values.ToArray();
    }
    public static void ColouredText(string input, ConsoleColor Base = ConsoleColor.White, ConsoleColor New = ConsoleColor.DarkYellow)
    {
        string[] split = input.Split('&');
        try
        {
            Console.ForegroundColor = New;
            for (int i = 0; i < split.Length; i += 2)
            {
                Console.ForegroundColor = New;
                Console.Write(split[i]);
                Console.ForegroundColor = Base;
                if (i == split.Length - 1)
                {
                    break;
                }
                Console.Write(split[i + 1]);
            }
            Console.ForegroundColor = Base;
        }
        catch (IndexOutOfRangeException)
        {
        }
    }
}
