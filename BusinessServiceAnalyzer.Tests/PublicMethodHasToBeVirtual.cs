#region Using Directives

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer.Tests {

    [TestClass]
    public class PublicMethodHasToBeVirtual : BusinessServiceCodeFixProviderTest {
        
        [TestMethod]
        public void Test() {
            var test = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { }
            }

            interface ITestBS: IBusinessService { }

            class TestBS: ITestBS {
                public void Foo() { }
            }
            ";

            var expected = new DiagnosticResult {
                Id = BusinessServiceAnalyzer.VirtualMethodDiagnosticId,
                Message = string.Format(VirtualMethodRuleMessageFormat, "void TestBS.Foo()"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 11, 29)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { }
            }

            interface ITestBS: IBusinessService { }

            class TestBS: ITestBS {
                public virtual void Foo() { }
            }
            ";

            VerifyCSharpFix(test, fixtest);
        }       

    }

}