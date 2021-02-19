using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace MyProject.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyProjectAnalyzersCodeFixProvider)), Shared]
    public class MyProjectAnalyzersCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CodeAnalyzers.DateTimeCodeAnalyzer.GetRule().Id); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            await CodeFixes.MyProjectDateTimeCodeFix.RegisterUseMyProjectDateTimeCodeFix(context);
        }
    }
}
