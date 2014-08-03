using System.IO;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

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
      AnalysisSyntaxWalker walker = CreateSyntaxWalker(context);
      
      foreach (var tree in context.CurrentCompilation.SyntaxTrees)
      {
        SemanticModel model = context.CurrentCompilation.GetSemanticModel(tree);
        walker.WalkSyntaxTree(await tree.GetRootAsync(), model);
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected abstract AnalysisSyntaxWalker CreateSyntaxWalker(AnalysisContext context);
  }
}