using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis.Utils;

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
    private class TypeStatementSyntaxWalker : TypedAnalysisSyntaxWalker<TypeAnalysisRule>
    {
      public TypeStatementSyntaxWalker(TypeAnalysisRule rule, AnalysisContext context)
        : base(rule, context)
      { }

      /// <summary>
      /// Invokes the bound rule for a class declaration.
      /// </summary>
      /// <param name="node">The class to analyse.</param>
      public override void VisitClassBlock(ClassBlockSyntax node)
      {
        VisitType(node);
      }

      /// <summary>
      /// Invokes the bound rule for a structure declaration.
      /// </summary>
      /// <param name="node">The structure declaration to analyse.</param>
      public override void VisitStructureBlock(StructureBlockSyntax node)
      {
        VisitType(node);
      }

      private void VisitType(TypeBlockSyntax node)
      {
        //Ignore compiler generated classes if needed
        bool ignoreType = Context.Options.IgnoreGeneratedCode
                          ? AnalysisUtils.HasGeneratedCodeAttribute(node.Begin.AttributeLists, CurrentSemanticModel)
                          : false;

        if (!ignoreType)
          Rule.AnalyseTypeDeclaration(node, Context, CurrentSemanticModel);

        DefaultVisit(node);
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected override AnalysisSyntaxWalker CreateSyntaxWalker(AnalysisContext context)
    {
      return new TypeStatementSyntaxWalker(this, context);
    }

    /// <summary>
    /// Analyses a class statement.
    /// </summary>
    /// <param name="node">The type declaration to analyse</param>
    public abstract void AnalyseTypeDeclaration(TypeBlockSyntax node,
                                                AnalysisContext context,
                                                SemanticModel model);
  }
}