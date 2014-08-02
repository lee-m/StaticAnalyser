using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on method declarations.
  /// </summary>
  public abstract class MethodBlockAnalysisRule : AnalysisRuleBase
  {
    /// <summary>
    /// Syntax walker which only visits method blocks.
    /// </summary>
    private class MethodBlockSyntaxWalker : TypedAnalysisSyntaxWalker<MethodBlockAnalysisRule>
    {
      /// <summary>
      /// Initialise a new instance bound to the specified rule.
      /// </summary>
      /// <param name="rule">The rule to invoke for each method found.</param>
      public MethodBlockSyntaxWalker(MethodBlockAnalysisRule rule, AnalysisContext context)
        : base(rule, context)
      { }

      /// <summary>
      /// Invokes the bound rule for a method statement.
      /// </summary>
      /// <param name="node">The method to analyse.</param>
      public override void VisitMethodBlock(MethodBlockSyntax node)
      {
        Rule.AnalyseMethod(node, Context, CurrentSemanticModel);
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected override AnalysisSyntaxWalker CreateSyntaxWalker(AnalysisContext context)
    {
      return new MethodBlockSyntaxWalker(this, context);
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
