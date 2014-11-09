using Microsoft.VisualStudio.TestTools.UnitTesting;

using StaticAnalysis;
using StaticAnalysis.Analysis;
using StaticAnalysis.Rules.Performance;

using System.Linq;

using UnitTests.Utils;

namespace StaticAnalyser.UnitTests.Performance
{
  [TestClass]
  public class TestUnsealedAttributeRule
  {
    [TestMethod]
    public void TestUnsealedAttributeRuleProducesExpectedWarnings()
    {
      string sourceText = 
      @"Public Class SomeAttribute
            Inherits System.Attribute

        End Class

        Public Class SomeOtherAttribute
            Inherits System.ComponentModel.DescriptionAttribute

            Public Sub New()
                MyBase.New("")
            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
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
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class SomeAttribute
            Inherits System.Attribute

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = true };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotIgnoresGeneratedCodeWhenIGOptionNotSet()
    {
      string sourceText = 
      @"<System.CodeDom.Compiler.GeneratedCode(""Tool"", ""1.0.0.0"")>
        Public Class SomeAttribute
            Inherits System.Attribute

        End Class";

      AnalysisOptions options = new AnalysisOptions() { IgnoreGeneratedCode = false };
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(1, messages.Count);
      Assert.AreEqual("TestFile.vb:1 - Attribute 'SomeAttribute' should be declared as 'NotInheritable'.", messages[0].ToString());
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnTypesNotInheritedFromAttribute()
    {
      string sourceText = 
      @"Public Class SomeType
            Inherits System.Exception

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnForSealedAttributes()
    {
      string sourceText = 
      @"Public NotInheritable Class SomeAttribute
            Inherits System.Attribute

        End Class

        Public NotInheritable Class SomeOtherAttribute
            Inherits System.ComponentModel.DescriptionAttribute

            Public Sub New()
                MyBase.New("")
            End Sub

        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }

    [TestMethod]
    public void TestUnsealedAttributeRuleDoesNotWarnForTypesWithNoBaseType()
    {
      string sourceText = 
      @"Public Class SomeType
        End Class";

      AnalysisOptions options = new AnalysisOptions();
      AnalysisContext context = UnitTestUtils.CreateAnalysisContext(sourceText, options);
      UnsealedAttributeRule rule = new UnsealedAttributeRule();
      rule.ExecuteRuleAsync(context).Wait();

      var messages = context.AnalysisResults.Messages.ToList();
      Assert.AreEqual(0, messages.Count);
    }
  }
}
