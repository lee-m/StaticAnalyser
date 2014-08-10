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
  public class TestAvoidVisibleConstants
  {
    [TestMethod]
    public void TestVisibleConstantsRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          Public Const Foo = 42, Bar = 56
          Public Shared ReadOnly Baz As Integer = 42
          Private Const Asd = 57

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

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
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
       Public Class Class1
          Public Const Foo = 42, Bar = 56
      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true },
                                                    new AnalysisResults(),
                                                    comp);

      VisibleConstantFieldRule rule = new VisibleConstantFieldRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestVisibleConstantsRuleDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
       Public Class Class1
          Public Const Foo = 42, Bar = 56
      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      VisibleConstantFieldRule rule = new VisibleConstantFieldRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Foo' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:3 - 'Public Const' field 'Bar' in type 'Class1' should be declared as 'Shared ReadOnly' instead.", messages[1].ToString());
    }
  }
}