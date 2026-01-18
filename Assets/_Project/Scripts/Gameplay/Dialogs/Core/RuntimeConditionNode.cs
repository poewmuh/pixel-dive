using System;
using System.Collections.Generic;

namespace _Project.Gameplay.Dialogs.Core
{
    [Serializable]
    public class RuntimeConditionNode : RuntimeBaseNode
    {
        public string VariableName;
        public ComparisonType ComparisonType;
        public int CompareValue;
        
        public string TrueNodeId;
        public string FalseNodeId;
        
        public bool Evaluate(RuntimeVariableStorage variables)
        {
            int value = variables.GetVariable<int>(VariableName, 0);
            
            return ComparisonType switch
            {
                ComparisonType.Equal => value == CompareValue,
                ComparisonType.NotEqual => value != CompareValue,
                ComparisonType.Greater => value > CompareValue,
                ComparisonType.GreaterOrEqual => value >= CompareValue,
                ComparisonType.Less => value < CompareValue,
                ComparisonType.LessOrEqual => value <= CompareValue,
                _ => false
            };
        }
    }
    
    [Serializable]
    public class RuntimeRandomNode : RuntimeBaseNode
    {
        public List<string> NextNodeIds = new();
    }
}