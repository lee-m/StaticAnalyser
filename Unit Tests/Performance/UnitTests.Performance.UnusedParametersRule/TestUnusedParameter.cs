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
  public class TestUnusedParameter
  {
    [TestMethod]
    public void TestUnusedParametersRuleDoesNotWarnForEventParameters()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Imports System

        Public Class Class1

            Public Class SomeEventArgs
                Inherits EventArgs

            End Class

            Public Event SomeEvent(paramOne As String, paramTwo As String, paramThree As String)
            Public Event SomeOtherEvent As EventHandler(Of SomeEventArgs)

        End Class

        Public Class Class2
            Inherits Class1

            Private Sub Class2_SomeEvent(paramOne As String, paramTwo As String, paramThree As String) Handles Me.SomeEvent

            End Sub

            Private Sub Class2_SomeOtherEvent(sender As Object, e As Class1.SomeEventArgs) Handles Me.SomeOtherEvent

            End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestUnusedParametersProducesExpectedWarnings()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions(), new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresGeneratedCodeWhenIGCSetToTrue()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = true }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestUnusedParametersDoesNotIgnoresGeneratedCodeWhenIGCSetToFalse()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresPartialMethods()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Partial Public Class Class1

            Partial Private Sub Test(usedInPartial1 As String, unusedParam As String)
            End Sub

        End Class

        Partial Public Class Class1

            Private Sub Test(usedInPartial1 As String, unusedParam As String)
                usedInPartial1 = String.Empty
            End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:10 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresAbstractMethods()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public MustInherit Class Class1

            Public MustOverride Sub Test(paramOne As String)

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresPInvokeMethods()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Class1

            <System.Runtime.InteropServices.DllImport(""winmm.dll"")> _
            Public Shared Function waveOutGetVolume(ByVal hwo As IntPtr, ByRef dwVolume As System.UInt32) As Integer
            End Function

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresOverridableAndOverridesMethods()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Class Foo

            Public Overridable Sub SomeMethod(paramOne As String)
            End Sub

        End Class

        Public Class Bar
            Inherits Foo

            Public Overrides Sub SomeMethod(paramOne As String)
            End Sub

        End Class", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresInterfaceMethods()
    {
      SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(
      @"Public Interface IFoo
            Sub SomeFunc(paramOne As String, paramTwo As String)
        End Interface", "TestFile.vb");

      Compilation comp = VisualBasicCompilation.Create("Test")
                         .AddReferences(new MetadataFileReference(typeof(Object).Assembly.Location))
                         .AddReferences(new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location))
                         .AddSyntaxTrees(syntaxTree);
      SemanticModel model = comp.GetSemanticModel(syntaxTree);
      AnalysisContext context = new AnalysisContext(new AnalysisOptions() { IgnoreGeneratedCode = false }, new AnalysisResults(), comp);

      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
