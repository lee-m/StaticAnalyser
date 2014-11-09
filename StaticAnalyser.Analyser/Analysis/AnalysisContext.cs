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
    /// The current solution loaded.
    /// </summary>
    private readonly Solution mSolution;

    /// <summary>
    /// The project being analysed.
    /// </summary>
    private readonly Project mProject;

    /// <summary>
    /// Initialise a new context instance.
    /// </summary>
    /// <param name="options">Parsed command line options for the program.</param>
    /// <param name="results">Collection of analysis warnings.</param>
    /// <param name="compilation">The current compilation instance.</param>
    public AnalysisContext(AnalysisOptions options,
                           AnalysisResults results,
                           Compilation compilation,
                           Solution solution,
                           Project project)
    {
      mOptions = options;
      mResults = results;
      mCompilation = compilation;
      mSolution = solution;
      mProject = project;
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
    public AnalysisResults AnalysisResults
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

    /// <summary>
    /// Accessor for the solution
    /// </summary>
    public Solution Solution
    {
      get { return mSolution; }
    }

    /// <summary>
    /// Accessor for the project being analysed.
    /// </summary>
    public Project Project
    {
      get { return mProject; }
    }
  }
}