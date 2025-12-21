using UnityEngine;

namespace _Project.Gameplay
{
    public class ComponentInitializer : MonoBehaviour
    {
        [SerializeField] private bool _initializeOnStart = true;

        private void Start()
        {
            if (_initializeOnStart) 
                Initialize();
        }

        private void Initialize()
        {
            InitComponents();
        }

        private void InitComponents()
        {
            var components = GetComponents<IComponent>();
            foreach (var component in components)
            {
                component.Init();
            }
        }
    }
}
