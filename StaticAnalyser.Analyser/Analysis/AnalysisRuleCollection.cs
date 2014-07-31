using StaticAnalysis.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace StaticAnalysis.Analysis
{
  /// <summary>
  /// Collection of rules to execute.
  /// </summary>
  public class AnalysisRuleCollection
  {
    /// <summary>
    /// Set of dynamically loaded rules.
    /// </summary>
    private List<Lazy<AnalysisRuleBase>> mRules;

    /// <summary>
    /// Loads the available rules from the configured rules directory.
    /// </summary>
    public AnalysisRuleCollection()
    {
      var catalog = new CompositionContainer(new DirectoryCatalog(Settings.Default.RuleLocation));
      mRules = catalog.GetExports<AnalysisRuleBase>().ToList();
    }

    /// <summary>
    /// Runs each rule on a particular compilation unit.
    /// </summary>
    /// <param name="compilationUnit">Compilation unit to analyse.</param>
    /// <param name="context">Analysis context.</param>
    public void ExecuteRules(CompilationUnitSyntax compilationUnit, AnalysisContext context)
    {
      foreach (var rule in mRules)
        rule.Value.ExecuteRule(compilationUnit, context);
    }
  }
}
