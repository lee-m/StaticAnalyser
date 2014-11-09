using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Design;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Design
{
  [TestClass]
  public class TestEmptyInterfaceRule
  {
    [TestMethod]
    public void TestEmptyInterfaceRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Interface IFoo
        End Interface";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Interface 'IFoo' has no members.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo
        End Interface";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo
        End Interface";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Interface 'IFoo' has no members.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleDoesNotWarnForEmptyClasses()
    {
      string sourceText = 
      @"Public Class IFoo
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleDoesNotWarnForNonEmptyInterfaces()
    {
      string sourceText = 
      @"Public Class IFoo
            Sub SomeMethod()
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}