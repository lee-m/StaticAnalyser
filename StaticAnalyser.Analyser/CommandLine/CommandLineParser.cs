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
      return new CommandLineOptions
                 {
                   SolutionFile = args[0]
                 };
    }
  }
}