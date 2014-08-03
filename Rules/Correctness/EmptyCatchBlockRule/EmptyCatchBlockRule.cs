using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Correctness
{
  [Export(typeof(AnalysisRuleBase))]
  public class EmptyCatchBlockRule : MethodBlockAnalysisRule
  {
    public override void AnalyseMethod(MethodBlockSyntax methodBlock,
                                       AnalysisContext context,
                                       SemanticModel model)
    {
      var catchBlocks = methodBlock.DescendantNodes().OfType<CatchPartSyntax>();

      foreach (var catchBlock in catchBlocks)
      {
        if (!catchBlock.Statements.Any())
          context.Results.AddWarning(catchBlock.GetLocation(), "Remove empty catch block.");
      }
    }
  }
}