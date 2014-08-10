using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Design
{
  public class TooManyTypeParameters : TypeAnalysisRule
  {
    private const int MaxTypeParamters = 2;

    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      AnalyseType(node, context, model);
    }

    public override void AnalyseInterfaceDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      AnalyseType(node, context, model);
    }

    //TODO: combine this
    private void AnalyseType(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)model.GetDeclaredSymbol(node);

      if (typeSymbol.TypeParameters.Length > MaxTypeParamters)
        context.AnalysisResults.AddWarning(node.GetLocation(),
                                           "Avoid excessive type parameters to {0} '{1}'.",
                                           typeSymbol.TypeKind == TypeKind.Interface ? "interface" : "type",
                                           node.Begin.Identifier.Text);
    }
  }
}
