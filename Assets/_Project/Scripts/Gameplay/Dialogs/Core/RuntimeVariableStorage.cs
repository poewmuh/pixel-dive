using System.Collections.Generic;

namespace _Project.Gameplay.Dialogs.Core
{
    public class RuntimeVariableStorage
    {
        private Dictionary<string, object> _variables = new();

        public void SetVariable<T>(string key, T value)
        {
            _variables[key] = value;
        }

        public T GetVariable<T>(string key, T defaultValue = default)
        {
            if (_variables.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        public bool HasVariable(string key)
        {
            return _variables.ContainsKey(key);
        }

        public void Clear()
        {
            _variables.Clear();
        }
        
        public void IncrementInt(string key)
        {
            int current = GetVariable<int>(key, 0);
            SetVariable(key, current + 1);
        }
        
        public void DecrementInt(string key)
        {
            int current = GetVariable<int>(key, 0);
            SetVariable(key, current + 1);
        }
    }
}