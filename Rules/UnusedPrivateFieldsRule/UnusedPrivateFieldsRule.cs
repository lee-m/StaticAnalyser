using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using StaticAnalysis.Analysis;
using StaticAnalysis.Analysis.Utils;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Rules.Performance
{
  /// <summary>
  /// Determines whether a class or structure declaration contains any private fields which are never used.
  /// </summary>
  [Export(typeof(AnalysisRuleBase))]
  public class UnusedPrivateFieldsRule : AnalysisRuleBase
  {
    public override async Task ExecuteRuleAsync(AnalysisContext context)
    {
      Dictionary<INamedTypeSymbol, Dictionary<IFieldSymbol, bool>> fieldDets;
      fieldDets = new Dictionary<INamedTypeSymbol, Dictionary<IFieldSymbol, bool>>();

      foreach (var tree in context.CurrentCompilation.SyntaxTrees)
      {
        SemanticModel model = context.CurrentCompilation.GetSemanticModel(tree);
        CompilationUnitSyntax compilationUnit = (CompilationUnitSyntax)await tree.GetRootAsync();

        //Get a list of type within this compilation unit
        var typeDecls = compilationUnit.DescendantNodes().OfType<TypeBlockSyntax>();

        foreach(TypeBlockSyntax typeDecl in typeDecls)
        {
          //Ignore compiler generated types
          if (AnalysisUtils.IsCompilerGeneratedType(typeDecl, model))
            continue;

          INamedTypeSymbol typeSymbol = model.GetDeclaredSymbol(typeDecl);

          if (!fieldDets.ContainsKey(typeSymbol))
            fieldDets.Add(typeSymbol, new Dictionary<IFieldSymbol, bool>());

          AnalyseTypeDeclaration(typeDecl, typeSymbol, context, model, fieldDets[typeSymbol]);
        }
      }

      //Warn for any field which isn't referenced. Need to wait until the end to handle partial
      //classes where a field may be referenced in a different compilation unit
      foreach(INamedTypeSymbol typeSymbol in fieldDets.Keys)
      {
        foreach (var field in fieldDets[typeSymbol])
        {
          if (!field.Value)
            ReportDiagnostic(context.AnalysisOutputWriter,
                             field.Key.Locations.First(),
                             "Field '{0}' within type '{1}' is never referenced.",
                             field.Key.Name, typeSymbol.Name);
        }
      }
    }

    private void AnalyseTypeDeclaration(TypeBlockSyntax typeDecl, 
                                        INamedTypeSymbol typeSymbol,
                                        AnalysisContext context,
                                        SemanticModel model,
                                        Dictionary<IFieldSymbol, bool> fieldMapping)
    {
      //Get the list of fields for this type
      IEnumerable<IFieldSymbol> typeFields = typeSymbol.GetMembers().OfType<IFieldSymbol>();

      //Filter out any fields which are not declared private and are not implicitly declared
      IEnumerable<IFieldSymbol> privateFields = (from field in typeFields
                                                 where field.DeclaredAccessibility == Accessibility.Private
                                                    && !field.IsImplicitlyDeclared
                                                 select field);

      //For partial classes, we see the type multiple times (once for each partial) so we may 
      //already have added some fiels for this type
      foreach (IFieldSymbol privateField in privateFields)
      {
        if(!fieldMapping.ContainsKey(privateField))
          fieldMapping.Add(privateField, false);
      }
        
      //Make a pass over the identifiers referred to by this type to determine if they in turn
      //refer to a private field
      var identifiers = typeDecl.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();

      foreach (IdentifierNameSyntax identifier in identifiers)
      {
        SymbolInfo identifierSymbol = model.GetSymbolInfo(identifier);
        IFieldSymbol fieldSymbol = identifierSymbol.Symbol as IFieldSymbol;

        //Check that this symbol refers to a field in the current type
        if (fieldSymbol != null
            && identifierSymbol.Symbol.ContainingType == typeSymbol)
          fieldMapping[fieldSymbol] = true;
      }
    }

    protected override AnalysisSyntaxWalker CreateSyntaxWalker(AnalysisContext context)
    {
      throw new NotSupportedException();
    }
  }
}
