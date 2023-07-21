using System;

namespace UnityEngine
{
    //
    // 摘要:
    //     Representation of four-dimensional vectors.
    public struct Vector4
    {
        public const float kEpsilon = 1E-05f;

        //
        // 摘要:
        //     X component of the vector.
        public float x;

        //
        // 摘要:
        //     Y component of the vector.
        public float y;

        //
        // 摘要:
        //     Z component of the vector.
        public float z;

        //
        // 摘要:
        //     W component of the vector.
        public float w;

        private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);

        private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

        private static readonly Vector4 positiveInfinityVector = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        private static readonly Vector4 negativeInfinityVector = new Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        //
        // 摘要:
        //     Returns this vector with a magnitude of 1 (Read Only).
        public Vector4 normalized => Normalize(this);

        //
        // 摘要:
        //     Returns the length of this vector (Read Only).
        public float magnitude => Mathf.Sqrt(Dot(this, this));

        //
        // 摘要:
        //     Returns the squared length of this vector (Read Only).
        public float sqrMagnitude => Dot(this, this);

        //
        // 摘要:
        //     Shorthand for writing Vector4(0,0,0,0).
        public static Vector4 zero => zeroVector;

        //
        // 摘要:
        //     Shorthand for writing Vector4(1,1,1,1).
        public static Vector4 one => oneVector;

        //
        // 摘要:
        //     Shorthand for writing Vector4(float.PositiveInfinity, float.PositiveInfinity,
        //     float.PositiveInfinity, float.PositiveInfinity).
        public static Vector4 positiveInfinity => positiveInfinityVector;

        //
        // 摘要:
        //     Shorthand for writing Vector4(float.NegativeInfinity, float.NegativeInfinity,
        //     float.NegativeInfinity, float.NegativeInfinity).
        public static Vector4 negativeInfinity => negativeInfinityVector;

        //
        // 摘要:
        //     Creates a new vector with given x, y, z, w components.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   z:
        //
        //   w:
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        //
        // 摘要:
        //     Creates a new vector with given x, y, z components and sets w to zero.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   z:
        public Vector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0f;
        }

        //
        // 摘要:
        //     Creates a new vector with given x, y components and sets z and w to zero.
        //
        // 参数:
        //   x:
        //
        //   y:
        public Vector4(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
            w = 0f;
        }

        //
        // 摘要:
        //     Set x, y, z and w components of an existing Vector4.
        //
        // 参数:
        //   newX:
        //
        //   newY:
        //
        //   newZ:
        //
        //   newW:
        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        //
        // 摘要:
        //     Linearly interpolates between two vectors.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        //
        // 摘要:
        //     Linearly interpolates between two vectors.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        //
        // 摘要:
        //     Moves a point current towards target.
        //
        // 参数:
        //   current:
        //
        //   target:
        //
        //   maxDistanceDelta:
        public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            Vector4 vector = target - current;
            float num = vector.magnitude;
            if (num <= maxDistanceDelta || num == 0f)
            {
                return target;
            }

            return current + vector / num * maxDistanceDelta;
        }

        //
        // 摘要:
        //     Multiplies two vectors component-wise.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        //
        // 摘要:
        //     Multiplies every component of this vector by the same component of scale.
        //
        // 参数:
        //   scale:
        public void Scale(Vector4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        //
        // 摘要:
        //     Returns true if the given vector is exactly equal to this vector.
        //
        // 参数:
        //   other:
        public override bool Equals(object other)
        {
            if (!(other is Vector4))
            {
                return false;
            }

            Vector4 vector = (Vector4)other;
            return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
        }

        //
        // 参数:
        //   a:
        public static Vector4 Normalize(Vector4 a)
        {
            float num = Magnitude(a);
            if (num > 1E-05f)
            {
                return a / num;
            }

            return zero;
        }

        //
        // 摘要:
        //     Makes this vector have a magnitude of 1.
        public void Normalize()
        {
            float num = Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = zero;
            }
        }

        //
        // 摘要:
        //     Dot Product of two vectors.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        //
        // 摘要:
        //     Projects a vector onto another vector.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static Vector4 Project(Vector4 a, Vector4 b)
        {
            return b * Dot(a, b) / Dot(b, b);
        }

        //
        // 摘要:
        //     Returns the distance between a and b.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static float Distance(Vector4 a, Vector4 b)
        {
            return Magnitude(a - b);
        }

        public static float Magnitude(Vector4 a)
        {
            return Mathf.Sqrt(Dot(a, a));
        }

        //
        // 摘要:
        //     Returns a vector that is made from the smallest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
        }

        //
        // 摘要:
        //     Returns a vector that is made from the largest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4 operator -(Vector4 a)
        {
            return new Vector4(0f - a.x, 0f - a.y, 0f - a.z, 0f - a.w);
        }

        public static Vector4 operator *(Vector4 a, float d)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator *(float d, Vector4 a)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator /(Vector4 a, float d)
        {
            return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 0f);
        }

        public static implicit operator Vector3(Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector4(Vector2 v)
        {
            return new Vector4(v.x, v.y, 0f, 0f);
        }

        public static implicit operator Vector2(Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
        }

        public static float SqrMagnitude(Vector4 a)
        {
            return Dot(a, a);
        }

        public float SqrMagnitude()
        {
            return Dot(this, this);
        }
    }
}