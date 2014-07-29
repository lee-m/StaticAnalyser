using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.CommandLine;

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

      //var workspace = MSBuildWorkspace.Create();
      //var solution = workspace.OpenSolutionAsync(mOptions.SolutionFile).Result;

      //foreach (var project in solution.Projects)
      //{
      //  VisualBasicCompilation compilation = (VisualBasicCompilation)project.GetCompilationAsync().Result;

      //  foreach (var tree in compilation.SyntaxTrees)
      //  {
      //    CompilationUnitSyntax root = (CompilationUnitSyntax)tree.GetRoot();
      //    var types = root.DescendantNodes().OfType<TypeStatementSyntax>().ToList();
      //    int a = 1;
      //  }
      //}
    }
  }
}