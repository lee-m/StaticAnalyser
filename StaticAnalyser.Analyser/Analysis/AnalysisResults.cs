using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Collection of results from an analysis run,
  /// </summary>
  public class AnalysisResults
  {
    /// <summary>
    /// Set of warnings generated.
    /// </summary>
    private List<AnalysisMessage> mResults;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public AnalysisResults()
    {
      mResults = new List<AnalysisMessage>();
    }

    /// <summary>
    /// Adds a warning.
    /// </summary>
    /// <param name="location">Location of the code which triggered the message.</param>
    /// <param name="message">The message to output.</param>
    /// <param name="messageArgs">Any message formatting arguments.</param>
    public void AddWarning(Location location,
                           string message,
                           params object[] messageArgs)
    {
      lock (mResults)
      {
        //Line numbers are 0 based in roslyn so need to increment it by one to
        //get the line number the user expects
        mResults.Add(new AnalysisMessage()
          {
            Message = string.Format(message, messageArgs),
            SourceFile = location.FilePath,
            LineNumber = location.GetLineSpan().StartLinePosition.Line + 1,
          });
      }
    }

    /// <summary>
    /// Outputs all warnings to the specified output writer.
    /// </summary>
    /// <param name="outputWriter">Output for the warnings.</param>
    public void OutputResults(TextWriter outputWriter)
    {
      var orderedResults = mResults.OrderBy(r => r.SourceFile).ThenBy(r => r.LineNumber);

      foreach (var result in orderedResults)
        outputWriter.WriteLine(result.ToString());
    }

    /// <summary>
    /// Accessor for the analysis results.
    /// </summary>
    public IEnumerable<AnalysisMessage> Messages
    {
      get { return mResults; }
    }
  }
}

/// <summary>
/// Details about a single warning.
/// </summary>
public class AnalysisMessage
{
  /// <summary>
  /// Source file the warning appears in
  /// </summary>
  public string SourceFile { get; set; }

  /// <summary>
  /// Line number of the code which triggered it.
  /// </summary>
  public int LineNumber { get; set; }

  /// <summary>
  /// The warning message.
  /// </summary>
  public string Message { get; set; }

  /// <summary>
  /// Converts this warning into a string to show to the user.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return string.Format("{0}:{1} - {2}", SourceFile, LineNumber, Message);
  }
}