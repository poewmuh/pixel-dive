using UnityEngine;
using UnityEngine.UI;

namespace _Project.Gameplay.TruePuzzle.TubeGame
{
    public class TubeCellView : MonoBehaviour
    {
        public int GridX;
        public int GridY;
        public bool IsRoot;
        
        private float _correctRotation;
        
        public void Init(TubeGameWindow game)
        {
            GetComponent<Button>().onClick.AddListener(() => game.OnCellClicked(this));
            _correctRotation = transform.rotation.eulerAngles.z;
        }

        public void UpdateVisual()
        {
            if (IsRoot) return;
            transform.Rotate(new Vector3(0, 0, -90));
        }

        public bool IsCorrect()
        {
            return Mathf.Approximately(_correctRotation, transform.rotation.eulerAngles.z);
        }
    }

    public enum TubeType
    {
        I = 0,
        L = 1,
        T = 2
    }
}