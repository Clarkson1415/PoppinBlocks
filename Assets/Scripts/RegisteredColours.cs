using UnityEngine;

namespace Assets.Scripts
{
    public static class RegisteredColours
    {
        public static readonly Color PastelBlue = new Color(0.5f, 0.7f, 1f);
        public static readonly Color PastelGreen = new Color(0.5f, 1f, 0.7f);
        public static readonly Color PastelRed = new Color(1f, 0.7f, 0.7f);
        public static readonly Color PastelYellow = new Color(1f, 1f, 0.7f);

        public static Color GetColor(TileColour colorType)
        {
            return colorType switch
            {
                TileColour.Blue => PastelBlue,
                TileColour.Green => PastelGreen,
                TileColour.Red => PastelRed,
                _ => Color.white,
            };
        }
    }
}
