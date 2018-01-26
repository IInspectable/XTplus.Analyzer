#region Using Directives

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer {

    static class CommonSymbolExtensions {

        public static bool IsOverridable(this ISymbol symbol) {
            // Members can only have overrides if they are virtual, abstract or override and is not
            // sealed.
            return symbol?.ContainingType?.TypeKind == TypeKind.Class           &&
                   (symbol.IsVirtual || symbol.IsAbstract || symbol.IsOverride) &&
                   !symbol.IsSealed;
        }

        public static bool IsDeclaredPublic(this ISymbol symbol) {
            return symbol.DeclaredAccessibility == Accessibility.Public;
        }

        public static string GetFullNamespace(this ISymbol symbol) {
            if (string.IsNullOrEmpty(symbol.ContainingNamespace?.Name)) {
                return null;
            }

            // get the rest of the full namespace string
            string restOfResult = symbol.ContainingNamespace.GetFullNamespace();

            string result = symbol.ContainingNamespace.Name;

            if (restOfResult != null)
                // if restOfResult is not null, append it after a period
                result = restOfResult + '.' + result;

            return result;
        }

        public static string ToMinimallyQualifiedDisplayString(this IMethodSymbol symbol) {
            return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        public static string ToFullyQualifiedFormatDisplayString(this INamedTypeSymbol symbol) {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string GetFullTypeString(this INamedTypeSymbol type) {
            string result = type.Name;

            if (!type.TypeArguments.Any()) {
                return result;
            }

            result += "<";

            bool isFirstIteration = true;
            foreach (var typeArg in type.TypeArguments.OfType<INamedTypeSymbol>()) {
                if (isFirstIteration) {
                    isFirstIteration = false;
                } else {
                    result += ", ";
                }

                result += typeArg.GetFullTypeString();
            }

            result += ">";

            return result;
        }

        public static ImmutableArray<INamedTypeSymbol> AllBasesTypesAndSelf(this INamedTypeSymbol classSymbol) {

            if (classSymbol.TypeKind != TypeKind.Class) {
                return Enumerable.Empty<INamedTypeSymbol>().ToImmutableArray();
            }

            return AllBasesTypesAndSelfImpl(classSymbol).Distinct().ToImmutableArray();

            IEnumerable<INamedTypeSymbol> AllBasesTypesAndSelfImpl(INamedTypeSymbol currentClass) {
                if (currentClass == null) {
                    yield break;
                }

                yield return currentClass;

                foreach (var baseClass in AllBasesTypesAndSelfImpl(currentClass.BaseType)) {
                    yield return baseClass;
                }
            }
        }

    }

}