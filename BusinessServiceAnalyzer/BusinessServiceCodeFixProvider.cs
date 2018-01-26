#region Using Directives

using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer {

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BusinessServiceCodeFixProvider)), Shared]
    public class BusinessServiceCodeFixProvider: CodeFixProvider {

        private const string Title = "Make method overridable";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            BusinessServiceAnalyzer.VirtualMethodDiagnosticId,
            BusinessServiceAnalyzer.SealedMethodDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {

            var root = await context.Document.GetSyntaxRootAsync().ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics) {

                var diagnosticSpan    = diagnostic.Location.SourceSpan;
                var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent as MethodDeclarationSyntax;

                if (methodDeclaration == null) {
                    continue;
                }

                if (diagnostic.Id == BusinessServiceAnalyzer.VirtualMethodDiagnosticId) {
                   
                    context.RegisterCodeFix(action: CodeAction.Create(
                                                title                : Title,
                                                createChangedDocument: cancellationToken => EnsureMethodVirtualAsync(context.Document, methodDeclaration, cancellationToken),
                                                equivalenceKey       : Title),
                                            diagnostic: diagnostic);
                } else if (diagnostic.Id == BusinessServiceAnalyzer.SealedMethodDiagnosticId) {
                   
                    context.RegisterCodeFix(action: CodeAction.Create(
                                                title                : Title,
                                                createChangedDocument: cancellationToken => EnsureMethodNonSealedAsync(context.Document, methodDeclaration, cancellationToken),
                                                equivalenceKey       : Title),
                                            diagnostic: diagnostic);
                }

            }
        }

        private async Task<Document> EnsureMethodVirtualAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken) {

            return await EnsureMethodModifierAsync(
                document         : document,
                methodDeclaration: methodDeclaration,
                removeModifier   : SyntaxKind.None,
                addModifier      : SyntaxKind.VirtualKeyword,
                cancellationToken: cancellationToken);
        }

        private async Task<Document> EnsureMethodNonSealedAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken) {

            return await EnsureMethodModifierAsync(
                document         : document,
                methodDeclaration: methodDeclaration,
                removeModifier   : SyntaxKind.SealedKeyword,
                addModifier      : SyntaxKind.None,
                cancellationToken: cancellationToken);
        }

        private async Task<Document> EnsureMethodModifierAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            SyntaxKind removeModifier,
            SyntaxKind addModifier,
            CancellationToken cancellationToken) {

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var modifiers = methodDeclaration.Modifiers;

            if (removeModifier != SyntaxKind.None) {
                var removeModifierIndex = methodDeclaration.Modifiers.IndexOf(removeModifier);
                if (removeModifierIndex != -1) {
                    modifiers = modifiers.RemoveAt(removeModifierIndex);
                }
            }

            if (addModifier != SyntaxKind.None) {
                var addModifierIndex = methodDeclaration.Modifiers.IndexOf(addModifier);
                if (addModifierIndex == -1) {
                    modifiers = modifiers.Add(SyntaxFactory.Token(addModifier));
                }
            }

            var newMethodDeclaration = methodDeclaration.WithModifiers(modifiers);
            var newRoot              = root.ReplaceNode(methodDeclaration, newMethodDeclaration);
            var newDocument          = document.WithSyntaxRoot(newRoot);

            return newDocument;

        }

    }

}