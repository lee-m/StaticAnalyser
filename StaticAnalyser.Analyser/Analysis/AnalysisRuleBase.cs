using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.Threading.Tasks;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Base for all analysis rules.
  /// </summary>
  public abstract class AnalysisRuleBase
  {
    /// <summary>
    /// Executes this rule on a single compilation unit.
    /// </summary
    /// <param name="compilationUnit">The compilation unit to analyse.</param>
    public virtual async Task ExecuteRuleAsync(AnalysisContext context)
    {
      foreach (var tree in context.CurrentCompilation.SyntaxTrees)
      {
        SemanticModel model = context.CurrentCompilation.GetSemanticModel(tree);
        AnalyseCompilationUnit((CompilationUnitSyntax)await tree.GetRootAsync(), model, context);
      }
    }

    protected abstract void AnalyseCompilationUnit(CompilationUnitSyntax compilationUnit, 
                                                   SemanticModel model,
                                                   AnalysisContext context);
  }
}