namespace VNMEM;

public static class ParseHelpers
{
    public static string Until(this string str, string delim)
    {
        for (int i = 0; i < str.Length - delim.Length; i++)
        {
            if (str.Substring(i, delim.Length) == delim)
            {
                return str.Substring(0, i);
            }
        }

        return str;
    }

    public static string GetVar(string line, string instruction)
    {
        var v = line.Until("//").Replace(instruction, "");
        return v;
    }


    public static int Realize(this string varname)
    {
        if (varname.StartsWith("#"))
        {
            return Int32.Parse(varname.Replace("#", ""));
        }

        return Program.vars[varname.ToUpper()];
    }
}