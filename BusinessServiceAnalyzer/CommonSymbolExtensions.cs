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
       
        public static string ToMinimallyQualifiedDisplayString(this IMethodSymbol symbol) {
            return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        public static string ToFullyQualifiedFormatDisplayString(this INamedTypeSymbol symbol) {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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