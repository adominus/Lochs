using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Lochs.Tests
{
    public class EndToEndTests
    {
        [Theory, ClassData(typeof(TernaryTestData))]
        public void Given_expression_then_should_return_expected_output(
            string input, object expected, string because)
        {
            // Given
            // When
            var result = Interpret(input);

            // Then
            Debug.WriteLine(because);
            Assert.Equal(expected, result);
        }

        public object Interpret(string input)
        {
            var errorReporter = new ErrorReporter();
            var scanner = new Scanner(input, errorReporter);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens, errorReporter);
            var expression = parser.Parse();
            var interpreter = new Interpreter(errorReporter);

            return interpreter.InterpretAndStringify(expression);
        }

        private class TernaryTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    "1 == 1 ? \"expected\" : \"unexpected\"",
                    "expected",
                    "Ternary LHS"
                };

                yield return new object[]
                {
                    "1 != 1 ? \"unexpected\" : \"expected\"",
                    "expected",
                    "Ternary RHS"
                };

                yield return new object[]
                {
                    "1 == 1 ? (2 == 2 ? \"expected\" : \"unexpected\") : \"unexpected\"",
                    "expected",
                    "Nested ternary"
                };

                yield return new object[]
                {
                    "1 == 1 ? (2 == 2 ? 3 != 3 ? \"unexpected\" : \"expected\" : \"unexpected\") : \"unexpected\"",
                    "expected",
                    "Multi nested ternary without grouping"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
