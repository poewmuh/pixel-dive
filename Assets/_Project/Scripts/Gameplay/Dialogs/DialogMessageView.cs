using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Gameplay.Dialogs
{
    public class DialogMessageView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _speakerName;
        [SerializeField] private TextMeshProUGUI _dialogMessage;
        [SerializeField] private Image _speakerIcon;

        public void Setup(string speakerName, string dialogMessage, Sprite speakerSprite)
        {
            _speakerName.text = speakerName;
            _dialogMessage.text = dialogMessage;
            _speakerIcon.sprite = speakerSprite;
        }
    }
}