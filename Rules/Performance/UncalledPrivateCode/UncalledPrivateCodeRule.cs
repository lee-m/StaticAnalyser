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
using System.Threading.Tasks;

namespace StaticAnalysis.Rules.Performance
{
  [Export(typeof(AnalysisRuleBase))]
  public class UncalledPrivateCodeRule : TypeAnalysisRule
  {
    public override async Task AnalyseTypeDeclarationAsync(TypeBlockSyntax node,
                                                           AnalysisContext context,
                                                           SemanticModel model)
    {
      //For partial types, get the set of documents this type appears in for looking up any references to handle the case
      //of a private method only being called in one of the partial classes
      var privateMethods = node.DescendantNodes().OfType<MethodBlockSyntax>().Where(method => SyntaxTokenListContainsPrivateKeyword(method.Begin.Modifiers));
      var typeSymbol = model.GetDeclaredSymbol(node);
      var documents = typeSymbol.Locations.Select(loc => context.Project.GetDocument(loc.SourceTree)).ToImmutableHashSet();
      
      foreach (var privateMethod in privateMethods)
        await CheckForUncalledPrivateMethodAsync(privateMethod, context, model, documents);

      //Check for any uncalled private properties
      var props = node.DescendantNodes().OfType<PropertyStatementSyntax>();
      props = props.Where(prop => SyntaxTokenListContainsPrivateKeyword(prop.Modifiers));

      foreach (var property in props)
        await CheckForUncalledPrivatePropertyAsync(property, context, model, documents);
    }

    private async Task CheckForUncalledPrivateMethodAsync(MethodBlockSyntax privateMethod, 
                                                          AnalysisContext context, 
                                                          SemanticModel model,
                                                          ImmutableHashSet<Document> sourceFiles)
    {
      if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(privateMethod.Begin.AttributeLists, model))
        return;

      var sym = model.GetDeclaredSymbol(privateMethod);
      var results = await SymbolFinder.FindReferencesAsync(sym, context.Solution, sourceFiles);

      if (results.Count() == 1
         && !results.First().Locations.Any())
      {
        context.AnalysisResults.AddWarning(privateMethod.GetLocation(),
                                           "Private method '{0}' is never referenced.",
                                           privateMethod.Begin.Identifier.Text);
      }
    }

    private async Task CheckForUncalledPrivatePropertyAsync(PropertyStatementSyntax property,
                                                            AnalysisContext context,
                                                            SemanticModel model,
                                                            ImmutableHashSet<Document> sourceFiles)
    {
      if (context.Options.IgnoreGeneratedCode
            && AnalysisUtils.HasGeneratedCodeAttribute(property.AttributeLists, model))
        return;

      var sym = model.GetDeclaredSymbol(property);
      var results = await SymbolFinder.FindReferencesAsync(sym, context.Solution, sourceFiles);
      
      //For properties, the results will return references to any component (get/set etc.) so check that
      //all reference results return no references.
      if (!results.Any(res => res.Locations.Any()))
      {
        context.AnalysisResults.AddWarning(property.GetLocation(),
                                           "Private property '{0}' is never referenced.",
                                           property.Identifier.Text);
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
