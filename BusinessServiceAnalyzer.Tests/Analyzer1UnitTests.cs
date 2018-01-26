using System;

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pharmatechnik.Apotheke.XTplus.Analyzer;

namespace BusinessServiceAnalyzer.Tests {

    [TestClass]
    public class UnitTest: CodeFixVerifier {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1() {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
       // TODO [TestMethod]
        public void TestMethod2() {
            var test      = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";
            var expected  = new DiagnosticResult {
                Id        = "Analyzer1",
                Message   = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity  = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 11, 15)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
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