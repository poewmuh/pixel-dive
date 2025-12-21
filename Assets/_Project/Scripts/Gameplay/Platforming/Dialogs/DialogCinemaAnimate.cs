using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Dialogs
{
    public class DialogCinemaAnimate : MonoBehaviour
    {
        [SerializeField] private Transform _upperBackground;

        [SerializeField] private Transform _bottomBackground;

        [SerializeField] private float _upperShowValue = -1000;
        [SerializeField] private float _bottomShowValue = -1000;
        
        [SerializeField] private float _upperHideValue = 0;
        [SerializeField] private float _bottomHideValue = 0;

        public void ShowCinema()
        {
            _upperBackground.DOLocalMoveY(_upperShowValue, 1);
            _bottomBackground.DOLocalMoveY(_bottomShowValue, 1);
        }

        public void HideCinema()
        {
            _upperBackground.DOLocalMoveY(_upperHideValue, 1);
            _bottomBackground.DOLocalMoveY(_bottomHideValue, 1);
        }
    }
}
