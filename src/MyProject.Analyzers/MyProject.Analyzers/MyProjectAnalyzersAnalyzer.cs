using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace MyProject.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MyProjectAnalyzersAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(CodeAnalyzers.DateTimeCodeAnalyzer.GetRule());
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            CodeAnalyzers.DateTimeCodeAnalyzer.RegisterDateTimeCodeAnalyzer(context);
        }
    }
}
