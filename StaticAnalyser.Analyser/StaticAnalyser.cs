using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.CommandLine;
using StaticAnalysis.Analysis;

using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StaticAnalysis
{
  /// <summary>
  /// The static analyser which handles the analysis.
  /// </summary>
  public class StaticAnalyser
  {
    /// <summary>
    /// Parsed command line options.
    /// </summary>
    private CommandLineOptions mOptions;

    /// <summary>
    /// Output for recording any diagostics.
    /// </summary>
    private TextWriter mDiagnosticsWriter;

    /// <summary>
    /// Collection of rules to run.
    /// </summary>
    private AnalysisRuleCollection mRules;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public StaticAnalyser()
    {
      mRules = new AnalysisRuleCollection();
    }

    /// <summary>
    /// Runs the analysis using the specified command line options.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="outputWriter">Output writer to record any diagnosics</param>
    public void RunAnalysis(string[] args,
                            TextWriter outputWriter)
    {
      
      mDiagnosticsWriter = outputWriter;
      mOptions = StaticAnalysis.CommandLine.CommandLineParser.ParseOptions(args, outputWriter);

      if (mOptions == null)
        return;

      Stopwatch analysisTimer = Stopwatch.StartNew();

      //Load the solution and then iterate over each project and source file to run
      //the analysis rules on.
      var workspace = MSBuildWorkspace.Create();
      var solution = workspace.OpenSolutionAsync(mOptions.SolutionFile).Result;

      foreach (var project in solution.Projects)
      {
        VisualBasicCompilation compilation = (VisualBasicCompilation)project.GetCompilationAsync().Result;

        foreach (var tree in compilation.SyntaxTrees)
        {
          CompilationUnitSyntax root = (CompilationUnitSyntax)tree.GetRoot();
          mRules.ExecuteRules(root, new AnalysisContext(mOptions, mDiagnosticsWriter, project));
        }
      }

      analysisTimer.Stop();
      mDiagnosticsWriter.WriteLine("Analysis completed in {0}", analysisTimer.Elapsed.ToString());
    }
  }
}