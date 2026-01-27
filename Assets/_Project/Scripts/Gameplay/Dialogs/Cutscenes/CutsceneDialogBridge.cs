using _Project.Application.GUI.DialogeSystem;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.Dialogs.Cutscenes
{
    public class CutsceneDialogBridge : MonoBehaviour
    {
        [Inject] private IDialogeSystem _dialogSystem;

        public IDialogeSystem DialogSystem => _dialogSystem;
    }
}