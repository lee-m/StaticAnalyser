using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.BadPractice;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.BadPractice
{
  [TestClass]
  public class TestAvoidVisibleConstants
  {
    [TestMethod]
    public void TestVisibleConstantsRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Class Class1

          Public Const Foo = 42, Bar = 56
          Public Shared ReadOnly Baz As Integer = 42
          Private Const Asd = 57

      End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VisibleConstantFieldRule rule = new VisibleConstantFieldRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Foo' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Bar' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[1].ToString());
    }

    [TestMethod()]
    public void TestVisibleConstantsRuleIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
       Public Class Class1
          Public Const Foo = 42, Bar = 56
      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VisibleConstantFieldRule rule = new VisibleConstantFieldRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestVisibleConstantsRuleDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
       Public Class Class1
          Public Const Foo = 42, Bar = 56
      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      VisibleConstantFieldRule rule = new VisibleConstantFieldRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Foo' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Bar' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[1].ToString());
    }
  }
}