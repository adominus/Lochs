using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace Lochs.Tests
{
    public class EndToEndTests
    {
        [Theory]
        [ClassData(typeof(TernaryTestData))]
        [ClassData(typeof(VariableTestData))]
        [ClassData(typeof(ScopedVariablesTestData))]
        public void Given_expression_then_should_return_expected_output(
            string input, string[] expected, string because)
        {
            // Given
            // When
            var result = Interpret(input);

            // Then
            Debug.WriteLine(because);
            Assert.Equal(expected, result);
        }

        public string[] Interpret(string input)
        {
            using var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            var runner = new LochsRunner();
            runner.RunSource(input);

            var result = stringWriter.GetStringBuilder().ToString();

            return result!.Split(Environment.NewLine)[..^1];
        }

        private class TernaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    "print 1 == 1 ? \"expected\" : \"unexpected\";",
                    new [] { "expected" },
                    "Ternary LHS"
                };

                yield return new object[]
                {
                    "print 1 != 1 ? \"unexpected\" : \"expected\";",
                    new [] { "expected" },
                    "Ternary RHS"
                };

                yield return new object[]
                {
                    "print 1 == 1 ? (2 == 2 ? \"expected\" : \"unexpected\") : \"unexpected\";",
                    new [] { "expected" },
                    "Nested ternary"
                };

                yield return new object[]
                {
                    "print 1 == 1 ? (2 == 2 ? 3 != 3 ? \"unexpected\" : \"expected\" : \"unexpected\") : \"unexpected\";",
                    new [] { "expected" },
                    "Multi nested ternary without grouping"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class VariableTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    @"
var a = 1;
var b = 2;
a = a + 10;
print a + b;
",
                    new [] { "13" },
                    "Assignment and update"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class ScopedVariablesTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    @"
var a = ""global a"";
var b = ""global b"";
var c = ""global c"";
{
var a = ""outer a"";
var b = ""outer b"";
{
var a = ""inner a"";
print a;
print b;
print c;
}
print a;
print b;
print c;
}
print a;
print b;
print c;
",
                    new [] 
                    { 
                        "inner a", "outer b", "global c" ,
                        "outer a", "outer b", "global c" ,
                        "global a", "global b", "global c" 
                    },
                    "Assignment and update"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
