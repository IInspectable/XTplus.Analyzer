#region Using Directives

using BusinessServiceAnalyzer.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Pharmatechnik.Apotheke.XTplus.Analyzer.Tests {

    [TestClass]
    public class PublicProperty : BusinessServiceCodeFixProviderTest {
        
        [TestMethod]
        public void Test() {
            var test = @"
            using Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL;

            namespace Pharmatechnik.Apotheke.XTplus.Framework.Core.IBOL {
                interface IBusinessService { }
            }

            interface ITestBS: IBusinessService { }

            class TestBS: ITestBS {
                public string Foo { get {return ""; } }
            }
            ";

            VerifyCSharpDiagnostic(test);           
        }       

    }

}