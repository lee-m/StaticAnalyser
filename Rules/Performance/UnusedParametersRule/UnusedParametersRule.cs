using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Performance
{
  [Export(typeof(AnalysisRuleBase))]
  public class UnusedParametersRule : MethodBlockAnalysisRule
  {
    public override void AnalyseMethod(MethodBlockSyntax methodBlock, AnalysisContext context, SemanticModel model)
    {
      //Ignore partial methods as they don't have method bodies, we'll pick up the
      //atual body later on. Also ignore any method declared Overridable or Overrides
      HashSet<SyntaxKind> ignoredModifiers;
      ignoredModifiers = new HashSet<SyntaxKind>(new SyntaxKind[]{SyntaxKind.OverridableKeyword,
                                                                  SyntaxKind.OverridesKeyword,
                                                                  SyntaxKind.PartialKeyword});
      if (methodBlock.Begin.Modifiers.Any(modifier => ignoredModifiers.Contains(modifier.VisualBasicKind())))
        return;

      //Ignore P/Invoke methods
      if (AnalysisUtils.HasDllImportAttribte(methodBlock.Begin.AttributeLists, model))
        return;

      IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodBlock);

      //It's not unusual for parameters on an event handler to be unused so build up a list
      //of parameters of any events handled by this method so we don't warn about them.
      HashSet<string> eventParamNames = new HashSet<string>();

      if (methodBlock.Begin.HandlesClause != null)
      {
        foreach (HandlesClauseItemSyntax handlesCause in methodBlock.Begin.HandlesClause.Events)
        {
          var sym = model.GetSymbolInfo(handlesCause.EventMember);

          if (sym.Symbol != null)
          {
            var members = ((IEventSymbol)sym.Symbol).Type.GetMembers(WellKnownMemberNames.DelegateInvokeName);

            if (members.Length == 1)
            {
              foreach (IParameterSymbol param in ((IMethodSymbol)members.First()).Parameters)
                eventParamNames.Add(param.Name);
            }
          }
        }
      }

      //Build up a list of parameters for this method and keep track of whether they've
      //been referenced or not. Skip any parameter which comes from any handled event.
      Dictionary<IParameterSymbol, bool> parameters = new Dictionary<IParameterSymbol, bool>();

      foreach (IParameterSymbol param in methodSymbol.Parameters)
      {
        if (!eventParamNames.Contains(param.Name))
          parameters.Add(param, false);
      }

      //Get any identifiers used within this method and see if they resolve back to any parameter
      var identifiers = methodBlock.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();

      foreach (IdentifierNameSyntax identifier in identifiers)
      {
        SymbolInfo identifierSymbol = model.GetSymbolInfo(identifier);
        IParameterSymbol paramSymbol = identifierSymbol.Symbol as IParameterSymbol;

        if (paramSymbol != null
            && paramSymbol.Kind == SymbolKind.Parameter)
          parameters[paramSymbol] = true;
      }

      //Warn for any unused parameters
      var unusedParams = parameters.Where(param => !param.Value);

      foreach (var unusedParam in unusedParams)
      {
        context.AnalysisResults.AddWarning(methodBlock.GetLocation(),
                                           "Parameter '{0}' to method '{1}' is never referenced.",
                                           unusedParam.Key.Name, methodSymbol.Name);
      }
    }
  }
}