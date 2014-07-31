using System;
using System.Linq;
using System.ComponentModel.Composition;

using StaticAnalysis.Analysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Rules.EmptyCatchBlockRule
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyCatchBlockRule : MethodStatementAnalysisRule
  {
    public override void AnalyseMethodStatement(MethodBlockSyntax methodStatement)
    {
      var catchBlocks = methodStatement.DescendantNodes().OfType<CatchStatementSyntax>();

      foreach (var catchBlock in catchBlocks)
      {
        //TODO:
      }
    }
  }
}
