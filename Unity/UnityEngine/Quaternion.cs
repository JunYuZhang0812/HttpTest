using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    //
    // 摘要:
    //     Quaternions are used to represent rotations.
    public struct Quaternion
    {
        //
        // 摘要:
        //     X component of the Quaternion. Don't modify this directly unless you know quaternions
        //     inside out.
        public float x;

        //
        // 摘要:
        //     Y component of the Quaternion. Don't modify this directly unless you know quaternions
        //     inside out.
        public float y;

        //
        // 摘要:
        //     Z component of the Quaternion. Don't modify this directly unless you know quaternions
        //     inside out.
        public float z;

        //
        // 摘要:
        //     W component of the Quaternion. Do not directly modify quaternions.
        public float w;

        private static readonly Quaternion identityQuaternion = new Quaternion(0f, 0f, 0f, 1f);

        public const float kEpsilon = 1E-06f;

        //
        // 摘要:
        //     Returns or sets the euler angle representation of the rotation.
        public Vector3 eulerAngles
        {
            get
            {
                return Internal_MakePositive(Internal_ToEulerRad(this) * 57.29578f);
            }
            set
            {
                this = Internal_FromEulerRad(value * ((float)Math.PI / 180f));
            }
        }

        public float this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y; 
                    case 2:
                        return this.z; 
                    case 3:
                        return this.w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        //
        // 摘要:
        //     The identity rotation (Read Only).
        public static Quaternion identity => identityQuaternion;

        //
        // 摘要:
        //     Constructs new Quaternion with given x,y,z,w components.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   z:
        //
        //   w:
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        //
        // 摘要:
        //     Creates a rotation which rotates angle degrees around axis.
        //
        // 参数:
        //   angle:
        //
        //   axis:
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            INTERNAL_CALL_AngleAxis(angle, ref axis, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AngleAxis(float angle, ref Vector3 axis, out Quaternion value);

        public void ToAngleAxis(out float angle, out Vector3 axis)
        {
            Internal_ToAxisAngleRad(this, out axis, out angle);
            angle *= 57.29578f;
        }

        //
        // 摘要:
        //     Creates a rotation which rotates from fromDirection to toDirection.
        //
        // 参数:
        //   fromDirection:
        //
        //   toDirection:
        public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            INTERNAL_CALL_FromToRotation(ref fromDirection, ref toDirection, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FromToRotation(ref Vector3 fromDirection, ref Vector3 toDirection, out Quaternion value);

        //
        // 摘要:
        //     Creates a rotation which rotates from fromDirection to toDirection.
        //
        // 参数:
        //   fromDirection:
        //
        //   toDirection:
        public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            this = FromToRotation(fromDirection, toDirection);
        }

        //
        // 摘要:
        //     Creates a rotation with the specified forward and upwards directions.
        //
        // 参数:
        //   forward:
        //     The direction to look in.
        //
        //   upwards:
        //     The vector that defines in which direction up is.
        public static Quaternion LookRotation(Vector3 forward, [DefaultValue("Vector3.up")] Vector3 upwards)
        {
            INTERNAL_CALL_LookRotation(ref forward, ref upwards, out var value);
            return value;
        }

        //
        // 摘要:
        //     Creates a rotation with the specified forward and upwards directions.
        //
        // 参数:
        //   forward:
        //     The direction to look in.
        //
        //   upwards:
        //     The vector that defines in which direction up is.
        public static Quaternion LookRotation(Vector3 forward)
        {
            Vector3 upwards = Vector3.up;
            INTERNAL_CALL_LookRotation(ref forward, ref upwards, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_LookRotation(ref Vector3 forward, ref Vector3 upwards, out Quaternion value);

        //
        // 摘要:
        //     Spherically interpolates between a and b by t. The parameter t is clamped to
        //     the range [0, 1].
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
        {
            INTERNAL_CALL_Slerp(ref a, ref b, t, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Slerp(ref Quaternion a, ref Quaternion b, float t, out Quaternion value);

        //
        // 摘要:
        //     Spherically interpolates between a and b by t. The parameter t is not clamped.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
        {
            INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SlerpUnclamped(ref Quaternion a, ref Quaternion b, float t, out Quaternion value);

        //
        // 摘要:
        //     Interpolates between a and b by t and normalizes the result afterwards. The parameter
        //     t is clamped to the range [0, 1].
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
        {
            INTERNAL_CALL_Lerp(ref a, ref b, t, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Lerp(ref Quaternion a, ref Quaternion b, float t, out Quaternion value);

        //
        // 摘要:
        //     Interpolates between a and b by t and normalizes the result afterwards. The parameter
        //     t is not clamped.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
        {
            INTERNAL_CALL_LerpUnclamped(ref a, ref b, t, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_LerpUnclamped(ref Quaternion a, ref Quaternion b, float t, out Quaternion value);

        //
        // 摘要:
        //     Rotates a rotation from towards to.
        //
        // 参数:
        //   from:
        //
        //   to:
        //
        //   maxDegreesDelta:
        public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            float num = Angle(from, to);
            if (num == 0f)
            {
                return to;
            }

            float t = Mathf.Min(1f, maxDegreesDelta / num);
            return SlerpUnclamped(from, to, t);
        }

        //
        // 摘要:
        //     Returns the Inverse of rotation.
        //
        // 参数:
        //   rotation:
        public static Quaternion Inverse(Quaternion rotation)
        {
            INTERNAL_CALL_Inverse(ref rotation, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Inverse(ref Quaternion rotation, out Quaternion value);

        //
        // 摘要:
        //     Returns a rotation that rotates z degrees around the z axis, x degrees around
        //     the x axis, and y degrees around the y axis.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   z:
        public static Quaternion Euler(float x, float y, float z)
        {
            return Internal_FromEulerRad(new Vector3(x, y, z) * ((float)Math.PI / 180f));
        }

        //
        // 摘要:
        //     Returns a rotation that rotates z degrees around the z axis, x degrees around
        //     the x axis, and y degrees around the y axis.
        //
        // 参数:
        //   euler:
        public static Quaternion Euler(Vector3 euler)
        {
            return Internal_FromEulerRad(euler * ((float)Math.PI / 180f));
        }

        private static Vector3 Internal_ToEulerRad(Quaternion rotation)
        {
            INTERNAL_CALL_Internal_ToEulerRad(ref rotation, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_ToEulerRad(ref Quaternion rotation, out Vector3 value);

        private static Quaternion Internal_FromEulerRad(Vector3 euler)
        {
            INTERNAL_CALL_Internal_FromEulerRad(ref euler, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_FromEulerRad(ref Vector3 euler, out Quaternion value);

        private static void Internal_ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle)
        {
            INTERNAL_CALL_Internal_ToAxisAngleRad(ref q, out axis, out angle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_ToAxisAngleRad(ref Quaternion q, out Vector3 axis, out float angle);

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerRotation(float x, float y, float z)
        {
            return Internal_FromEulerRad(new Vector3(x, y, z));
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerRotation(Vector3 euler)
        {
            return Internal_FromEulerRad(euler);
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(float x, float y, float z)
        {
            this = Internal_FromEulerRad(new Vector3(x, y, z));
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(Vector3 euler)
        {
            this = Internal_FromEulerRad(euler);
        }

        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vector3 ToEuler()
        {
            return Internal_ToEulerRad(this);
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerAngles(float x, float y, float z)
        {
            return Internal_FromEulerRad(new Vector3(x, y, z));
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerAngles(Vector3 euler)
        {
            return Internal_FromEulerRad(euler);
        }

        [Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void ToAxisAngle(out Vector3 axis, out float angle)
        {
            Internal_ToAxisAngleRad(this, out axis, out angle);
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(float x, float y, float z)
        {
            SetEulerRotation(new Vector3(x, y, z));
        }

        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(Vector3 euler)
        {
            this = EulerRotation(euler);
        }

        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public static Vector3 ToEulerAngles(Quaternion rotation)
        {
            return Internal_ToEulerRad(rotation);
        }

        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vector3 ToEulerAngles()
        {
            return Internal_ToEulerRad(this);
        }

        [Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion AxisAngle(Vector3 axis, float angle)
        {
            INTERNAL_CALL_AxisAngle(ref axis, angle, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AxisAngle(ref Vector3 axis, float angle, out Quaternion value);

        [Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetAxisAngle(Vector3 axis, float angle)
        {
            this = AxisAngle(axis, angle);
        }

        //
        // 摘要:
        //     Set x, y, z and w components of an existing Quaternion.
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

        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vector3 result = default(Vector3);
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }

        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return Dot(lhs, rhs) > 0.999999f;
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }

        //
        // 摘要:
        //     The dot product between two rotations.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        //
        // 摘要:
        //     Creates a rotation with the specified forward and upwards directions.
        //
        // 参数:
        //   view:
        //     The direction to look in.
        //
        //   up:
        //     The vector that defines in which direction up is.
        public void SetLookRotation(Vector3 view)
        {
            Vector3 up = Vector3.up;
            SetLookRotation(view, up);
        }

        //
        // 摘要:
        //     Creates a rotation with the specified forward and upwards directions.
        //
        // 参数:
        //   view:
        //     The direction to look in.
        //
        //   up:
        //     The vector that defines in which direction up is.
        public void SetLookRotation(Vector3 view, [DefaultValue("Vector3.up")] Vector3 up)
        {
            this = LookRotation(view, up);
        }

        //
        // 摘要:
        //     Returns the angle in degrees between two rotations a and b.
        //
        // 参数:
        //   a:
        //
        //   b:
        public static float Angle(Quaternion a, Quaternion b)
        {
            float f = Dot(a, b);
            return Mathf.Acos(Mathf.Min(Mathf.Abs(f), 1f)) * 2f * 57.29578f;
        }

        private static Vector3 Internal_MakePositive(Vector3 euler)
        {
            float num = -0.005729578f;
            float num2 = 360f + num;
            if (euler.x < num)
            {
                euler.x += 360f;
            }
            else if (euler.x > num2)
            {
                euler.x -= 360f;
            }

            if (euler.y < num)
            {
                euler.y += 360f;
            }
            else if (euler.y > num2)
            {
                euler.y -= 360f;
            }

            if (euler.z < num)
            {
                euler.z += 360f;
            }
            else if (euler.z > num2)
            {
                euler.z -= 360f;
            }

            return euler;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        public override bool Equals(object other)
        {
            if (!(other is Quaternion))
            {
                return false;
            }

            Quaternion quaternion = (Quaternion)other;
            return x.Equals(quaternion.x) && y.Equals(quaternion.y) && z.Equals(quaternion.z) && w.Equals(quaternion.w);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string of the Quaternion.
        //
        // 参数:
        //   format:
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        //
        // 摘要:
        //     Returns a nicely formatted string of the Quaternion.
        //
        // 参数:
        //   format:
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
        }
    }
}
