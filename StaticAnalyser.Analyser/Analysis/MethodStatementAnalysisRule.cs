using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on method declarations.
  /// </summary>
  public abstract class MethodStatementAnalysisRule : AnalysisRuleBase
  {
    /// <summary>
    /// Syntax walker which only visits method statements.
    /// </summary>
    private class MethodStatementSyntaxWalker : VisualBasicSyntaxWalker
    {
      /// <summary>
      /// The analysis rule to invoke for each method statement.
      /// </summary>
      private MethodStatementAnalysisRule mRule;

      /// <summary>
      /// Initialise a new instance bound to the specified rule.
      /// </summary>
      /// <param name="rule">The rule to invoke for each method found.</param>
      public MethodStatementSyntaxWalker(MethodStatementAnalysisRule rule)
        : base(SyntaxWalkerDepth.Node)
      {
        mRule = rule;
      }

      /// <summary>
      /// Invokes the bound rule for a method statement.
      /// </summary>
      /// <param name="node">The method to analyse.</param>
      public override void VisitMethodStatement(MethodStatementSyntax node)
      {
        mRule.AnalyseMethodStatement(node);
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected override VisualBasicSyntaxWalker CreateSyntaxWalker()
    {
      return new MethodStatementSyntaxWalker(this);
    }

    /// <summary>
    /// Analyses a method statement.
    /// </summary>
    /// <param name="node">The method to analyse.</param>
    public abstract void AnalyseMethodStatement(MethodStatementSyntax methodStatement);
  }
}
