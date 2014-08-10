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
  public class TestAbstractTypeWithPublicCtorRule
  {
    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestAbstractTypeWithPublicCtorRuleIgnoresGeneratedCodeWhenIGCOptionSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true },
                                                    new AnalysisResults(),
                                                    comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestAbstractTypeWithPublicCtorRuleDoesNoIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public MustInherit Class Foo

          Public Sub New()
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleOnlyWarnsOnceWhenMultiplePublicCtorsExist()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public MustInherit Class Foo

          Public Sub New()
          End Sub

          Public Sub New(param As String)
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Avoid 'Public' constructors on 'MustInherit' type 'Foo'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForPrivateCtor()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public MustInherit Class Foo

          Private Sub New()
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestAbstractTypeWithPublicCtorRuleDoesNotWarnForProtectedCtor()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public MustInherit Class Foo

          Protected Sub New()
          End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      AbstractTypeWithPublicCtor rule = new AbstractTypeWithPublicCtor();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
