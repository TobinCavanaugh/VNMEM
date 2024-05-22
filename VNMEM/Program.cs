namespace VNMEM;

public static class Program
{
    public static bool running;


    private static void DrawUI(string[] lines)
    {
        Console.Clear();

        for (int i = 0; i < lines.Length; i++)
        {
            //This is janky af but functional
            var ls = i.ToString("".PadRight(lines.Length.ToString().Length, '0'));
            if (i == vars["PC"])
            {
                Console.WriteLine($">{ls}.  {lines[i]}");
            }
            else
            {
                Console.WriteLine($" {ls}.  {lines[i]}");
            }
        }

        Console.WriteLine("---");

        foreach (var v in vars)
        {
            Console.WriteLine($"{v.Key}: {v.Value}");
        }
    }


    public static Dictionary<string, int> vars = new()
    {
        { "PC", 0 },
        { "X", 5 },
        { "Y", 2 },
        { "Z", 0 },
        { "mem", 0 },
    };

    // private static Dictionary<string, Action<string>> commands = new()
    // {
        // { "HLT", (_) => { running = false; } },
        // { ""}
    // }

    static void Main(string[] args)
    {
        string program = @"//Our z is our output value
LOD #1
STO Z

//If our exponent is 0, the result is one
LOD X 
JMZ #25


//z *= y
LOD Y
MUL Z
STO Z

//X -= 1, if x == 0 -> jump, else -> loop
LOD X
SUB #1
STO X
JMZ #22
JMP #10

//Regular program exit
HLT //22

//Exponent of 0 exit
HLT
";

        program += "\n";
        program = program.Replace("\r\n", "\n");

        var lines = program.Split("\n");


        while (true)
        {
            DrawUI(lines);
            Console.ReadLine();


            var current = lines[vars["PC"]];

            current = current.Replace(" ", "");

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
                vars["PC"] = v;
                continue;
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