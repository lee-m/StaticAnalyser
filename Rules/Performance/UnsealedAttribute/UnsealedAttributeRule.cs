using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using StaticAnalysis.Analysis;

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace StaticAnalysis.Rules.Performance
{
  [Export(typeof(AnalysisRuleBase))]
  public class UnsealedAttributeRule : TypeAnalysisRule
  {
    public override void AnalyseTypeDeclaration(TypeBlockSyntax node, AnalysisContext context, SemanticModel model)
    {
      //If the type doesn't inherit from anything it can't be an attribute
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)model.GetDeclaredSymbol(node);

      if (typeSymbol.BaseType == null)
        return;

      //If it's sealed, we can skip it
      if (typeSymbol.IsSealed)
        return;

      //If the type doesn't inherit from System.Attribute, walk the inheritance train to see if
      //it derives from a type that does inherit from System.Attribute
      bool isAttribute = false;
      INamedTypeSymbol baseType = typeSymbol.BaseType;
      string attributeClassName = typeof(Attribute).FullName;

      while(baseType != null)
      {
        if (baseType.ToString() == attributeClassName)
        {
          isAttribute = true;
          break;
        }
        else
          baseType = baseType.BaseType;
      }

      if (!isAttribute)
        return;

      //If we get here it's an unsealed attribute so warn
      context.AnalysisResults.AddWarning(node.GetLocation(),
                                         "Attribute '{0}' should be declared as 'NotInheritable'.",
                                         typeSymbol.Name);
    }
  }
}
