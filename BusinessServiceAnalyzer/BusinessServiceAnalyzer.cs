#region Using Directives

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BusinessServiceAnalyzer: DiagnosticAnalyzer {

        private const string Category = "IXOS BusinessService";

        public static readonly string VirtualMethodDiagnosticId = "BS001";
        public static readonly string SealedMethodDiagnosticId  = "BS002";

        static readonly DiagnosticDescriptor VirtualMethodRule = new DiagnosticDescriptor(
            id                : VirtualMethodDiagnosticId,
            title             : "Business service methods",
            messageFormat     : "Business service method '{0}' has to be virtual.",
            category          : Category, 
            defaultSeverity   : DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description       : "Methods implementing a member of a business service have to be virtual.");

        static readonly DiagnosticDescriptor SealedMethodRule = new DiagnosticDescriptor(
            id                : SealedMethodDiagnosticId,
            title             : "Business service methods",
            messageFormat     : "Business service method '{0}' has to be overridable.",
            category          : Category,
            defaultSeverity   : DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description       : "Methods implementing a member of a business service have to be overridable.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(VirtualMethodRule, SealedMethodRule);

        public override void Initialize(AnalysisContext context) {

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context) {
            var method = (IMethodSymbol) context.Symbol;

            // Nur "ordinäre" Methoden analysieren, d.h. keine Ctors, Properties etc.
            if (method.MethodKind != MethodKind.Ordinary) {
                return;
            }

            var classSymbol = method.ContainingType;
            // Die Klasse ist kein BusinessService
            if (!classSymbol.IsBusinessServiceClass()) {
                return;
            }

            // Nicht öffentliche oder statische Methoden sind ok.
            if (!method.IsDeclaredPublic() ||
                method.IsStatic            ||
                method.IsIgnoreValidationAttributeDefined()) {
                return;
            }

            // Die Methode ist überschreibbar => so soll es sein
            if (method.IsOverridable()) {
                return;
            }

            foreach (var location in context.Symbol.Locations) {

                if (method.IsSealed) {
                    // Methode darf nicht versiegelt sein
                    context.ReportDiagnostic(
                        Diagnostic.Create(SealedMethodRule,
                                          location,
                                          method.ToMinimallyQualifiedDisplayString()));
                } else {
                    // Es fehlt wohl das virtual Keyword
                    context.ReportDiagnostic(
                        Diagnostic.Create(VirtualMethodRule,
                                          location,
                                          method.ToMinimallyQualifiedDisplayString()));
                }
            }

        }

    }

}