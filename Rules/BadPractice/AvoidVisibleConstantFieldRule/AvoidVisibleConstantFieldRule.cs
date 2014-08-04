using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.BadPractice
{
  [Export(typeof(AnalysisRuleBase))]
  public class AvoidVisibleConstantFieldRule : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      foreach (FieldDeclarationSyntax fieldDecl in node.DescendantNodes().OfType<FieldDeclarationSyntax>())
      {
        var modifierKinds = fieldDecl.Modifiers.Select(r => r.VisualBasicKind());

        if (modifierKinds.Contains(SyntaxKind.ConstKeyword)
           && modifierKinds.Contains(SyntaxKind.PublicKeyword))
        {
          foreach (VariableDeclaratorSyntax varDecl in fieldDecl.Declarators)
          {
            context.Results.AddWarning(varDecl.GetLocation(),
                                       "'Public Const' field '{0}' in type '{1}' should be declared as 'Shared ReadOnly' instead.",
                                       varDecl.Names.First(),
                                       node.Begin.Identifier.Text);
          }
        }
      }
    }
  }
}