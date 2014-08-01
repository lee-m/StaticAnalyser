﻿using System;
using System.Linq;
using System.ComponentModel.Composition;

using StaticAnalysis.Analysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Rules.EmptyCatchBlockRule
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyCatchBlockRule : MethodBlockAnalysisRule
  {
    public override void AnalyseMethod(MethodBlockSyntax methodBlock)
    {
      var catchBlocks = methodBlock.DescendantNodes().OfType<CatchPartSyntax>();

      foreach (var catchBlock in catchBlocks)
      {
        if (!catchBlock.Statements.Any())
          ReportDiagnostic(catchBlock.GetLocation(), "Remove empty catch block.");
      }
    }
  }
}
