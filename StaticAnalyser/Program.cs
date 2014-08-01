using StaticAnalysis.Analysis;
using System;

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
      StaticAnalyser analyser = new StaticAnalyser();
      analyser.RunAnalysisAsync(args, Console.Out).Wait();
    }
  }
}