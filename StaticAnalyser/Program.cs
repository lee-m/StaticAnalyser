using StaticAnalysis.Analysis;
using StaticAnalysis.CommandLine;

using System;
using System.Diagnostics;

namespace StaticAnalysis
{
  /// <summary>
  /// Main executable for the application.
  /// </summary>
  internal class Program
  {
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private static void Main(string[] args)
    {
      Stopwatch analysisTimer = Stopwatch.StartNew();
      AnalysisOptions options = CommandLineParser.ParseOptions(args, Console.Out);

      if (options == null)
        return;

      StaticAnalyser analyser = new StaticAnalyser();
      AnalysisResults results = analyser.RunAnalysisAsync(options).Result;

      analysisTimer.Stop();
      results.OutputResults(Console.Out);
      Console.WriteLine("Analysis completed in {0}", analysisTimer.Elapsed.ToString());
    }
  }
}