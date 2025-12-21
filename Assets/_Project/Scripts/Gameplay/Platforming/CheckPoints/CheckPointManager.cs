using UnityEngine;

namespace _Project.Gameplay.Platforming.CheckPoints
{
    public class CheckPointManager : MonoBehaviour
    {
        [SerializeField] private Transform _checkPoint;
        
        public Vector3 GetCheckPointPos() => _checkPoint.position;
    }
}