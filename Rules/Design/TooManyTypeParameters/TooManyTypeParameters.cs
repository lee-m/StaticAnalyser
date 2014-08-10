﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

namespace StaticAnalysis.Rules.Design
{
  public class TooManyTypeParameters : TypeAnalysisRule
  {
    private const int MaxTypeParamters = 2;

    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
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