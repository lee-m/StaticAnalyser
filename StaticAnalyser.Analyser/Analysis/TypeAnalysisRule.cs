using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on class/structure declarations.
  /// </summary>
  public abstract class TypeAnalysisRule : AnalysisRuleBase
  {
    /// <summary>
    /// Syntax walker which only visits type statements.
    /// </summary>
    private class TypeStatementSyntaxWalker : VisualBasicSyntaxWalker
    {
      /// <summary>
      /// The analysis rule to invoke for each type statement.
      /// </summary>
      private TypeAnalysisRule mRule;

      public TypeStatementSyntaxWalker(TypeAnalysisRule rule)
        : base(SyntaxWalkerDepth.Node)
      {
        mRule = rule;
      }

      /// <summary>
      /// Invokes the bound rule for a class declaration.
      /// </summary>
      /// <param name="node">The class to analyse.</param>
      public override void VisitClassBlock(ClassBlockSyntax node)
      {
        //TODO: ignore compiler generated classes
        mRule.AnalyseClassDeclaration(node);
        DefaultVisit(node);
      }

      /// <summary>
      /// Invokes the bound rule for a structure declaration.
      /// </summary>
      /// <param name="node">The structure declaration to analyse.</param>
      public override void VisitStructureBlock(StructureBlockSyntax node)
      {
        //TODO: ignore compiler generated classes
        mRule.AnalyseStructureDeclaration(node);
        DefaultVisit(node);
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
    /// <param name="node">The class declaration to analyse</param>
    public abstract void AnalyseClassDeclaration(ClassBlockSyntax node);

    /// <summary>
    /// Analyse a structure statement.
    /// </summary>
    /// <param name="node">The structure declaration to analyse.</param>
    public abstract void AnalyseStructureDeclaration(StructureBlockSyntax node);
  }
}
