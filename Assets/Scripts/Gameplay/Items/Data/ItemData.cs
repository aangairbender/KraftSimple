using UnityEngine;

namespace Assets.Scripts.Items.Data
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Game/Items/Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private new string name = "New Item Name";
        [SerializeField] private string description = "Description";
        [SerializeField] private RarityData rarity;
        [SerializeField] [Min(1)] private int stackLimit = 1;
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefab;

        public string Name => name;
        public string Description => description;
        public RarityData Rarity => rarity;
        public int StackLimit => stackLimit;
        public Sprite Sprite => sprite;
        public GameObject Prefab => prefab;
    }
}
