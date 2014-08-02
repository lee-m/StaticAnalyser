using System;
using System.Linq;
using System.ComponentModel.Composition;

using StaticAnalysis.Analysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Rules.Performance
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyFinaliserRule : MethodBlockAnalysisRule
  {
    public override void AnalyseMethod(MethodBlockSyntax methodBlock)
    {
      MethodStatementSyntax methodStatement = methodBlock.Begin;

      //Only interested in subs
      if (methodBlock.VisualBasicKind() != SyntaxKind.SubBlock)
        return;

      //Not interested if the method is not called the right thing
      if (methodStatement.Identifier.Text != "Finalize")
        return;

      //Check for Protected Overrides
      SyntaxTokenList modifiers = methodStatement.Modifiers;

      if (modifiers.Count != 2)
        return;

      if (modifiers[0].VisualBasicKind() != SyntaxKind.ProtectedKeyword
         || modifiers[1].VisualBasicKind() != SyntaxKind.OverridesKeyword)
        return;

      //This is a finaliser method so check that it does something other than call MyBase.Finalize()
      bool isEmptyFinaliser = false;

      if (!methodBlock.Statements.Any())
        isEmptyFinaliser = true;
      else if(methodBlock.Statements.Count == 1)
      {
        //See if the only statement is a call to the base finaliser
        var stmt = methodBlock.Statements.First();
        var memberAccessExpression = stmt.DescendantNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

        if(memberAccessExpression == null)
          return;

        isEmptyFinaliser = (memberAccessExpression.Expression.VisualBasicKind() == SyntaxKind.MyBaseExpression
                            && memberAccessExpression.Name.Identifier.Text == "Finalize");
      }

      if(isEmptyFinaliser)
        ReportDiagnostic(methodStatement.GetLocation(), "Remove empty finaliser.");
    }
  }
}
