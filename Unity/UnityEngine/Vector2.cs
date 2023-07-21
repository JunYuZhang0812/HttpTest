using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    //
    // 摘要:
    //     Representation of 2D vectors and points.
    public struct Vector2
    {
        //
        // 摘要:
        //     X component of the vector.
        public float x;

        //
        // 摘要:
        //     Y component of the vector.
        public float y;

        private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

        private static readonly Vector2 oneVector = new Vector2(1f, 1f);

        private static readonly Vector2 upVector = new Vector2(0f, 1f);

        private static readonly Vector2 downVector = new Vector2(0f, -1f);

        private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

        private static readonly Vector2 rightVector = new Vector2(1f, 0f);

        private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        public const float kEpsilon = 1E-05f;

        //
        // 摘要:
        //     Returns this vector with a magnitude of 1 (Read Only).
        public Vector2 normalized
        {
            get
            {
                Vector2 result = new Vector2(x, y);
                result.Normalize();
                return result;
            }
        }

        //
        // 摘要:
        //     Returns the length of this vector (Read Only).
        public float magnitude => Mathf.Sqrt(x * x + y * y);

        //
        // 摘要:
        //     Returns the squared length of this vector (Read Only).
        public float sqrMagnitude => x * x + y * y;

        //
        // 摘要:
        //     Shorthand for writing Vector2(0, 0).
        public static Vector2 zero => zeroVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(1, 1).
        public static Vector2 one => oneVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(0, 1).
        public static Vector2 up => upVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(0, -1).
        public static Vector2 down => downVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(-1, 0).
        public static Vector2 left => leftVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(1, 0).
        public static Vector2 right => rightVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(float.PositiveInfinity, float.PositiveInfinity).
        public static Vector2 positiveInfinity => positiveInfinityVector;

        //
        // 摘要:
        //     Shorthand for writing Vector2(float.NegativeInfinity, float.NegativeInfinity).
        public static Vector2 negativeInfinity => negativeInfinityVector;

        //
        // 摘要:
        //     Constructs a new vector with given x, y components.
        //
        // 参数:
        //   x:
        //
        //   y:
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        //
        // 摘要:
        //     Set x and y components of an existing Vector2.
        //
        // 参数:
        //   newX:
        //
        //   newY:
        public void Set(float newX, float newY)
        {
            x = newX;
            y = newY;
        }

        //
        // 摘要:
        //     Linearly interpolates between vectors a and b by t.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        //
        // 摘要:
        //     Linearly interpolates between vectors a and b by t.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
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
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 vector = target - current;
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
        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        //
        // 摘要:
        //     Multiplies every component of this vector by the same component of scale.
        //
        // 参数:
        //   scale:
        public void Scale(Vector2 scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        //
        // 摘要:
        //     Makes this vector have a magnitude of 1.
        public void Normalize()
        {
            float num = magnitude;
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
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1})", x, y);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public string ToString(string format)
        {
            return string.Format("({0}, {1})", x.ToString(format), y.ToString(format));
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        //
        // 摘要:
        //     Returns true if the given vector is exactly equal to this vector.
        //
        // 参数:
        //   other:
        public override bool Equals(object other)
        {
            if (!(other is Vector2))
            {
                return false;
            }

            Vector2 vector = (Vector2)other;
            return x.Equals(vector.x) && y.Equals(vector.y);
        }

        //
        // 摘要:
        //     Reflects a vector off the vector defined by a normal.
        //
        // 参数:
        //   inDirection:
        //
        //   inNormal:
        public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
        {
            return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        //
        // 摘要:
        //     Dot Product of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        //
        // 摘要:
        //     Returns the unsigned angle in degrees between from and to.
        //
        // 参数:
        //   from:
        //     The vector from which the angular difference is measured.
        //
        //   to:
        //     The vector to which the angular difference is measured.
        public static float Angle(Vector2 from, Vector2 to)
        {
            return Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
        }

        //
        // 摘要:
        //     Returns the signed angle in degrees between from and to.
        //
        // 参数:
        //   from:
        //     The vector from which the angular difference is measured.
        //
        //   to:
        //     The vector to which the angular difference is measured.
        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            Vector2 lhs = from.normalized;
            Vector2 rhs = to.normalized;
            float num = Mathf.Acos(Mathf.Clamp(Dot(lhs, rhs), -1f, 1f)) * 57.29578f;
            float num2 = Mathf.Sign(lhs.x * rhs.y - lhs.y * rhs.x);
            return num * num2;
        }

        //
        // 摘要:
        //     Returns the distance between a and b.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static float Distance(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude;
        }

        //
        // 摘要:
        //     Returns a copy of vector with its magnitude clamped to maxLength.
        //
        // 参数:
        //   vector:
        //
        //   maxLength:
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }

            return vector;
        }

        public static float SqrMagnitude(Vector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        public float SqrMagnitude()
        {
            return x * x + y * y;
        }

        //
        // 摘要:
        //     Returns a vector that is made from the smallest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
        }

        //
        // 摘要:
        //     Returns a vector that is made from the largest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
        }

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            Vector2 vector = current - target;
            Vector2 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * num3;
            Vector2 vector4 = target + (vector + vector3) * num3;
            if (Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }

            return vector4;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(0f - a.x, 0f - a.y);
        }

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.99999944E-11f;
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }
    }
}