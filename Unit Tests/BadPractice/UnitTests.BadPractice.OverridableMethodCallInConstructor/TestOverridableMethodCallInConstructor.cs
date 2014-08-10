using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.BadPractice;

using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace StaticAnalyser.UnitTests.BadPractice
{
  [TestClass]
  public class TestOverridableMethodCallInConstructor
  {
    [TestMethod]
    public void TestAvoidOverridableMethodCallsInConstructorProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          Public Sub New()
              OverridableMethod()
              NotOverridableMethod()
          End Sub

          Protected Overridable Sub OverridableMethod()
          End Sub

          Protected Sub NotOverridableMethod()
          End Sub

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid calling Overridable method 'OverridableMethod' within constructor of type 'Class1'.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorIgnoresGeneratedCodeOnMethodWhenIGCOptionSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
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

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAvoidOverridableMethodCallsInConstructorDoesNotIgnoreGeneratedCodeOnMethodWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
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

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      OverridableMethodCallInConstructor rule = new OverridableMethodCallInConstructor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid calling Overridable method 'OverridableMethod' within constructor of type 'Class1'.", messages[0].ToString());
    }
  }
}