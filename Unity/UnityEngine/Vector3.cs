
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    //
    // 摘要:
    //     Representation of 3D vectors and points.
    public struct Vector3
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

        private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

        private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);

        private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);

        private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);

        private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);

        private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);

        private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

        private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);

        private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        //
        // 摘要:
        //     Returns this vector with a magnitude of 1 (Read Only).
        public Vector3 normalized => Normalize(this);

        //
        // 摘要:
        //     Returns the length of this vector (Read Only).
        public float magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        //
        // 摘要:
        //     Returns the squared length of this vector (Read Only).
        public float sqrMagnitude => x * x + y * y + z * z;

        //
        // 摘要:
        //     Shorthand for writing Vector3(0, 0, 0).
        public static Vector3 zero => zeroVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(1, 1, 1).
        public static Vector3 one => oneVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(0, 0, 1).
        public static Vector3 forward => forwardVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(0, 0, -1).
        public static Vector3 back => backVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(0, 1, 0).
        public static Vector3 up => upVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(0, -1, 0).
        public static Vector3 down => downVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(-1, 0, 0).
        public static Vector3 left => leftVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(1, 0, 0).
        public static Vector3 right => rightVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(float.PositiveInfinity, float.PositiveInfinity,
        //     float.PositiveInfinity).
        public static Vector3 positiveInfinity => positiveInfinityVector;

        //
        // 摘要:
        //     Shorthand for writing Vector3(float.NegativeInfinity, float.NegativeInfinity,
        //     float.NegativeInfinity).
        public static Vector3 negativeInfinity => negativeInfinityVector;

        [Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd => new Vector3(0f, 0f, 1f);

        //
        // 摘要:
        //     Creates a new vector with given x, y, z components.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   z:
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        //
        // 摘要:
        //     Creates a new vector with given x, y components and sets z to zero.
        //
        // 参数:
        //   x:
        //
        //   y:
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        [Obsolete("Use Vector3.ProjectOnPlane instead.")]
        public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
        {
            return fromThat - Project(fromThat, excludeThis);
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
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
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
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        //
        // 摘要:
        //     Calculate a position between the points specified by current and target, moving
        //     no farther than the distance specified by maxDistanceDelta.
        //
        // 参数:
        //   current:
        //     The position to move from.
        //
        //   target:
        //     The position to move towards.
        //
        //   maxDistanceDelta:
        //     Distance to move current per call.
        //
        // 返回结果:
        //     The new position.
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 vector = target - current;
            float num = vector.magnitude;
            if (num <= maxDistanceDelta || num < float.Epsilon)
            {
                return target;
            }

            return current + vector / num * maxDistanceDelta;
        }

        //
        // 摘要:
        //     Set x, y and z components of an existing Vector3.
        //
        // 参数:
        //   newX:
        //
        //   newY:
        //
        //   newZ:
        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        //
        // 摘要:
        //     Multiplies two vectors component-wise.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        //
        // 摘要:
        //     Multiplies every component of this vector by the same component of scale.
        //
        // 参数:
        //   scale:
        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        //
        // 摘要:
        //     Cross Product of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        //
        // 摘要:
        //     Returns true if the given vector is exactly equal to this vector.
        //
        // 参数:
        //   other:
        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }

            Vector3 vector = (Vector3)other;
            return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
        }

        //
        // 摘要:
        //     Reflects a vector off the plane defined by a normal.
        //
        // 参数:
        //   inDirection:
        //
        //   inNormal:
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        //
        // 摘要:
        //     Makes this vector have a magnitude of 1.
        //
        // 参数:
        //   value:
        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return value / num;
            }

            return zero;
        }

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
        //   lhs:
        //
        //   rhs:
        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        //
        // 摘要:
        //     Projects a vector onto another vector.
        //
        // 参数:
        //   vector:
        //
        //   onNormal:
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float num = Dot(onNormal, onNormal);
            if (num < Mathf.Epsilon)
            {
                return zero;
            }

            return onNormal * Dot(vector, onNormal) / num;
        }

        //
        // 摘要:
        //     Projects a vector onto a plane defined by a normal orthogonal to the plane.
        //
        // 参数:
        //   planeNormal:
        //     The direction from the vector towards the plane.
        //
        //   vector:
        //     The location of the vector above the plane.
        //
        // 返回结果:
        //     The location of the vector on the plane.
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        //
        // 摘要:
        //     Returns the angle in degrees between from and to.
        //
        // 参数:
        //   from:
        //     The vector from which the angular difference is measured.
        //
        //   to:
        //     The vector to which the angular difference is measured.
        //
        // 返回结果:
        //     The angle in degrees between the two vectors.
        public static float Angle(Vector3 from, Vector3 to)
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
        //
        //   axis:
        //     A vector around which the other vectors are rotated.
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            Vector3 lhs = from.normalized;
            Vector3 rhs = to.normalized;
            float num = Mathf.Acos(Mathf.Clamp(Dot(lhs, rhs), -1f, 1f)) * 57.29578f;
            float num2 = Mathf.Sign(Dot(axis, Cross(lhs, rhs)));
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
        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        //
        // 摘要:
        //     Returns a copy of vector with its magnitude clamped to maxLength.
        //
        // 参数:
        //   vector:
        //
        //   maxLength:
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }

            return vector;
        }

        public static float Magnitude(Vector3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static float SqrMagnitude(Vector3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        //
        // 摘要:
        //     Returns a vector that is made from the smallest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        }

        //
        // 摘要:
        //     Returns a vector that is made from the largest components of two vectors.
        //
        // 参数:
        //   lhs:
        //
        //   rhs:
        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(0f - a.x, 0f - a.y, 0f - a.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string for this vector.
        //
        // 参数:
        //   format:
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));
        }

        [Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to)
        {
            return Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f));
        }
    }
}

