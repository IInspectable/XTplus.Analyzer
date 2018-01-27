#region Using Directives

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer.Tests {

    [TestClass]
    public class EmptyFile : BusinessServiceCodeFixProviderTest {

        public static string VirtualMethodDiagnosticMessageFormat = "Business service method '{0}' has to be virtual.";

        [TestMethod]
        public void Trst() {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

    }

}