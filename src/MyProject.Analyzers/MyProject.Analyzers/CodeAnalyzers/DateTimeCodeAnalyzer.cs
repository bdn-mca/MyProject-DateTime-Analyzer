using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MyProject.Analyzers.CodeAnalyzers
{
    public static class DateTimeCodeAnalyzer
    {
        private static DiagnosticDescriptor rule = new DiagnosticDescriptor(
            "MyProjectAnalyzer",
            "DateTime should not be used in MyProject.",
            "Type name '{0}' should not be used in MyProject.",
            "DateTimeUsage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "MyPrjectDateTime should be used instead.");

        private static readonly string[] propertyParent = { "DateTime", "DateTimeOffset" };

        public static DiagnosticDescriptor GetRule()
        {
            return rule;
        }

        public static void RegisterDateTimeCodeAnalyzer(AnalysisContext context)
        {
            if (propertyParent != null && propertyParent.Length > 0)
            {
                context.RegisterSyntaxNodeAction(AnalyzeIdentifierName, SyntaxKind.IdentifierName);
            }
        }

        private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierName = context.Node as IdentifierNameSyntax;
            if (identifierName != null)
            {
                var identifierValue = identifierName.Identifier;

                if (IsDateTimeExpression(identifierValue.Text)
                    && IsDateTimeAssignment(identifierValue))
                {
                    var dateTimeDiagnostic = Diagnostic.Create(GetRule(), identifierValue.GetLocation(), identifierName);
                    context.ReportDiagnostic(dateTimeDiagnostic);
                }
            }
        }

        private static bool IsDateTimeExpression(string expression)
        {
            return IsInArray(expression, propertyParent);
        }

        private static bool IsDateTimeAssignment(SyntaxToken identifierValue)
        {
            SyntaxNode tokenParent = identifierValue.Parent;

            while (tokenParent != null)
            {
                if (tokenParent is ExpressionStatementSyntax
                    || tokenParent is EqualsValueClauseSyntax
                    || tokenParent is ReturnStatementSyntax
                    || tokenParent is ObjectCreationExpressionSyntax)
                {
                    return true;
                }

                if (tokenParent is UsingDirectiveSyntax
                    || tokenParent is ConstructorDeclarationSyntax
                    || tokenParent is MethodDeclarationSyntax
                    || tokenParent is FieldDeclarationSyntax
                    || tokenParent is PropertyDeclarationSyntax
                    || tokenParent is LocalDeclarationStatementSyntax
                    || tokenParent is CastExpressionSyntax
                    || tokenParent.IsKind(SyntaxKind.AsExpression)
                    || tokenParent is TypeOfExpressionSyntax
                    || tokenParent is TypeArgumentListSyntax)
                {
                    return false;
                }

                tokenParent = tokenParent.Parent;
            }

            return false;
        }

        private static bool IsInArray(string value, string[] array)
        {
            if (string.IsNullOrWhiteSpace(value) || array == null || array.Length == 0)
            {
                return false;
            }

            return array.Contains(value);
        }
    }
}
