namespace Lochs
{
    internal class EnvironmentMap
    {
        private readonly EnvironmentMap _enclosing;
        private readonly Dictionary<string, object> _values = new();

        public EnvironmentMap()
        {
            _enclosing = null;
        }

        public EnvironmentMap(EnvironmentMap enclosing)
        {
            _enclosing = enclosing;
        }

        internal object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            if (_enclosing != null)
            {
                return _enclosing.Get(name);
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        internal void Define(string key, object value)
        {
            _values[key] = value;
        }

        internal void Assign(Token name, object result)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = result;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, result);
                return; 
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
