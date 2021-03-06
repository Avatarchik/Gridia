﻿namespace Gridia
{
    using System;

    using UnityEngine;

    public class FloatingText : Label
    {
        #region Constructors

        public FloatingText(Vector3 coord, String text)
            : base(Vector2.zero, text)
        {
            Background = Centered = true;
            Life = 400;
            Coord = coord;
        }

        #endregion Constructors

        #region Properties

        public Vector3 Coord
        {
            get; set;
        }

        public int Life
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void Reposition(float tileSize, Vector3 playerLocation)
        {
            var relative = Coord - playerLocation;
            X = (relative.x + 0.5f) * tileSize;
            Y = Screen.height - (int)((relative.y + 1.5) * tileSize + 100 - Life / 8);
        }

        public void Tick()
        {
            Life--;
        }

        #endregion Methods
    }
}