using UnityEngine.InputSystem;
using UnityEngine;

namespace _Project.Application.Input
{
    public struct InputActionData
    {
        public InputActionPhase Phase;
    }
    
    public struct InputVectorActionData
    {
        public InputActionPhase Phase;
        public Vector2 Vector;
    }
}