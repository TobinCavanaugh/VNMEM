using System.Text;

namespace VNMEM;

public static class Program
{
    private static (int w, int h) window = new(0, 0);

    private static void DrawUI(string[] lines)
    {
        var w = Console.WindowWidth - 3;
        Console.SetCursorPosition(0, 0);

        //Clear the console if the window has been resized
        (int w, int h) size = (Console.WindowWidth, Console.WindowHeight);
        if (size != window)
        {
            Console.Clear();
            window = size;
        }

        //We padright everything in order to clear the full line as we print it.
        //This is speed ideal (as microsoft is unable to clear a buffer in under half a second)
        //but not memory ideal
        StringBuilder display = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            //This is janky af but functional
            var ls = i.ToString("".PadRight(lines.Length.ToString().Length, '0'));
            if (i == vars["PC"])
            {
                display.Append("> ");
            }
            else
            {
                display.Append(" ");
            }

            display.AppendLine($"{ls}.  {lines[i]}".PadRight(w));
        }

        display.AppendLine();
        display.AppendLine("Registers:".PadRight(w));

        foreach (var v in vars)
        {
            display.AppendLine($"\t{v.Key}: {v.Value}".PadRight(w));
        }

        Console.Write(display);
    }


    public static Dictionary<string, int> vars = new()
    {
        { "PC", 0 },
        { "mem", 0 },
        { "X", 5 },
        { "Y", 2 },
        { "Z", 0 },
        { "T0", 0 },
        { "T1", 0 },
    };

    // private static Dictionary<string, Action<string>> commands = new()
    // {
    // { "HLT", (_) => { running = false; } },
    // { ""}
    // }


    static void Main(string[] args)
    {
        Entry:

        if (args.Length == 0)
        {
            args = new string[] { "" };
        }

        string targetPath = args[0];

        string program = Resources.defaultProgram;
        if (File.Exists(targetPath))
        {
            program = File.ReadAllText(targetPath);
        }
        else
        {
            Console.WriteLine(
                $"Failed to load program... \"{targetPath}\" does not exist. Submit 'R' to try again, or press enter to load the default program.");

            while (true)
            {
                string s = Console.ReadLine().ToLower().Replace(" ", "");
                if (s == "r")
                {
                    goto Entry;
                }

                if (s == "")
                {
                    break;
                }
            }
        }

        program += "\n";
        program = program.Replace("\r\n", "\n");

        var lines = program.Split("\n");

        while (true)
        {
            DrawUI(lines);
            Console.ReadLine();

            var current = lines[vars["PC"]];

            current = current.Replace(" ", "");

            //I considered making this more interesting via a dictionary of strings and actions but
            //now that I think about it, thats stupid, slow, more memory consumption, less readable,
            //less debuggable, more complex, uses more language features. REMEMBER: CLARITY != VERBOSITY

            if (current.StartsWith("#"))
            {
                goto NextLine;
            }

            if (current.StartsWith("LOD"))
            {
                var v = ParseHelpers.GetVar(current, "LOD");
                vars["mem"] = v.Realize();
                goto NextLine;
            }

            if (current.StartsWith("STO"))
            {
                var v = ParseHelpers.GetVar(current, "STO");
                vars[v] = vars["mem"];
                goto NextLine;
            }

            if (current.StartsWith("ADD"))
            {
                var v = ParseHelpers.GetVar(current, "ADD").Realize();
                vars["mem"] += v;
                goto NextLine;
            }

            if (current.StartsWith("SUB"))
            {
                var v = ParseHelpers.GetVar(current, "SUB").Realize();
                vars["mem"] -= v;
                goto NextLine;
            }

            if (current.StartsWith("MUL"))
            {
                var v = ParseHelpers.GetVar(current, "MUL").Realize();
                vars["mem"] *= v;
                goto NextLine;
            }

            if (current.StartsWith("DIV"))
            {
                var v = ParseHelpers.GetVar(current, "DIV").Realize();
                vars["mem"] /= v;
                goto NextLine;
            }

            if (current.StartsWith("JMP"))
            {
                var v = ParseHelpers.GetVar(current, "JMP").Realize();
                vars["PC"] = v - 1;
                goto NextLine;
            }

            if (current.StartsWith("JMZ"))
            {
                var v = ParseHelpers.GetVar(current, "JMZ").Realize();

                if (vars["mem"] == 0)
                {
                    vars["PC"] = v;
                    continue;
                }

                goto NextLine;
            }

            if (current.StartsWith("HLT"))
            {
                break;
            }

            NextLine:
            vars["PC"]++;
        }

        DrawUI(lines);

        Console.WriteLine("PROGRAM ENDED");

        Console.WriteLine("Enter Y to exit");

        while (true)
        {
            var exit = Console.ReadLine();
            if (exit.ToLower().Replace(" ", "") == "y")
            {
                return;
            }
        }
    }
}