using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace StaticAnalysis.Analysis
{
  public class AnalysisSyntaxWalker : VisualBasicSyntaxWalker
  {
    private AnalysisContext mContext;
    private SemanticModel mCurrentModel;

    public AnalysisSyntaxWalker(AnalysisContext context)
      : base(SyntaxWalkerDepth.Node)
    {
      mContext = context;
    }

    public void WalkSyntaxTree(SyntaxNode tree, SemanticModel model)
    {
      mCurrentModel = model;
      Visit(tree);
    }

    protected AnalysisContext Context
    {
      get { return mContext; }
    }

    protected SemanticModel CurrentSemanticModel
    {
      get { return mCurrentModel; }
    }
  }

  public class TypedAnalysisSyntaxWalker<TRule> : AnalysisSyntaxWalker where TRule : AnalysisRuleBase
  {
    private TRule mRule;

    public TypedAnalysisSyntaxWalker(TRule rule, AnalysisContext context)
      : base(context)
    {
      mRule = rule;
    }

    protected TRule Rule
    {
      get { return mRule; }
    }
  }
}