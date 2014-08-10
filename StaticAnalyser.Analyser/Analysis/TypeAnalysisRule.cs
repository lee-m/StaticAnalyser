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

      /// <summary>
      /// Invokes the bound rule for an interface declaration.
      /// </summary>
      /// <param name="node">The interface declaration to analyse.</param>
      public override void VisitInterfaceBlock(InterfaceBlockSyntax node)
      {
        VisitType(node);
      }

      /// <summary>
      /// Optionally executes the rule on a class or structure declaration.
      /// </summary>
      /// <param name="node">The type to analyse.</param>
      private void VisitType(TypeBlockSyntax node)
      {
        if (!IgnoreTypeDeclaration(node.Begin.AttributeLists))
          Rule.AnalyseTypeDeclaration(node, Context, CurrentSemanticModel);

        DefaultVisit(node);
      }

      /// <summary>
      /// Determines whether a given type declaration should be ignored for analysis purposes or not.
      /// </summary>
      /// <param name="attributes">Set of attributes applied to the type.</param>
      /// <returns></returns>
      private bool IgnoreTypeDeclaration(SyntaxList<AttributeListSyntax> attributes)
      {
        return Context.Options.IgnoreGeneratedCode
               ? AnalysisUtils.HasGeneratedCodeAttribute(attributes, CurrentSemanticModel)
               : false;
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
    /// Analyses a class, structure or interface declaration.
    /// </summary>
    /// <param name="node">The type declaration to analyse</param>
    public abstract void AnalyseTypeDeclaration(TypeBlockSyntax node,
                                               AnalysisContext context,
                                               SemanticModel model);
  }
}