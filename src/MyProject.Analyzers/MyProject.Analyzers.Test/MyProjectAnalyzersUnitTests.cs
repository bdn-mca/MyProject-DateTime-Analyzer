using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyProject.Analyzers;
using System;
using TestHelper;

namespace MyProject.Analyzers.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void RunAnalyzer_OnEmptyCode_MarksNoErrors()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public void RunAnalyzer_OnCodeWithNoDateTime_MarksNoErrors()
        {
            var test = @"
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
            int testInt = 5;
            string testString = null;
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public void RunAnalyzer_DateTimeInUsingStatement_MarksNoErrors()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Some.Namespace.DateTime.InIt;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            int testInt = 5;
            string testString = null;
        }
    }

    namespace Some.Namespace.DateTime.InIt
    {
        class NewTypeName
        {
            int testIntNew = 5;
            string testStringNew = null;
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_DateTimeAsProperty_MarksNoErrors()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Some.Namespace.DateTime.InIt;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            DateTime date { get; set; }
            DateTime? dateNullable { get; set; }
            int testInt = 5;
            string testString = null;
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_DateTimeInConstructor_MarksNoErrors()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Some.Namespace.DateTime.InIt;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public TypeName(DateTime date)
            {
            }

            public TypeNameNullable(DateTime? date)
            {
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_DateTimeAsVariableWithoutDeclaration_MarksNoErrors()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Some.Namespace.DateTime.InIt;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            DateTime date;
            DateTime? dateNullable;
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_DateTimeInMethodProperty_MarksNoErrors()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            int Date(DateTime date, DateTime? dateNullable)
            {
                return 5;
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_CastToDateTime_MarksNoErrors()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public void CastObject(object value)
            {
                var date = (DateTime)value;
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_CastToDateTimeWithAs_MarksNoErrors()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public void CastObject(object value)
            {
                var date = value as DateTime?;
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_TypeOfDateTime_MarksNoErrors()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public void CastObject(object value)
            {
                var date = typeof(DateTime);
                var dateNullable = typeof(DateTime?);
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_GenericClassTypeDateTime_MarksNoErrors()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public void CastObject(object value)
            {
                var date = new Result<DateTime, int>();
                var dateNullable = new Result<DateTime?, decimal>();
            }
        }

        public class Result<T1, T2>
        {
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void RunAnalyzer_StaticDateTimeField_MarksDeclarationAsError_ButNotTheField()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            public static readonly DateTime date = new DateTime(2010, 1, 1);
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = "MyProjectAnalyzer",
                Message = String.Format("Type name '{0}' should not be used in MyProject.", "DateTime"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 56)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void RunAnalyzer_DateTimeAsReturnInProperty_MarksDateTimeAsError()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
            DateTime Date
            {
                get
                {
                    return DateTime.Now;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = "MyProjectAnalyzer",
                Message = String.Format("Type name '{0}' should not be used in MyProject.", "DateTime"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 28)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void RunAnalyzer_OnCodeWithDateTimeNow_MarksDateTimeAsError_FixesToMyProjectDateTime()
        {
            var test = @"
    using System;
    using MyProject.Analyzers.Test.Helpers;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            var dateTime = DateTime.Now;
        }
    }

    namespace MyProject.Analyzers.Test.Helpers
    {
        public static class MyProjectDateTime
        {
            public static int Now = 5;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MyProjectAnalyzer",
                Message = String.Format("Type name '{0}' should not be used in MyProject.", "DateTime"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 28)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using MyProject.Analyzers.Test.Helpers;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            var dateTime = MyProjectDateTime.Now;
        }
    }

    namespace MyProject.Analyzers.Test.Helpers
    {
        public static class MyProjectDateTime
        {
            public static int Now = 5;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void RunAnalyzer_OnCodeWithDateTimeCtor_MarksDateTimeAsError_FixesToMyProjectDateTime()
        {
            var test = @"
    using System;
    using MyProject.Analyzers.Test.Helpers;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            var dateTime = new DateTime(2018, 1, 1);
        }
    }

    namespace MyProject.Analyzers.Test.Helpers
    {
        public class MyProjectDateTime
        {
            public MyProjectDateTime(int year, int month, int day)
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MyProjectAnalyzer",
                Message = String.Format("Type name '{0}' should not be used in MyProject.", "DateTime"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 32)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using MyProject.Analyzers.Test.Helpers;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            var dateTime = new MyProjectDateTime(2018, 1, 1);
        }
    }

    namespace MyProject.Analyzers.Test.Helpers
    {
        public class MyProjectDateTime
        {
            public MyProjectDateTime(int year, int month, int day)
            {
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MyProjectAnalyzersCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MyProjectAnalyzersAnalyzer();
        }
    }
}
