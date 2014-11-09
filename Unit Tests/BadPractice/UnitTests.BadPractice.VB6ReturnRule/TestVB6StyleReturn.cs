using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.BadPractice;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.BadPractice
{
  [TestClass]
  public class TestVB6ReturnRule
  {
    [TestMethod]
    public void TestVB6ReturnRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Class Class1

          Public Function Test() As Integer
              Test = 42
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid use of VB6 style return statements.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestVB6ReturnRuleIgnoresGeneratedCodeOnMethodWhenIGCOptionSet()
    {
      string sourceText = 
      @"Public Class Class1

            <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
            Public Function Test() As Integer
                Test = 42
            End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestVB6ReturnRuleDoesNotIgnoreGeneratedCodeOnMethodWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"Public Class Class1

            <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
            Public Function Test() As Integer
                Test = 42
            End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:5 - Avoid use of VB6 style return statements.", messages[0].ToString());
    }
  }
}