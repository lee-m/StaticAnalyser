﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.BadPractice
{
  [Export(typeof(AnalysisRuleBase))]
  public class VB6StyleReturnRule : MethodBlockAnalysisRule
  {
    public override void AnalyseMethod(MethodBlockSyntax methodBlock, AnalysisContext context, SemanticModel model)
    {
      //VB6 style returns are implemented via assigning a value to a local
      //variable with the same name as the enclosing function.
      var assignStatements = methodBlock.DescendantNodes().OfType<AssignmentStatementSyntax>();

      foreach (AssignmentStatementSyntax assignStatement in assignStatements)
      {
        SymbolInfo lhsSymbol = model.GetSymbolInfo(assignStatement.Left);

        if (lhsSymbol.Symbol != null
           && lhsSymbol.Symbol.Kind == SymbolKind.Local
           && lhsSymbol.Symbol.Name == methodBlock.Begin.Identifier.Text)
        {
          context.AnalysisResults.AddWarning(assignStatement.GetLocation(),
                                     "Avoid use of VB6 style return statements.");
        }
      }
    }
  }
}