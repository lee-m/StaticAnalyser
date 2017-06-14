using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace StaticAnalysis.Rules.Performance
{
  /// <summary>
  /// Determines whether a class or structure declaration contains any private fields which are never used.
  /// </summary>
  [Export(typeof(AnalysisRuleBase))]
  public class UnusedPrivateFieldsRule : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      //Get the list of fields for this type
      ITypeSymbol typeSymbol = model.GetDeclaredSymbol(node);
      IEnumerable<IFieldSymbol> typeFields = typeSymbol.GetMembers().OfType<IFieldSymbol>();

      //Filter out any fields which are not declared private and are not implicitly declared
      IEnumerable<IFieldSymbol> privateFields = (from field in typeFields
                                                 where field.DeclaredAccessibility == Accessibility.Private
                                                    && !field.IsImplicitlyDeclared
                                                 select field);

      //Look for any reference in any source file for this type (partial types)
      var documents = typeSymbol.Locations.Select(loc => context.Project.GetDocument(loc.SourceTree)).ToImmutableHashSet();

      foreach (IFieldSymbol privateField in privateFields)
      {
        var lookupResults = SymbolFinder.FindReferencesAsync(privateField, context.Solution, documents).Result;
        
        if(!lookupResults.Any(res => res.Locations.Any()))
          context.AnalysisResults.AddWarning(privateField.Locations.First(),
                                           "Field '{0}' within type '{1}' is never referenced.",
                                           privateField.Name, typeSymbol.Name);
      }
    }
  }
}