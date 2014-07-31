using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

using StaticAnalysis.CommandLine;

using System.IO;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Holds contextual information used by each analysis rule.
  /// </summary>
  public class AnalysisContext
  {
    /// <summary>
    /// Command line options.
    /// </summary>
    private readonly CommandLineOptions mOptions;

    /// <summary>
    /// Output writer to report any diagnostics.
    /// </summary>
    private readonly TextWriter mAnalysisOutputWriter;

    /// <summary>
    /// The project being analysed.
    /// </summary>
    private readonly Project mProject;

    /// <summary>
    /// Initialise a new context instance.
    /// </summary>
    /// <param name="options">Parsed command line options for the program.</param>
    /// <param name="analysisOutputWriter">Output writer to report any diagnostics.</param>
    /// <param name="project">The project being analysed.</param>
    public AnalysisContext(CommandLineOptions options,
                           TextWriter analysisOutputWriter,
                           Project project)
    {
      mOptions = options;
      mAnalysisOutputWriter = analysisOutputWriter;
      mProject = project;
    }

    /// <summary>
    /// Accessor for the command line options.
    /// </summary>
    public CommandLineOptions Options
    {
      get { return mOptions; }
    }

    /// <summary>
    /// Accessor for the analysis output writer.
    /// </summary>
    public TextWriter AnalysisOutputWriter
    {
      get { return mAnalysisOutputWriter; }
    }
  }
}