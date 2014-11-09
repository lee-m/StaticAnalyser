using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.BadPractice;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.BadPractice
{
  [TestClass]
  public class TestOverridableMethodCallInConstructor
  {
    [TestMethod]
    public void TestAvoidOverridableMethodCallsInConstructorProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid calling Overridable method 'OverridableMethod' within constructor of type 'Class1'.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorIgnoresGeneratedCodeOnMethodWhenIGCOptionSet()
    {
      string sourceText =
      @"Public Class Class1

          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorDoesNotIgnoreGeneratedCodeOnMethodWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid calling Overridable method 'OverridableMethod' within constructor of type 'Class1'.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorHandlesLateBoundMethodCall()
    {
      string sourceText = 
      @"Option Strict Off

       Public Class Test
  
          Public Sub New()

              Dim a As New Object
              a.SomeLateBoundMethod()

          End Sub

       End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorIgnoresGeneratedCodeOnCtorWhenIGCOptionSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorDoesNotIgnoresGeneratedCodeOnCtorWhenIGCOptionNotSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:5 - Avoid calling Overridable method 'OverridableMethod' within constructor of type 'Class1'.", messages[0].ToString());
    }
  }
}