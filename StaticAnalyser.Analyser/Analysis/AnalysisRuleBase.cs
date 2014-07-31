using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Base for all analysis rules.
  /// </summary>
  public abstract class AnalysisRuleBase
  {
    /// <summary>
    /// Contextual information to use during the analysis.
    /// </summary>
    AnalysisContext mContext;

    /// <summary>
    /// Syntax walker used to traverse a compiation unit.
    /// </summary>
    VisualBasicSyntaxWalker mSyntaxWalker;

    /// <summary>
    /// Initialises a new rule instance.
    /// </summary>
    /// <param name="context">Contextual information to use during the analysis.</param>
    public AnalysisRuleBase()
    {
      mSyntaxWalker = CreateSyntaxWalker();
    }

    /// <summary>
    /// Executes this rule on a single compilation unit.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit to analyse.</param>
    public void ExecuteRule(CompilationUnitSyntax compilationUnit, 
                            AnalysisContext context)
    {
      mContext = context;
      mSyntaxWalker.Visit(compilationUnit);
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected abstract VisualBasicSyntaxWalker CreateSyntaxWalker();

    /// <summary>
    /// Accessor for the analysis context.
    /// </summary>
    protected AnalysisContext Context
    {
      get
      {
        return mContext;
      }
    }
  }
}