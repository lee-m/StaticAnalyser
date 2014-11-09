using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Design;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Design
{
  [TestClass]
  public class TestTooManyTypeParameters
  {
    [TestMethod]
    public void TestTooManyTypeParametersRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Interface IFoo(Of T, K, V)
        End Interface

        Public Class Foo(Of T, K, V)
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid excessive type parameters to interface 'IFoo'.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:4 - Avoid excessive type parameters to type 'Foo'.", messages[1].ToString());
    }

    [TestMethod()]
    public void TestTooManyTypeParametersIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo(Of T, K, V)
        End Interface

        <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class Foo(Of T, K, V)
        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestTooManyTypeParametersDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo(Of T, K, V)
        End Interface

        <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class Foo(Of T, K, V)
        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid excessive type parameters to interface 'IFoo'.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:5 - Avoid excessive type parameters to type 'Foo'.", messages[1].ToString());
    }

    [TestMethod()]
    public void TestTooManyTypeParametersDoesNotWarnForOneOrTwoTypeParams()
    {
      string sourceText =
      @"Public Interface IFoo(Of T)
        End Interface

        Public Interface IBar(Of T, K)
        End Interface

        Public Class Foo(Of T)
        End Class

        Public Class Bar(Of T, K)
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}