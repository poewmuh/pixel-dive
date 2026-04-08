using _Project.Gameplay.TruePuzzle.Interactable;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    [CreateAssetMenu(fileName = nameof(InventoySpritesConfig), menuName = "_Project/Inventory/" + nameof(InventoySpritesConfig))]
    public class InventoySpritesConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<InteractableType, Sprite> _inventoryItemSprites;

        public Sprite GetInventoryItemSprite(InteractableType interactableType)
        {
            return _inventoryItemSprites[interactableType];
        }
    }
}