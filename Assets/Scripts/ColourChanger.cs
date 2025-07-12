using Assets.Scripts;
using UnityEngine;

public class ColourChanger : MonoBehaviour
{
    public TileColour Colour;

    private void Awake()
    {
        this.GetComponent<SpriteRenderer>().color = RegisteredColours.GetColor(this.Colour);
    }
}
