#region Using Directives

using System.Linq;

using Microsoft.CodeAnalysis;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer {

    static class BusinessServiceExtensions {

        public static bool IsBusinessServiceClass(this INamedTypeSymbol classSymbol) {

            return classSymbol.TypeKind == TypeKind.Class && 
                   classSymbol.AllBasesTypesAndSelf()
                              .SelectMany(c => c.AllInterfaces)
                              .Any(IsBusinessInterface);

        }

        const string BusinessServiceInterfaceName = "global::Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL.IBusinessService";

        static bool IsBusinessInterface(this INamedTypeSymbol itf) {

            return itf.TypeKind      == TypeKind.Interface &&
                   itf.ToFullyQualifiedFormatDisplayString() == BusinessServiceInterfaceName;
        }

        private const string IgnoreValidationAttributeName = "global::Pharmatechnik.Apotheke.XTplus.Framework.Core.BOL.IgnoreValidationAttribute";

        public static bool IsIgnoreValidationAttributeDefined(this IMethodSymbol method) {
            return method.GetAttributes()
                         .Any(ad => ad.AttributeClass.ToFullyQualifiedFormatDisplayString() == IgnoreValidationAttributeName);
        }

    }

}