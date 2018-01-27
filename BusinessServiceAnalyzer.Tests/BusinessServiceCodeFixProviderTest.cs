#region Using Directives

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer.Tests {

    public class BusinessServiceCodeFixProviderTest : CodeFixVerifier {

        public static string VirtualMethodRuleMessageFormat = "Business service method '{0}' has to be virtual.";
        public static string SealedMethodRuleMessageFormat  = "Business service method '{0}' has to be overridable.";

        protected override CodeFixProvider GetCSharpCodeFixProvider() {
            return new BusinessServiceCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
            return new BusinessServiceAnalyzer();
        }

    }

}