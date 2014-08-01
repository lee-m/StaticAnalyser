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
    /// Outputs a diagnostic messge.
    /// </summary>
    /// <param name="loc">Location of the code which triggered the message.</param>
    /// <param name="message">The message to output.</param>
    protected void ReportDiagnostic(Location loc, string message)
    {
      lock (mContext.AnalysisOutputWriter)
      {
        //Line numbers are 0 based in roslyn so need to increment it by one to 
        //get the line number the user expects
        mContext.AnalysisOutputWriter.WriteLine("{0}:{1} - {2}",
                                                loc.FilePath,
                                                loc.GetLineSpan().StartLinePosition.Line + 1,
                                                message);
      }
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