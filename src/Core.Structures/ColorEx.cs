﻿namespace Core.Structures
{
    using UnityEngine;

    public static class ColorEx
    {
        public static Color WithAlpha(this Color c, float a) => new Color(c.r, c.g, c.b, a);
    }
}