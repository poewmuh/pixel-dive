using System.Collections.Generic;
using _Project.Gameplay.Platforming.Damage;
using R3;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.Platforming.CheckPoints
{
    public class CheckPointComponent : MonoBehaviour, IComponent
    {
        [Inject] private CheckPointManager _checkPointManager;
        
        private IKillable _killable;

        void IComponent.Init()
        {
            _killable = GetComponent<IKillable>();
            _killable.IsDead.Subscribe(OnDead).AddTo(this);
        }

        private void OnDead(bool isDead)
        {
            if (!isDead) return;

            transform.position = _checkPointManager.GetCheckPointPos();
        }
    }
}
