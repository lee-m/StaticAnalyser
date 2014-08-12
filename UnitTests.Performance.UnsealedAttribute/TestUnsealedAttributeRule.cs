using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Performance;

using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace StaticAnalyser.UnitTests.Performance
{
  [TestClass]
  public class TestUnsealedAttributeRule
  {
    [TestMethod]
    public void TestUnsealedAttributeRuleProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class SomeAttribute
            Inherits System.Attribute

        End Class

        Public Class SomeOtherAttribute
            Inherits System.ComponentModel.DescriptionAttribute

            Public Sub New()
                MyBase.New("")
            End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(System.ComponentModel.DescriptionAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Attribute 'SomeAttribute' should be declared as 'NotInheritable'.", messages[0].ToString());
      Assert.AreEqual("TestFile.vb:6 - Attribute 'SomeOtherAttribute' should be declared as 'NotInheritable'.", messages[1].ToString());
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleIgnoresGeneratedCodeWhenIGOptionSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class SomeAttribute
            Inherits System.Attribute

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotIgnoresGeneratedCodeWhenIGOptionNotSet()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class SomeAttribute
            Inherits System.Attribute

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Attribute 'SomeAttribute' should be declared as 'NotInheritable'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnTypesNotInheritedFromAttribute()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class SomeType
            Inherits System.Exception

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnForSealedAttributes()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public NotInheritable Class SomeAttribute
            Inherits System.Attribute

        End Class

        Public NotInheritable Class SomeOtherAttribute
            Inherits System.ComponentModel.DescriptionAttribute

            Public Sub New()
                MyBase.New("")
            End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnForTypesWithNoBaseType()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class SomeType
        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
