using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Performance
{
  [Export(typeof(AnalysisRuleBase))]
  public class UncalledPrivateCodeRule : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      //List of private methods/properties and whether they have been called or not
      Dictionary<IMethodSymbol, bool> privateMethodMapping = new Dictionary<IMethodSymbol,bool>();
      Dictionary<IPropertySymbol, bool> privatePropMapping = new Dictionary<IPropertySymbol,bool>();

      //Get a list of private methods within this type
      var privateMethods = node.DescendantNodes().OfType<MethodBlockSyntax>().Where(method => SyntaxTokenListContainsPrivateKeyword(method.Begin.Modifiers));

      foreach (var privateMethod in privateMethods)
      {
        if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(privateMethod.Begin.AttributeLists, model))
          continue;

        var sym = model.GetDeclaredSymbol(privateMethod);
        privateMethodMapping.Add(sym, false);
      }
        
      //Get details of the private properties. Need to look at property statements to find auto 
      //implemented and traditional property declarations
      var props = node.DescendantNodes().OfType<PropertyStatementSyntax>();
      props = props.Where(prop => SyntaxTokenListContainsPrivateKeyword(prop.Modifiers));

      foreach (var property in props)
      {
        if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(property.AttributeLists, model))
          continue;

        privatePropMapping.Add((IPropertySymbol)model.GetDeclaredSymbol(property), false);
      }

      //Find all invokcation statements contained within this type and determine if they call any private method
      foreach(var invokeExpr in node.DescendantNodes().OfType<InvocationExpressionSyntax>())
      {
        var calledMethod = model.GetSymbolInfo(invokeExpr.Expression);
        ISymbol symbol = null;

        if (calledMethod.Symbol != null)
          symbol = calledMethod.Symbol;
        else if (calledMethod.CandidateSymbols.Length == 1)
          symbol = calledMethod.CandidateSymbols[0];

        if(symbol != null
           && symbol.Kind == SymbolKind.Method)
        {
          IMethodSymbol methodSym = (IMethodSymbol)symbol;

          if(methodSym.DeclaredAccessibility == Accessibility.Private)
            privateMethodMapping[methodSym] = true;
        }
      }

      //Find all references to symbols within this type and see if they refer to a private property
      foreach(var identifierNode in node.DescendantNodes().OfType<IdentifierNameSyntax>())
      {
        SymbolInfo identifierSymbol = model.GetSymbolInfo(identifierNode);
        IPropertySymbol propSymbol = identifierSymbol.Symbol as IPropertySymbol;

        if (propSymbol != null
           && privatePropMapping.ContainsKey(propSymbol))
          privatePropMapping[propSymbol] = true;
      }

      //Check for any private methods which have their address taken
      foreach(var unaryExpr in node.DescendantNodes().OfType<UnaryExpressionSyntax>())
      {
        //Only interested in AddressOf expressions
        if (unaryExpr.OperatorToken.VisualBasicKind() != SyntaxKind.AddressOfKeyword)
          continue;

        SymbolInfo delegateSym = model.GetSymbolInfo(unaryExpr.Operand);
        IMethodSymbol methodSym = delegateSym.Symbol as IMethodSymbol;

        if (methodSym != null
           && privateMethodMapping.ContainsKey(methodSym))
          privateMethodMapping[methodSym] = true;
      }

      //Issue warnings for any uncalled private methods
      foreach(var privateMethodItem in privateMethodMapping)
      {
        if(!privateMethodItem.Value)
          context.AnalysisResults.AddWarning(privateMethodItem.Key.Locations.First(), 
                                             "Private method '{0}' is never referenced.", 
                                             privateMethodItem.Key.Name);
      }

      //Issue warnings for any uncalled private properties
      foreach (var privatePropItem in privatePropMapping)
      {
        if (!privatePropItem.Value)
          context.AnalysisResults.AddWarning(privatePropItem.Key.Locations.First(),
                                             "Private property '{0}' is never referenced.",
                                             privatePropItem.Key.Name);
      }
    }

    private bool SyntaxTokenListContainsPrivateKeyword(SyntaxTokenList list)
    {
      foreach (SyntaxToken tok in list)
      {
        if (tok.VisualBasicKind() == SyntaxKind.PrivateKeyword)
          return true;
      }

      return false;
    }
  }
}
