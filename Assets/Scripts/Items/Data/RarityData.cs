using UnityEngine;

namespace Assets.Scripts.Items.Data
{
    [CreateAssetMenu(fileName = "New Rarity", menuName = "Game/Items/Rarity")]
    public class RarityData : ScriptableObject
    {
        [SerializeField] private new string name = "New Rarity Name";
        [SerializeField] private Color textColor = Color.white;

        public string Name => name;
        public Color TextColor => textColor;
    }
}
