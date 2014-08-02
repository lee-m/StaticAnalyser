using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.CommandLine;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task RunAnalysisAsync(string[] args, TextWriter outputWriter)
    {
      
      mDiagnosticsWriter = outputWriter;
      mOptions = StaticAnalysis.CommandLine.CommandLineParser.ParseOptions(args, outputWriter);

      if (mOptions == null)
        return;

      Stopwatch analysisTimer = Stopwatch.StartNew();

      var workspace = MSBuildWorkspace.Create();
      var solution = workspace.OpenSolutionAsync(mOptions.SolutionFile).Result;
      await Task.WhenAll(solution.Projects.Select(proj => AnalyseProjectAsync(proj)));

      analysisTimer.Stop();
      mDiagnosticsWriter.WriteLine("Analysis completed in {0}", analysisTimer.Elapsed.ToString());
    }

    /// <summary>
    /// Asynchronously analyses a single project.
    /// </summary>
    /// <param name="project">The project to analyse.</param>
    /// <returns></returns>
    private async Task AnalyseProjectAsync(Project project)
    {
      VisualBasicCompilation compilation = (VisualBasicCompilation) await project.GetCompilationAsync();

      foreach (var tree in compilation.SyntaxTrees)
      {
        CompilationUnitSyntax root = (CompilationUnitSyntax) await tree.GetRootAsync();
        SemanticModel model = compilation.GetSemanticModel(tree);

        mRules.ExecuteRules(root, new AnalysisContext(mOptions, mDiagnosticsWriter, model));
      }
    }
  }
}