using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace StaticAnalysis.Analysis.Utils
{
  public static class AnalysisUtils
  {
    /// <summary>
    /// Determines if any attribute in a list of attributes applied to a symbol represents a generated 
    /// code attibute.
    /// </summary>
    /// <param name="attributes">List of attributes to check.</param>
    /// <returns>True if the list of attributes contains the generated code attribute.</returns>
    public static bool HasGeneratedCodeAttribute(ImmutableArray<AttributeData> attributes)
    {
      return attributes.Any(attr => attr.AttributeClass.ToString() == typeof(GeneratedCodeAttribute).FullName);
    }

    /// <summary>
    /// Determines whether a set of attributes contains the GeneratedCodeAttribute
    /// </summary>
    /// <param name="typeDecl">The type to check.</param>
    /// <param name="model">Semantic model instance to use.</param>
    /// <returns>True if the type is compiler generated, false if it's not.</returns>
    public static bool HasGeneratedCodeAttribute(SyntaxList<AttributeListSyntax> attributes, SemanticModel model)
    {
      foreach (AttributeSyntax attr in attributes.SelectMany(attrList => attrList.Attributes))
      {
        //The symbol lookup returns the constructor of the attribute being applied so need to look
        //at the containing symbol to find the actual attribute class
        var symbol = model.GetSymbolInfo(attr.Name);

        if (symbol.Symbol != null
           && symbol.Symbol.ContainingSymbol.ToString() == typeof(GeneratedCodeAttribute).FullName)
          return true;
      }

      return false;
    }
  }
}