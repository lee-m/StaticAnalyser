using StaticAnalysis.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
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
    /// <param name="context">Analysis context.</param>
    public async Task ExecuteRulesAsync(AnalysisContext context)
    {
      await Task.WhenAll(mRules.Select(rule => rule.Value.ExecuteRuleAsync(context)));
    }
  }
}