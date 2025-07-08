using UnityEngine;

namespace Assets.Scripts
{
    public class GameColour : MonoBehaviour
    {
        public TileColour ThisGuysColour;

        private void Start()
        {
            this.GetComponent<SpriteRenderer>().color = RegisteredColours.GetColor(this.ThisGuysColour);
        }
    }
}
