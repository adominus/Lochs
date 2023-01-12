namespace Lochs
{
    internal class EnvironmentMap
    {
        private readonly Dictionary<string, object> _values = new(); 

        internal object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        internal void Define(string key, object value)
        {
            _values[key] = value;
        }

        internal void Assign(Token name, object result)
        {
            if (!_values.ContainsKey(name.Lexeme))
            {
                throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }

            _values[name.Lexeme] = result;
        }
    }
}
