using TriInspector;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    [CreateAssetMenu(menuName = "_Project/Gameplay/Platforming/MovementConfig", fileName = nameof(MovementConfig))]
    [DeclareBoxGroup("moveBox", Title = "Movement Settings")]
    [DeclareBoxGroup("jumpBox", Title = "Jump Settings")]
    public class MovementConfig : ScriptableObject
    {
        [field: SerializeField, Group("moveBox")]
        public float MoveSpeed { get; private set; }
        [field: SerializeField, Group("moveBox")]
        public float Acceleration { get; private set; }
        [field: SerializeField, Group("moveBox")]
        public float Deceleration { get; private set; }
        
        [field: SerializeField, Group("jumpBox")]
        public float MaxJumpForce { get; private set; }
        [field: SerializeField, Group("jumpBox")]
        public float MinJumpForce { get; private set; }
        [field: SerializeField, Group("jumpBox")]
        public float ChargeTime { get; private set; }
        [field: SerializeField, Group("jumpBox")]
        public float AirAcceleration { get; private set; }
        [field: SerializeField, Group("jumpBox")]
        public float AirDeceleration { get; private set; }
        [field: SerializeField, Group("jumpBox")]
        public LayerMask GroundLayer { get; private set; }
    }
}
