using System.Collections.Generic;

namespace pdxpartyparrot.Core.Editor
{
    public class ScriptingDefineSymbols
    {
        private readonly HashSet<string> _symbols = new HashSet<string>();

        public IReadOnlyCollection<string> Symbols => _symbols;

        public int Count => Symbols.Count;

        public ScriptingDefineSymbols()
        {
        }

        public ScriptingDefineSymbols(string symbols)
        {
            AddSymbols(symbols);
        }

#region Add
        public void AddSymbol(string symbol)
        {
            _symbols.Add(symbol);
        }

        public void AddSymbols(string symbols)
        {
            AddSymbols(symbols.Split(';'));
        }

        public void AddSymbols(string[] symbols)
        {
            foreach(string symbol in symbols) {
                _symbols.Add(symbol);
            }
        }
#endregion

#region Remove
        public void RemoveSymbol(string symbol)
        {
            _symbols.Remove(symbol);
        }

        public void RemoveSymbols(string symbols)
        {
            RemoveSymbols(symbols.Split(';'));
        }

        public void RemoveSymbols(string[] symbols)
        {
            foreach(string symbol in symbols) {
                RemoveSymbol(symbol);
            }
        }
#endregion

        public bool Contains(string symbol)
        {
            return _symbols.Contains(symbol);
        }

        public override string ToString()
        {
            return string.Join(";", Symbols);
        }
    }
}
