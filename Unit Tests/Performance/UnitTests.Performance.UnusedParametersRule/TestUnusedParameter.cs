using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Performance;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Performance
{
  [TestClass]
  public class TestUnusedParameter
  {
    [TestMethod]
    public void TestUnusedParametersRuleDoesNotWarnForEventParameters()
    {
      string sourceText = 
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

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestUnusedParametersProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresGeneratedCodeWhenIGCSetToTrue()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod()]
    public void TestUnusedParametersDoesNotIgnoresGeneratedCodeWhenIGCSetToFalse()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Public Sub Test(usedParam As String, unusedParam As String)
              Dim x = usedParam
          End Sub

      End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:3 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresPartialMethods()
    {
      string sourceText = 
      @"Partial Public Class Class1

            Partial Private Sub Test(usedInPartial1 As String, unusedParam As String)
            End Sub

        End Class

        Partial Public Class Class1

            Private Sub Test(usedInPartial1 As String, unusedParam As String)
                usedInPartial1 = String.Empty
            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:10 - Parameter 'unusedParam' to method 'Test' is never referenced.", messages[0].ToString());
    }

    [TestMethod()]
    public void TestUnusedParametersIgnoresAbstractMethods()
    {
      string sourceText = 
      @"Public MustInherit Class Class1

            Public MustOverride Sub Test(paramOne As String)

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresPInvokeMethods()
    {
      string sourceText = 
      @"Public Class Class1

            <System.Runtime.InteropServices.DllImport(""winmm.dll"")> _
            Public Shared Function waveOutGetVolume(ByVal hwo As IntPtr, ByRef dwVolume As System.UInt32) As Integer
            End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresOverridableAndOverridesMethods()
    {
      string sourceText = 
      @"Public Class Foo

            Public Overridable Sub SomeMethod(paramOne As String)
            End Sub

        End Class

        Public Class Bar
            Inherits Foo

            Public Overrides Sub SomeMethod(paramOne As String)
            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersIgnoresInterfaceMethods()
    {
      string sourceText = 
      @"Public Interface IFoo
            Sub SomeFunc(paramOne As String, paramTwo As String)
        End Interface";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersRuleDoesNotWarnForEventHandlersAddedViaAddHandler()
    {
      string sourceText = 
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

            Public Sub New()
                AddHandler SomeEvent, AddressOf Class2_SomeEvent
                AddHandler SomeOtherEvent, AddressOf Class2_SomeOtherEvent
            End Sub

            Private Sub Class2_SomeEvent(paramOne As String, paramTwo As String, paramThree As String)

            End Sub

            Private Sub Class2_SomeOtherEvent(sender As Object, e As Class1.SomeEventArgs)

            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnusedParametersRuleIgnoresAddHandlerWithNoAddressOf()
    {
      string sourceText = 
      @"Imports System

        Public Class Class1

            Public Class SomeEventArgs
                Inherits EventArgs

            End Class

            Public Event SomeOtherEvent As EventHandler(Of SomeEventArgs)

            Public Sub AddValueChangedHandler(handler As EventHandler(Of SomeEventArgs))
                AddHandler SomeOtherEvent, handler
            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnusedParametersRule rule = new UnusedParametersRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}