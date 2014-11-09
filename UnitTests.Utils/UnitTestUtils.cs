using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.CodeDom.Compiler;
using System.Linq;

using StaticAnalysis;
using StaticAnalysis.Analysis;

namespace UnitTests.Utils
{
    public static class UnitTestUtils
    {
      public static AnalysisContext CreateAnalysisContext(string sourceText, AnalysisOptions options)
      {
        var projectID = ProjectId.CreateNewId();
        var solution = new CustomWorkspace().CurrentSolution;
        solution = solution.AddProject(projectID, "TestCases", "TestCases", LanguageNames.VisualBasic);
        solution = solution.AddDocument(DocumentId.CreateNewId(projectID), "TestFile.vb", SourceText.From(sourceText));
        solution = solution.AddMetadataReference(projectID, new MetadataFileReference(typeof(Object).Assembly.Location));
        solution = solution.AddMetadataReference(projectID, new MetadataFileReference(typeof(GeneratedCodeAttribute).Assembly.Location));

        Compilation comp = solution.Projects.First().GetCompilationAsync().Result;
        SemanticModel model = comp.GetSemanticModel(comp.SyntaxTrees.First());
        return new AnalysisContext(options, new AnalysisResults(), comp, solution, solution.Projects.First());
      }
    }
}
