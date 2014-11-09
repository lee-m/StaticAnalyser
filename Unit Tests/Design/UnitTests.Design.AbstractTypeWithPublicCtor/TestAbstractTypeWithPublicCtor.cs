using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Design;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Design
{
  [TestClass]
  public class TestAbstractTypeWithPublicCtorRule
  {
    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestAbstractTypeWithPublicCtorRuleIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAbstractTypeWithPublicCtorRuleDoesNoIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleOnlyWarnsOnceWhenMultiplePublicCtorsExist()
    {
      string sourceText = 
      @"Public MustInherit Class Foo

          Public Sub New()
          End Sub

          Public Sub New(param As String)
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForPrivateCtor()
    {
      string sourceText = 
      @"Public MustInherit Class Foo

          Private Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForProtectedCtor()
    {
      string sourceText = 
      @"Public MustInherit Class Foo

          Protected Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForNonAbstractTypes()
    {
      string sourceText = 
      @"Public Class Foo

          Public Sub New()
          End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForAbstractTypeWithNoCtors()
    {
      string sourceText = 
      @"Public MustInherit Class Foo
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}