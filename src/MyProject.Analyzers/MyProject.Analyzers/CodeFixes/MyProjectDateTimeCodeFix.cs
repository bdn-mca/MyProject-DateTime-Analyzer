using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Analyzers.CodeFixes
{
    public static class MyProjectDateTimeCodeFix
    {
        public static async Task RegisterUseMyProjectDateTimeCodeFix(CodeFixContext context)
        {
            const string useMyProjectDateTimeTitle = "Use MyProjectDateTime";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First(c => c.Id.Equals(CodeAnalyzers.DateTimeCodeAnalyzer.GetRule().Id));
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var identifierNode = root.FindNode(diagnosticSpan);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: useMyProjectDateTimeTitle,
                    createChangedDocument: c => ProvideDocumentCodeFix(context.Document, identifierNode, c),
                    equivalenceKey: useMyProjectDateTimeTitle),
                diagnostic);
        }

        private static async Task<Document> ProvideDocumentCodeFix(Document document, SyntaxNode dateTimeNode, CancellationToken cancellationToken)
        {
            const string replacementNode = "MyProjectDateTime";

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(dateTimeNode, SyntaxFactory.IdentifierName(replacementNode));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
