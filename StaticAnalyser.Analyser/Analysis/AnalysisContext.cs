using Microsoft.CodeAnalysis;

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
    private readonly AnalysisOptions mOptions;

    /// <summary>
    /// Set of warnings found
    /// </summary>
    private readonly AnalysisResults mResults;

    /// <summary>
    /// Semantic model for the current source file being analysed.
    /// </summary>
    private readonly Compilation mCompilation;

    /// <summary>
    /// Initialise a new context instance.
    /// </summary>
    /// <param name="options">Parsed command line options for the program.</param>
    /// <param name="results">Collection of analysis warnings.</param>
    /// <param name="compilation">The current compilation instance.</param>
    public AnalysisContext(AnalysisOptions options,
                           AnalysisResults results,
                           Compilation compilation)
    {
      mOptions = options;
      mResults = results;
      mCompilation = compilation;
    }

    /// <summary>
    /// Accessor for the command line options.
    /// </summary>
    public AnalysisOptions Options
    {
      get { return mOptions; }
    }

    /// <summary>
    /// Set of warnings found during analysis.
    /// </summary>
    public AnalysisResults Results
    {
      get { return mResults; }
    }

    /// <summary>
    /// Accessor for the current semantic model.
    /// </summary>
    public Compilation CurrentCompilation
    {
      get { return mCompilation; }
    }
  }
}