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
  public class TestVB6ReturnRule
  {
    [TestMethod]
    public void TestVB6ReturnRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          Public Function Test() As Integer
              Test = 42
          End Function

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:4 - Avoid use of VB6 style return statements.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestVB6ReturnRuleIgnoresGeneratedCodeOnMethodWhenIGCOptionSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

            <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
            Public Function Test() As Integer
                Test = 42
            End Function

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true },
                                                    new AnalysisResults(),
                                                    comp);

      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestVB6ReturnRuleDoesNotIgnoreGeneratedCodeOnMethodWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

            <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
            Public Function Test() As Integer
                Test = 42
            End Function

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      VB6StyleReturnRule rule = new VB6StyleReturnRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:5 - Avoid use of VB6 style return statements.", messages[0].ToString());
    }
  }
}