using System;

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pharmatechnik.Apotheke.XTplus.Analyzer;

namespace BusinessServiceAnalyzer.Tests {

    [TestClass]
    public class BusinessServiceAnalyzerUnitTest : CodeFixVerifier {

        [TestMethod]
        public void TrivialTest() {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

       [TestMethod]
        public void PublicMethodHasToBeVirtual() {
            var test      = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { }
            }

            interface ITestBS: IBusinessService { }

            class TestBS: ITestBS {
                public void Foo();
            }
            ";
           string m = "void TestBS.Foo()";
            var expected  = new DiagnosticResult {
                Id        = Pharmatechnik.Apotheke.XTplus.Analyzer.BusinessServiceAnalyzer.VirtualMethodDiagnosticId,
                Message   = $"Business service method '{m}' has to be virtual.",
                Severity  = DiagnosticSeverity.Error,
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
                public virtual void Foo();
            }
            ";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider() {
            return new BusinessServiceCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
            return new Pharmatechnik.Apotheke.XTplus.Analyzer.BusinessServiceAnalyzer();
        }

    }

}