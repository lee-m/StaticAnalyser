using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Design
{
  [Export(typeof(AnalysisRuleBase))]
  public class AbstractTypeWithPublicCtor : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)model.GetDeclaredSymbol(node);

      //Only interested in abstract types with at least one constructor
      if (!typeSymbol.IsAbstract)
        return;

      if (!typeSymbol.Constructors.Any())
        return;

      foreach (IMethodSymbol construtorSymbol in typeSymbol.Constructors)
      {
        if (construtorSymbol.DeclaredAccessibility == Accessibility.Public)
        {
          //No point in warning multiple times
          context.AnalysisResults.AddWarning(node.GetLocation(),
                                             "Avoid 'Public' constructors on 'MustInherit' type '{0}'.",
                                             node.Begin.Identifier.Text);
          return;
        }
      }
    }
  }
}