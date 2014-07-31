using System;
using System.ComponentModel.Composition;

using StaticAnalysis.Analysis;

using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Rules.EmptyCatchBlockRule
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyCatchBlockRule : MethodStatementAnalysisRule
  {
    public override void AnalyseMethodStatement(MethodStatementSyntax methodStatement)
    {
      throw new NotImplementedException();
    }
  }
}
