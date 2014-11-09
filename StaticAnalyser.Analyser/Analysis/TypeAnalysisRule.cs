using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis.Utils;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Analysis rule which operates on class/structure declarations.
  /// </summary>
  public abstract class TypeAnalysisRule : AnalysisRuleBase
  {
    /// <summary>
    /// Determines whether a given type declaration should be ignored for analysis purposes or not.
    /// </summary>
    /// <param name="attributes">Set of attributes applied to the type.</param>
    /// <returns></returns>
    private bool IgnoreTypeDeclaration(SyntaxList<AttributeListSyntax> attributes,
                                       AnalysisContext context,
                                       SemanticModel model)
    {
      return context.Options.IgnoreGeneratedCode
             ? AnalysisUtils.HasGeneratedCodeAttribute(attributes, model)
             : false;
    }

    protected override async Task AnalyseCompilationUnitAsync(CompilationUnitSyntax compilationUnit,
                                                              SemanticModel model,
                                                              AnalysisContext context)
    {
      var types = compilationUnit.DescendantNodes().OfType<TypeBlockSyntax>();

      foreach (var type in types)
      {
        if(!IgnoreTypeDeclaration(type.Begin.AttributeLists, context, model))
          await AnalyseTypeDeclarationAsync(type, context, model);
      }
    }

    /// <summary>
    /// Analyses a class, structure or interface declaration.
    /// </summary>
    /// <param name="node">The type declaration to analyse</param>
    /// <param name="context">Analysis context.</param>
    /// <param name="model">Semantic model for the compilation unit.</param>
    /// <remarks>Default to calling the synchronous implementation.</remarks>
    public virtual async Task AnalyseTypeDeclarationAsync(TypeBlockSyntax node,
                                                     AnalysisContext context,
                                                     SemanticModel model)
    {
      await Task.Run(() => AnalyseTypeDeclaration(node, context, model));
    }

    /// <summary>
    /// Analyses a class, structure or interface declaration.
    /// </summary>
    /// <param name="node">The type declaration to analyse</param>
    /// <param name="context">Analysis context.</param>
    /// <param name="model">Semantic model for the compilation unit.</param>
    public virtual void AnalyseTypeDeclaration(TypeBlockSyntax node,
                                                AnalysisContext context,
                                                SemanticModel model)
    {
      //Either this or the async variant should be overriden.
      throw new InvalidOperationException();
    }
  }
}