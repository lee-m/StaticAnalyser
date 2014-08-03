using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.Linq;

namespace StaticAnalysis.Analysis.Utils
{
  public static class AnalysisUtils
  {
    /// <summary>
    /// Determines whether a given type has the CompilerGeneratedAttribute.
    /// </summary>
    /// <param name="typeDecl">The type to check.</param>
    /// <param name="model">Semantic model instance to use.</param>
    /// <returns>True if the type is compiler generated, false if it's not.</returns>
    public static bool IsCompilerGeneratedType(TypeBlockSyntax typeDecl,
                                                 SemanticModel model)
    {
      foreach (AttributeSyntax attr in typeDecl.Begin.AttributeLists.SelectMany(attrList => attrList.Attributes))
      {
        //The symbol lookup returns the constructor of the attribute being applied so need to look
        //at the containing symbol to find the actual attribute class
        var symbol = model.GetSymbolInfo(attr.Name);

        if (symbol.Symbol != null
           && symbol.Symbol.ContainingSymbol.ToString() == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")
          return true;
      }

      return false;
    }
  }
}