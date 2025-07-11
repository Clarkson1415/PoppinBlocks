using Assets.Scripts;
using UnityEngine;

public class ColourChanger : MonoBehaviour
{
    public TileColour Colour;

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().color = RegisteredColours.GetColor(this.Colour);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ColouredUnit>(out var toColourThis) && toColourThis.ThisGuysColour != this.Colour)
        {
            toColourThis.GetComponent<ColouredUnit>().ChangeColour(Colour);
        }
    }
}
