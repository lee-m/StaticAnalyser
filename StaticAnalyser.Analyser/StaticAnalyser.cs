using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;

using StaticAnalysis.Analysis;

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
    /// <param name="options">Analysis options for this run.</param>
    public async Task<AnalysisResults> RunAnalysisAsync(AnalysisOptions options)
    {
      AnalysisResults results = new AnalysisResults();
      MSBuildWorkspace workspace = MSBuildWorkspace.Create();
      Solution solution = await workspace.OpenSolutionAsync(options.SolutionFile);

      await Task.WhenAll(solution.Projects.Select(proj => AnalyseProjectAsync(solution, proj, results, options)));
      return results;
    }

    /// <summary>
    /// Asynchronously analyses a single project.
    /// </summary>
    /// <param name="solution">The solution loaded.</param>
    /// <param name="project">The project to analyse.</param>
    /// <param name="results">Holds the set of analysis warnings produced.</param>
    /// <param name="options">Analysis options.</param>
    /// <returns></returns>
    private async Task AnalyseProjectAsync(Solution solution, Project project, AnalysisResults results, AnalysisOptions options)
    {
      VisualBasicCompilation compilation = (VisualBasicCompilation)await project.GetCompilationAsync();
      await mRules.ExecuteRulesAsync(new AnalysisContext(options, results, compilation, solution, project));
    }
  }
}