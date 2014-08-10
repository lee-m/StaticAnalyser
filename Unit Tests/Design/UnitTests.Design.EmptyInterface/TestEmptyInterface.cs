using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Design;

using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace StaticAnalyser.UnitTests.Design
{
  [TestClass]
  public class TestEmptyInterfaceRule
  {
    [TestMethod]
    public void TestEmptyInterfaceRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Interface IFoo
        End Interface", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Interface 'IFoo' has no members.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo
        End Interface", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true },
                                                    new AnalysisResults(),
                                                    comp);

      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo
        End Interface", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Interface 'IFoo' has no members.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestEmptyInterfaceRuleDoesNotWarnForEmptyClasses()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class IFoo
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      EmptyInterfaceRule rule = new EmptyInterfaceRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
