using UnityEngine;

namespace _Project.Application.GUI
{
    public class CanvasBehaviour : MonoBehaviour
    {
        public RectTransform CreateContainer(string containerName)
        {
            var container = new GameObject(containerName).AddComponent<RectTransform>();
            container.SetParent(transform);
            container.localScale = Vector3.one;
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = Vector2.zero;
            container.offsetMax = Vector2.zero;
            return container;
        }
    }
}
