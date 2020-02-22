using UnityEngine;

namespace pdxpartyparrot.Core.Math
{
    public static class MathUtil
    {
        // modulus that wraps negative numbers
        // NOTE: this does multiple modulos so only use if negative numbers are a certainty
        public static int WrapMod(int n, int m)
        {
            return (n % m + m) % m;
        }

        public static float WrapMod(float n, float m)
        {
            return n - m * Mathf.Floor(n / m);
        }

        public static float WrapAngle(float angle)
        {
            return angle % 360.0f;
        }

        public static float WrapAngleRad(float angle)
        {
            return angle % (Mathf.PI * 2.0f);
        }

        // uses x / z from the Vector3
        public static int Vector3ToQuadrant(Vector3 v)
        {
            return Vector3ToNtant(v, 4);
        }

        // uses x / z from the Vector3
        public static int Vector3ToOctant(Vector3 v)
        {
            return Vector3ToNtant(v, 8);
        }

        // uses x / z from the Vector3
        public static int Vector3ToNtant(Vector3 v, int n)
        {
            float angle = Mathf.Atan2(v.z, v.x);
            return Mathf.RoundToInt(n * angle / (n * Mathf.PI) + n) % n;
        }
    }
}