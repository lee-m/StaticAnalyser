using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis.Utils;

using System.Linq;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on method declarations.
  /// </summary>
  public abstract class MethodBlockAnalysisRule : AnalysisRuleBase
  {
    protected override void AnalyseCompilationUnit(CompilationUnitSyntax compilationUnit,
                                                   SemanticModel model,
                                                   AnalysisContext context)
    {
      var methods = compilationUnit.DescendantNodes().OfType<MethodBlockSyntax>();

      foreach (var method in methods)
      {
        if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(method.Begin.AttributeLists, model))
          continue;

        AnalyseMethod(method, context, model);
      }
    }

    /// <summary>
    /// Analyses a method statement.
    /// </summary>
    /// <param name="node">The method to analyse.</param>
    public abstract void AnalyseMethod(MethodBlockSyntax methodBlock,
                                       AnalysisContext context,
                                       SemanticModel model);
  }
}