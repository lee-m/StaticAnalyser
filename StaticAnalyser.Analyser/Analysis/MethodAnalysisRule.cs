using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis.Utils;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on method declarations.
  /// </summary>
  public abstract class MethodBlockAnalysisRule : AnalysisRuleBase
  {
    protected override async Task AnalyseCompilationUnitAsync(CompilationUnitSyntax compilationUnit,
                                                              SemanticModel model,
                                                              AnalysisContext context)
    {
      var methods = compilationUnit.DescendantNodes().OfType<MethodBlockSyntax>();

      foreach (var method in methods)
      {
        if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(method.Begin.AttributeLists, model))
          continue;

        await AnalyseMethodAsync(method, context, model);
      }
    }

    /// <summary>
    /// Analyses a method statement asynchronously.
    /// </summary>
    /// <param name="node">The method to analyse.</param>
    public virtual async Task AnalyseMethodAsync(MethodBlockSyntax methodBlock,
                                           AnalysisContext context,
                                           SemanticModel model)
    {
      await Task.Run(() => AnalyseMethod(methodBlock, context, model));
    }

    /// <summary>
    /// Analyses a method statement asynchronously.
    /// </summary>
    /// <param name="node">The method to analyse.</param>
    public virtual void AnalyseMethod(MethodBlockSyntax methodBlock,
                                      AnalysisContext context,
                                      SemanticModel model)
    {
      //Either this or the async variant should be overriden.
      throw new InvalidOperationException();
    }
  }
}