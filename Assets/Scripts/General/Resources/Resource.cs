using UnityEngine;

namespace General.Resources
{
    /// <summary>A collectible resource node (e.g. gold, wood) that pawns can gather from.</summary>
    public class Resource : MonoBehaviour
    {
        public string Id;
        public float Amount;
    }
}
