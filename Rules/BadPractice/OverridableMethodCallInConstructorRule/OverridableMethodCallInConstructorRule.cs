using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.BadPractice
{
  [Export(typeof(AnalysisRuleBase))]
  public class OverridableMethodCallInConstructorRule : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      foreach (ConstructorBlockSyntax constructor in node.DescendantNodes().OfType<ConstructorBlockSyntax>())
      {
        if (context.Options.IgnoreGeneratedCode
           && AnalysisUtils.HasGeneratedCodeAttribute(constructor.Begin.AttributeLists, model))
          continue;

        foreach (InvocationExpressionSyntax invokeStmt in constructor.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
          SymbolInfo callSymbol = model.GetSymbolInfo(invokeStmt.Expression);

          if (callSymbol.Symbol != null
             && callSymbol.Symbol.Kind == SymbolKind.Method
             && callSymbol.Symbol.IsVirtual)
          {
            context.Results.AddWarning(invokeStmt.GetLocation(),
                                       "Avoid calling Overridable method '{0}' within constructor of type '{1}'",
                                       callSymbol.Symbol.Name,
                                       node.Begin.Identifier.Text);
          }
        }
      }
    }
  }
}