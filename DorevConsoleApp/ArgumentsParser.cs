using System.Diagnostics;
using DorevLibrary;

namespace DorevConsoleApp;

public static class ArgumentsParser
{
    public static Arguments GetArguments(string[] args)
    {
        if(args.Length < 1)
            throw new ArgumentException("Not enough arguments.");

        if(args.Length == 1)
            if (args[0] == "-a" || args[0] == "-b" || args[0] == "-e")
                throw new ArgumentException("Option requires an argument.");
            else
                return new Arguments(args[0]);

        if(args.Length >= 2)
            if (args[0] == "-a" || args[0] == "-b" || args[0] == "-e")
            {
                var option = args[0] switch
                {
                    "-a" => Options.MatchAnywhere,
                    "-b" => Options.MatchBegin,
                    "-e" => Options.MatchEnd,
                    _ => Options.MatchBegin
                };
                return new Arguments(args[1], option);
            }
            else if(args[0][0] == '-')
                throw new ArgumentException($"Illegal option: {args[0]}");
            else
                return new Arguments(args[0]);

        throw new UnreachableException();
    }
}
