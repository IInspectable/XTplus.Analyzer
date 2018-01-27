#region Using Directives

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer.Tests {

    [TestClass]
    public class PublicMethodMustNotBeSealed : BusinessServiceCodeFixProviderTest {
        
        [TestMethod]
        public void Test() {
            var test = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { 
                    void Foo();
                }
            }

            interface ITestBS: IBusinessService { }

            class TestBS: ITestBS {
                public virtual sealed void Foo() { }
            }
            ";

            var expected = new DiagnosticResult {
                Id = BusinessServiceAnalyzer.SealedMethodDiagnosticId,
                Message = string.Format(SealedMethodRuleMessageFormat, "void TestBS.Foo()"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 13, 44)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { 
                    void Foo();
                }
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