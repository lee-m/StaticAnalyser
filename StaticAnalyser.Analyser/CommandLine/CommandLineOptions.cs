﻿namespace StaticAnalysis.CommandLine
{
  /// <summary>
  /// Analysis options passed on the command line.
  /// </summary>
  public class CommandLineOptions
  {
    /// <summary>
    /// Name of the solution file to analyse.
    /// </summary>
    public string SolutionFile
    { get; set; }

    /// <summary>
    /// Whether any generated code should be exluded from analysis.
    /// </summary>
    public bool IgnoreGeneratedCode
    { get; set; }
  }
}