using System.IO;

namespace StaticAnalysis.CommandLine
{
  /// <summary>
  /// Command line option parser.
  /// </summary>
  public static class CommandLineParser
  {
    /// <summary>
    /// Parses a set of command line options.
    /// </summary>
    /// <param name="args">Arguments to parse.</param>
    /// <param name="outputWriter">Output writer to record any diagnosics to.</param>
    /// <returns>The parsed command line options.</returns>
    public static CommandLineOptions ParseOptions(string[] args, TextWriter outputWriter)
    {
      if(args.Length == 0)
      {
        PrintUsage(outputWriter);
        return null;
      }

      string solutionFile = null;
      bool ignoreGeneratedCode = false;

      foreach(string arg in args)
      {
        if (arg.StartsWith("/"))
        {
          //Decode the option
          string argValue = arg.Substring(1);

          if (argValue == "igc")
            ignoreGeneratedCode = true;
          else
            outputWriter.WriteLine("Unrecognised option '{0}'", argValue);
        }
        else
          solutionFile = arg;
      }

      if(string.IsNullOrEmpty(solutionFile))
      {
        outputWriter.WriteLine("No solution specified.");
        return null;
      }

      return new CommandLineOptions
                 {
                   SolutionFile = solutionFile,
                   IgnoreGeneratedCode = ignoreGeneratedCode
                 };
    }

    private static void PrintUsage(TextWriter outputWriter)
    {
      outputWriter.WriteLine("Usage: StaticAnalyser <options> <solution file>");
      outputWriter.WriteLine();
      outputWriter.WriteLine("Available options:");
      outputWriter.WriteLine("/igc\t\tIgnores any generated code during analysis.");
    }
  }
}