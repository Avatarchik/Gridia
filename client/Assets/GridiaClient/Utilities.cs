﻿namespace Gridia
{
    using System;

    using UnityEngine;

    public class Utilities
    {
        #region Methods

        public static int CompareAbsoluteValues(float a, float b)
        {
            if (Math.Abs(a - b) < 0.0000001f) return 0;
            if (Mathf.Abs(a) > Mathf.Abs(b)) return 1;
            return -1;
        }

        public static Vector3 GetRelativeDirection(Vector3 from, Vector3 to)
        {
            var heading = to - from;
            return heading / heading.magnitude;
        }

        public static Vector3 RandomDirection()
        {
            var value = UnityEngine.Random.Range(1, 4);
            switch (value)
            {
                case 1: return Vector3.up;
                case 2: return Vector3.down;
                case 3: return Vector3.left;
                case 4: return Vector3.right;
            }
            return Vector3.zero;
        }

        public static Vector2 Vector2FromAngle(double radians)
        {
            var x = (float)Math.Cos(radians);
            var y = (float)Math.Sin(radians);
            return new Vector2(x, y);
        }

        public static bool Vector2IsAbsoluteGreaterThanOne(Vector2 vector)
        {
            return Mathf.Abs(vector.x) >= 1 || Mathf.Abs(vector.y) >= 1;
        }

        public static Vector3 Vector3Absolute(Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector3 Vector3Floor(Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
        }

        public static Vector3 Vector3Residual(Vector3 vector)
        {
            return new Vector3(vector.x % 1, vector.y % 1, vector.z % 1);
        }

        #endregion Methods
    }
}