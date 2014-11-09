using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

namespace StaticAnalysis.Analysis.Utils
{
  public static class AnalysisUtils
  {
    private static string GeneratedCodeAttributeName = typeof(GeneratedCodeAttribute).Name;
    private static string DllImportAttributeName = typeof(DllImportAttribute).Name;

    /// <summary>
    /// Determines if any attribute in a list of attributes applied to a symbol represents a generated
    /// code attibute.
    /// </summary>
    /// <param name="attributes">List of attributes to check.</param>
    /// <returns>True if the list of attributes contains the generated code attribute.</returns>
    public static bool HasGeneratedCodeAttribute(ImmutableArray<AttributeData> attributes)
    {
      return attributes.Any(attr => attr.AttributeClass.Name == GeneratedCodeAttributeName);
    }

    /// <summary>
    /// Determines whether a set of attributes contains the GeneratedCodeAttribute
    /// </summary>
    /// <param name="attributes">The attributes to check.</param>
    /// <param name="model">Semantic model instance to use.</param>
    /// <returns>True if the type is compiler generated, false if it's not.</returns>
    public static bool HasGeneratedCodeAttribute(SyntaxList<AttributeListSyntax> attributes, SemanticModel model)
    {
      return AttributeListContainsAttribute(attributes, model, GeneratedCodeAttributeName);
    }

    /// <summary>
    /// Determines whether a set of attributes contains the DllImportAttribute
    /// </summary>
    /// <param name="attributes">The attributes to check.</param>
    /// <param name="model">Semantic model instance to use.</param>
    /// <returns>True if the type is compiler generated, false if it's not.</returns>
    public static bool HasDllImportAttribte(SyntaxList<AttributeListSyntax> attributes, SemanticModel model)
    {
      return AttributeListContainsAttribute(attributes, model, DllImportAttributeName);
    }

    /// <summary>
    /// Helper method to check whether a given attribute is present in an attribute list.
    /// </summary>
    /// <param name="attributes">Atribute list to search.</param>
    /// <param name="model">Semantic model to use for looking up symbol info.</param>
    /// <param name="searchAttribute">The attribute to look for.</param>
    /// <returns></returns>
    private static bool AttributeListContainsAttribute(SyntaxList<AttributeListSyntax> attributes,
                                                       SemanticModel model,
                                                       string searchAttribute)
    {
      for(int i = 0; i < attributes.Count; i++)
      {
        var attrs = attributes[i];

        for(int j = 0; j < attrs.Attributes.Count; j++)
        {
          var symbol = model.GetSymbolInfo(attrs.Attributes[j].Name);

          if (symbol.Symbol != null
             && symbol.Symbol.ContainingSymbol.Name == searchAttribute)
            return true;
        }

      }

      return false;
    }
  }
}