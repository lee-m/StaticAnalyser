using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on class/structure declarations.
  /// </summary>
  public abstract class TypeStatementAnalysisRule : AnalysisRuleBase
  {
    /// <summary>
    /// Syntax walker which only visits type statements.
    /// </summary>
    private class TypeStatementSyntaxWalker : VisualBasicSyntaxWalker
    {
      /// <summary>
      /// The analysis rule to invoke for each type statement.
      /// </summary>
      private TypeStatementAnalysisRule mRule;

      public TypeStatementSyntaxWalker(TypeStatementAnalysisRule rule)
        : base(SyntaxWalkerDepth.Node)
      {
        mRule = rule;
      }

      /// <summary>
      /// Invokes the bound rule for a class statement.
      /// </summary>
      /// <param name="node">The method to analyse.</param>
      public override void VisitClassStatement(ClassStatementSyntax node)
      {
        mRule.AnalyseClassStatement(node);
      }

      /// <summary>
      /// Invokes the bound rule for a structure statement.
      /// </summary>
      /// <param name="node">The method to analyse.</param>
      public override void VisitStructureStatement(StructureStatementSyntax node)
      {
        mRule.AnalyseStructureStatement(node);
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected override VisualBasicSyntaxWalker CreateSyntaxWalker()
    {
      return new TypeStatementSyntaxWalker(this);
    }

    /// <summary>
    /// Analyses a class statement.
    /// </summary>
    /// <param name="node">The class statement to analyse</param>
    public abstract void AnalyseClassStatement(ClassStatementSyntax node);

    /// <summary>
    /// Analyse a structure statement.
    /// </summary>
    /// <param name="node">The structure statement to analyse.</param>
    public abstract void AnalyseStructureStatement(StructureStatementSyntax node);
  }
}
