using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Design
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyInterfaceRule : TypeAnalysisRule
  {
    public override void AnalyseInterfaceDeclaration(TypeBlockSyntax node, 
                                                     AnalysisContext context, 
                                                     SemanticModel model)
    {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)model.GetDeclaredSymbol(node);

      if (!typeSymbol.MemberNames.Any())
        context.AnalysisResults.AddWarning(node.GetLocation(),
                                           "Interface '{0}' has no members.",
                                           node.Begin.Identifier.Text);
    }
  }
}
