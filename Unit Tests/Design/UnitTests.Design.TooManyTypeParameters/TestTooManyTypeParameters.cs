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
  public class TestTooManyTypeParameters
  {
    [TestMethod]
    public void TestTooManyTypeParametersRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Interface IFoo(Of T, K, V)
        End Interface

        Public Class Foo(Of T, K, V)
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

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
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo(Of T, K, V)
        End Interface

        <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class Foo(Of T, K, V)
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true },
                                                    new AnalysisResults(),
                                                    comp);

      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestTooManyTypeParametersDoesNotIgnoresGeneratedCodeWhenIGCOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Interface IFoo(Of T, K, V)
        End Interface

        <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class Foo(Of T, K, V)
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

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
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Interface IFoo(Of T)
        End Interface

        Public Interface IBar(Of T, K)
        End Interface

        Public Class Foo(Of T)
        End Class

        Public Class Bar(Of T, K)
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false },
                                                    new AnalysisResults(),
                                                    comp);

      TooManyTypeParameters rule = new TooManyTypeParameters();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
