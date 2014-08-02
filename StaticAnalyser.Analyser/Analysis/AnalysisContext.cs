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
    /// Semantic model for the current source file being analysed.
    /// </summary>
    private readonly SemanticModel mSemanticModel;

    /// <summary>
    /// Initialise a new context instance.
    /// </summary>
    /// <param name="options">Parsed command line options for the program.</param>
    /// <param name="analysisOutputWriter">Output writer to report any diagnostics.</param>
    /// <param name="model">The semantic model for the source file being analysed.</param>
    public AnalysisContext(CommandLineOptions options,
                           TextWriter analysisOutputWriter,
                           SemanticModel model)
    {
      mOptions = options;
      mAnalysisOutputWriter = analysisOutputWriter;
      mSemanticModel = model;
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

    /// <summary>
    /// Accessor for the current semantic model.
    /// </summary>
    public SemanticModel Model
    {
      get { return mSemanticModel; }
    }
  }
}