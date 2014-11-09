using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Performance;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Performance
{
  [TestClass]
  public class TestUncalledPrivateCodeRule
  {
    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleWarnsForUncalledPrivateMethod()
    {
      string sourceText =
      @"Public Class Class1

          Private Sub UncalledSub()
          End Sub

          Private Function UncalledMethod() As Integer
              Return 0
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForGeneratedUncalledPrivateMethodWhenIGCSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Sub UncalledSub()
          End Sub

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Function UncalledMethod() As Integer
              Return 0
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleWarnsForGeneratedUncalledPrivateMethodWhenIGCNotSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Sub UncalledSub()
          End Sub

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Function UncalledMethod() As Integer
              Return 0
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleWarnsForUncalledPrivateProperty()
    {
      string sourceText = 
      @"Public Class Class1

          Private Property PrivateProperty As Integer

          Private ReadOnly Property PrivateReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledGeneratedPrivatePropertyWhenIGCSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Property PrivateProperty As Integer

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private ReadOnly Property PrivateReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleWarnsForUncalledGeneratedPrivatePropertyWhenIGCNotSet()
    {
      string sourceText = 
      @"Public Class Class1

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private Property PrivateProperty As Integer

          <System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
          Private ReadOnly Property PrivateReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForCalledPrivateMethod()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub New()
              
              CalledSub()
              Call CalledMethod()

              If CalledMethod2() > 1 Then
              End If
              
              If CalledMethod3(False) Then
              End If

          End Sub

          Private Sub CalledSub()
          End Sub

          Private Function CalledMethod() As Integer
              Return 0
          End Function

          Private Function CalledMethod2() As Integer
              Return 0
          End Function

          Private Function CalledMethod3(param As Boolean) As Boolean
            Return False
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForCalledPrivateProperty()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub New()
              Dim a = PrivateProperty
              Dim b = PrivateReadOnlyProperty
          End Sub

          Private Property PrivateProperty As Integer

          Private ReadOnly Property PrivateReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledProtectedMethod()
    {
      string sourceText = 
      @"Public Class Class1

          Protected Sub UncalledSub()
          End Sub

          Protected Friend Sub UncalledFriendSub()
          End Sub

          Protected Function UncalledMethod() As Integer
              Return 0
          End Function

          Protected Friend Function UncalledFriendMethod() As Integer
              Return 0
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledProtectedProperty()
    {
      string sourceText = 
      @"Public Class Class1

          Protected Property ProtectedProperty As Integer

          Protected ReadOnly Property ProtectedReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledPublicMethod()
    {
      string sourceText = 
      @"Public Class Class1

          Public Sub PublicSub()
          End Sub

          Public Function PublicMethod() As Integer
              Return 0
          End Function

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledPublicProperty()
    {
      string sourceText = 
      @"Public Class Class1

          Public Property PublicProperty As Integer

          Public ReadOnly Property PublicReadOnlyProperty
              Get
                  Return 0
              End Get
          End Property

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUncalledPrivateCodeRuleRuleDoesNotWarnForUncalledPrivateMethodPassedToAddHandler()
    {
      string sourceText =
      @"Public Class Class1

          Public Event AnEvent()
          Public Event AnotherEvent()

          Private Sub SomeEventHandler()
          End Sub

          Private Sub AnotherEventHandler()
          End Sub

          Public Sub Test()

            Dim c1 As New Class1 
            AddHandler c1.AnEvent, AddressOf SomeEventHandler
  
            Dim handler As AnotherEventEventHandler = AddressOf AnotherEventHandler
            AddHandler c1.AnotherEvent, handler
            
          End Sub          

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UncalledPrivateCodeRule rule = new UncalledPrivateCodeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
