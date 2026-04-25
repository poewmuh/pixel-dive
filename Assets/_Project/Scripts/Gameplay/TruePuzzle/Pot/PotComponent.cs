using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Gameplay.TruePuzzle.Interactable;
using _Project.Gameplay.TruePuzzle.Inventory;
using MessagePipe;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using UnityEngine;
using VContainer;
using IDropTarget = _Project.Gameplay.TruePuzzle.Inventory.IDropTarget;

namespace _Project.Gameplay.TruePuzzle.Pot
{
    public class PotComponent : MonoBehaviour, IDropTarget
    {
        [SerializeField] private FSMOwner _fsmOwner;
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private SpriteRenderer _potMassSprite;

		[Inject] private IPublisher<OnInteractionCompleteEvent> _onInteractionCompletePub;

        private GarbageFlags _accumulated = GarbageFlags.None;
        private int _potCapacity;
        
        void IDropTarget.OnDrop(IDraggable draggable)
        {
            _potMassSprite.color = Color.darkGreen;
            
            _accumulated |= _toFlags[draggable.InteractableType];
            _potCapacity++;
            Destroy(draggable.Transform.gameObject);

            if (_potCapacity < 2) return;
            
            _potMassSprite.color = Color.skyBlue;
            var combined = _toType[_accumulated];
            _blackboard.SetVariableValue("Combined", combined);
            _fsmOwner.SendEvent("PotGenerated");
            _onInteractionCompletePub.Publish(new OnInteractionCompleteEvent(combined, transform));
            
            _accumulated = GarbageFlags.None;
            _potCapacity = 0;
        }

        bool IDropTarget.CanDrop(IDraggable draggable)
        {
            return _toFlags.ContainsKey(draggable.InteractableType);
        }
        
        [Flags]
        private enum GarbageFlags { None=0, Fish=1, Paket=2, Bottle=4, Cigarette=8 }
        
        private static readonly Dictionary<InteractableType, GarbageFlags> _toFlags = new()
        {
            { InteractableType.FishGarbage,      GarbageFlags.Fish },
            { InteractableType.PaketGarbage,     GarbageFlags.Paket },
            { InteractableType.BottleGarbage,    GarbageFlags.Bottle },
            { InteractableType.CigaretteGarbage, GarbageFlags.Cigarette },
            
            { InteractableType.FishPaket,        GarbageFlags.Fish | GarbageFlags.Paket },
            { InteractableType.FishBottle,       GarbageFlags.Fish | GarbageFlags.Bottle },
            { InteractableType.FishCigarette,    GarbageFlags.Fish | GarbageFlags.Cigarette },
            { InteractableType.PaketBottle,    GarbageFlags.Paket | GarbageFlags.Bottle },
            { InteractableType.PaketCigarette,    GarbageFlags.Paket | GarbageFlags.Cigarette },
            { InteractableType.BottleCigarette,    GarbageFlags.Bottle | GarbageFlags.Cigarette },
            
            { InteractableType.FishPaketBottle,    GarbageFlags.Fish | GarbageFlags.Paket | GarbageFlags.Bottle },
            { InteractableType.FishBottleCigarette,    GarbageFlags.Fish | GarbageFlags.Cigarette | GarbageFlags.Bottle },
            { InteractableType.FishPaketCigarette,    GarbageFlags.Fish | GarbageFlags.Cigarette | GarbageFlags.Paket },
            { InteractableType.PaketBottleCigarette,    GarbageFlags.Bottle | GarbageFlags.Cigarette | GarbageFlags.Paket },
            { InteractableType.TubeKey,    GarbageFlags.Bottle | GarbageFlags.Cigarette | GarbageFlags.Paket | GarbageFlags.Fish },
        };
        
        private static readonly Dictionary<GarbageFlags, InteractableType> _toType =
            _toFlags.ToDictionary(kv => kv.Value, kv => kv.Key);
    }
}