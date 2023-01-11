namespace Lochs
{
    internal class EnvironmentMap
    {
        private readonly Dictionary<string, object> _values = new(); 

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Define(string key, object value)
        {
            _values[key] = value;
        }
    }
}
