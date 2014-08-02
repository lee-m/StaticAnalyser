using System.IO;
using System.Threading.Tasks;

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
    /// Lock object used to synchronise access to the output writer.
    /// </summary>
    private object mOutputLock;

    /// <summary>
    /// Initialises a new rule instance.
    /// </summary>
    /// <param name="context">Contextual information to use during the analysis.</param>
    public AnalysisRuleBase()
    {
      mOutputLock = new object();
    }

    /// <summary>
    /// Executes this rule on a single compilation unit.
    /// </summary
    /// <param name="compilationUnit">The compilation unit to analyse.</param>
    public virtual async Task ExecuteRuleAsync(AnalysisContext context)
    {
      AnalysisSyntaxWalker walker = CreateSyntaxWalker(context);
      
      foreach (var tree in context.CurrentCompilation.SyntaxTrees)
      {
        SemanticModel model = context.CurrentCompilation.GetSemanticModel(tree);
        walker.WalkSyntaxTree(await tree.GetRootAsync(), model);
      }
    }

    /// <summary>
    /// Outputs a diagnostic messge.
    /// </summary>
    /// <param name="outputWriter">Output for the diagnostic.</param>
    /// <param name="loc">Location of the code which triggered the message.</param>
    /// <param name="message">The message to output.</param>
    /// <param name="messageArgs">Any message formatting arguments.</param>
    protected void ReportDiagnostic(TextWriter outputWriter,
                                    Location loc, 
                                    string message, 
                                    params object[] messageArgs)
    {
      lock (mOutputLock)
      {
        //Line numbers are 0 based in roslyn so need to increment it by one to 
        //get the line number the user expects
        outputWriter.WriteLine("{0}:{1} - {2}",
                               loc.FilePath,
                               loc.GetLineSpan().StartLinePosition.Line + 1,
                               string.Format(message, messageArgs));
      }
    }

    /// <summary>
    /// Factory method to create a syntax walker specific to this type of rule
    /// </summary>
    /// <returns></returns>
    protected abstract AnalysisSyntaxWalker CreateSyntaxWalker(AnalysisContext context);
  }
}