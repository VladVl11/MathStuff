using UnityEngine;
using MathF = System.MathF;

namespace MathLib
{
public class MyVector2
{
    public float x;
    public float y;

    public MyVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public  float Magnitude()
    {
        return MathF.Sqrt(x * x + y * y);
    }
    public MyVector2 Normalize()
    {
       float length = Magnitude();
        if (length != 0)
        {
            return MFGLib.Divide(this, length);
        }
        return new MyVector2(0, 0);
    }
}

public class MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public static MyVector3 right = new(1f, 0f, 0f);
    public static MyVector3 up = new(0f, 1f, 0f);
    public static MyVector3 forward = new(0f, 0f, 1f);

    public float Magnitude()
    {
        return MathF.Sqrt(x * x + y * y + z * z);
    }
    public MyVector3 Normalize()
    {
        float length = Magnitude();
        if (length != 0)
        {
            return MFGLib.Divide(this, length);
        }
        return new MyVector3(0, 0, 0);
    }
    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }
}


public class MyVector4
{
    public float x;
    public float y;
    public float z;
    public float w;

    public MyVector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}

public class Matrix4
{
    public MyVector4 row0;
    public MyVector4 row1;
    public MyVector4 row2;
    public MyVector4 row3;
    public Matrix4(MyVector3 right, MyVector3 up, MyVector3 forward, MyVector3 p)
    {
        this.row0 = new MyVector4(right.x, up.x, forward.x, p.x);
        this.row1 = new MyVector4(right.y, up.y, forward.y, p.y);
        this.row2 = new MyVector4(right.z, up.z, forward.z, p.z);
        this.row3 = new MyVector4(0, 0, 0, 1);
    }

    public MyVector4 Multiply(MyVector4 vector)
    {
        return new MyVector4(
            MFGLib.Dot4(row0, vector),
            MFGLib.Dot4(row1, vector),
            MFGLib.Dot4(row2, vector),
            MFGLib.Dot4(row3, vector)
        );
    }
}
public class Quat
{
    public float w;
    public float x;
    public float y;
    public float z;

    public Quat(float w, float x, float y, float z)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // Quat from vector3 (w = 0)
    public Quat (MyVector3 v)
    {
        this.w = 0;
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    // Quat from axis and angle (angle in radians)
    public Quat(MyVector3 axis, float angleRad)
    {
        axis = axis.Normalize();

        float halfAngle = angleRad / 2f;
        
        float sinHalfAngle = MathF.Sin(halfAngle);
        this.w = MathF.Cos(halfAngle);
        this.x = axis.x * sinHalfAngle;
        this.y = axis.y * sinHalfAngle;
        this.z = axis.z * sinHalfAngle;
    }
    
    // Made with AI
    public Quat(Matrix4 m)
    {
        float trace = m.row0.x + m.row1.y + m.row2.z;
        if (trace > 0)
        {
            float scale = 0.5f / MathF.Sqrt(trace + 1.0f);
            this.w = 0.25f / scale;
            this.x = (m.row1.z - m.row2.y) * scale;
            this.y = (m.row2.x - m.row0.z) * scale;
            this.z = (m.row0.y - m.row1.x) * scale;
        }
        else
        {
            if (m.row0.x > m.row1.y && m.row0.x > m.row2.z)
            {
                float scale = 2.0f * MathF.Sqrt(1.0f + m.row0.x - m.row1.y - m.row2.z);
                this.w = (m.row1.z - m.row2.y) / scale;
                this.x = 0.25f * scale;
                this.y = (m.row1.x + m.row0.y) / scale;
                this.z = (m.row2.x + m.row0.z) / scale;
            }
            else if (m.row1.y > m.row2.z)
            {
                float scale = 2.0f * MathF.Sqrt(1.0f + m.row1.y - m.row0.x - m.row2.z);
                this.w = (m.row2.x - m.row0.z) / scale;
                this.x = (m.row1.x + m.row0.y) / scale;
                this.y = 0.25f * scale;
                this.z = (m.row2.y + m.row1.z) / scale;
            }
            else
            {
                float scale = 2.0f * MathF.Sqrt(1.0f + m.row2.z - m.row0.x - m.row1.y);
                this.w = (m.row0.y - m.row1.x) / scale;
                this.x = (m.row2.x + m.row0.z) / scale;
                this.y = (m.row2.y + m.row1.z) / scale;
                this.z = 0.25f * scale;
            }
        }
    }

    public static Quat operator *(Quat a, Quat b)
        {
            return new Quat(
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z,
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w
            );
        }

        public Quat Inverse()
        {
            float magnitudeSquared = w * w + x * x + y * y + z * z;
            if (magnitudeSquared > 0)
            {
                return new Quat(w / magnitudeSquared, -x / magnitudeSquared, -y / magnitudeSquared, -z / magnitudeSquared);
            }
            return new Quat(0, 0, 0, 0);
        }

        // Rotate a vector by this quaternion
        public MyVector3 RotateVector(MyVector3 v)
        {
            Quat vQuat = new Quat(v);
            Quat resultQuat = this * vQuat * Inverse();
            return new MyVector3(resultQuat.x, resultQuat.y, resultQuat.z);
        }
}

public static class UnityConvert
{
    public static Vector3 ToUnityVector3(MyVector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static MyVector3 FromUnityVector3(Vector3 v)
    {
        return new MyVector3(v.x, v.y, v.z);
    }

    public static Quaternion ToUnityQuaternion(Quat q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    public static Quat FromUnityQuaternion(Quaternion q)
    {
        return new Quat(q.x, q.y, q.z, q.w);
    }
}
    public static class MFGLib
    {

        // ----------------------------------
        // MATRIX MATH
        // ----------------------------------

        public static float Dot4(MyVector4 a, MyVector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static MyVector3 TransformPoint(MyVector3 point, Matrix4 matrix)
        {
            MyVector4 homogenousPoint = new MyVector4(point.x, point.y, point.z, 1);
            MyVector4 transformed = matrix.Multiply(homogenousPoint);
            return new MyVector3(transformed.x, transformed.y, transformed.z);
        }

        public static MyVector3 TransformDirection(MyVector3 direction, Matrix4 matrix)
        {
            MyVector4 homogenousDirection = new MyVector4(direction.x, direction.y, direction.z, 0);
            MyVector4 transformed = matrix.Multiply(homogenousDirection);
            return new MyVector3(transformed.x, transformed.y, transformed.z);
        }

        public static Matrix4 BuildTRS(MyVector3 p, MyVector3 right, MyVector3 up, MyVector3 forward, MyVector3 scale)
        {
            return new Matrix4(Scale(right, scale.x), Scale(up, scale.y), Scale(forward, scale.z), p);
        }

        // ----------------------------------
        // VECTOR MATH
        // ----------------------------------

        public static MyVector2 Add(MyVector2 a, MyVector2 b)
        {
            return new MyVector2(a.x + b.x, a.y + b.y);
        }

        public static MyVector3 Add(MyVector3 a, MyVector3 b)
        {
            return new MyVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static MyVector2 Subtract(MyVector2 a, MyVector2 b)
        {
            return new MyVector2(a.x - b.x, a.y - b.y);
        }

        public static MyVector3 Subtract(MyVector3 a, MyVector3 b)
        {
            return new MyVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        // ----------------------------------
        // VECTOR FUNCTIONS
        // ----------------------------------

        public static float Distance(MyVector2 a, MyVector2 b)
        {
            return Subtract(a, b).Magnitude();
        }

        public static float Distance(MyVector3 a, MyVector3 b)
        {
            return Subtract(a, b).Magnitude();
        }

        // ----------------------------------
        // VECTOR SCALING
        // ----------------------------------

        public static MyVector2 Scale(MyVector2 vector, float scale)
        {
            return new MyVector2(vector.x * scale, vector.y * scale);
        }

        public static MyVector3 Scale(MyVector3 vector, float scale)
        {
            return new MyVector3(vector.x * scale, vector.y * scale, vector.z * scale);
        }

        public static MyVector2 Divide(MyVector2 vector, float divisor)
        {
            if (divisor != 0)
            {
                return new MyVector2(vector.x / divisor, vector.y / divisor);
            }
            return new MyVector2(0, 0);
        }

        public static MyVector3 Divide(MyVector3 vector, float divisor)
        {
            if (divisor != 0)
            {
                return new MyVector3(vector.x / divisor, vector.y / divisor, vector.z / divisor);
            }
            return new MyVector3(0, 0, 0);
        }

        // ----------------------------------
        // VECTOR PRODUCTS
        // ----------------------------------

        public static float DotProd(MyVector2 a, MyVector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static float DotProd(MyVector3 a, MyVector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        // ----------------------------------
        // MOVEMENT AND GRAVITY
        // ----------------------------------

        public static MyVector2 MoveStep(MyVector2 direction, float speed, float deltaTime)
        {
            MyVector2 dir = direction.Normalize();
            MyVector2 vel = Scale(dir, speed);
            return Scale(vel, deltaTime);
        }

        public static MyVector3 MoveStep(MyVector3 direction, float speed, float deltaTime)
        {
            MyVector3 dir = direction.Normalize();
            MyVector3 vel = Scale(dir, speed);
            return Scale(vel, deltaTime);
        }

        public static MyVector2 ApplyGravity(MyVector2 velocity, MyVector2 gravity, float deltaTime)
        {
            return Add(velocity, Scale(gravity, deltaTime));
        }

        public static MyVector3 ApplyGravity(MyVector3 velocity, MyVector3 gravity, float deltaTime)
        {
            return Add(velocity, Scale(gravity, deltaTime));
        }

        // ----------------------------------
        // ANGLE AND ROTATION
        // ----------------------------------

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (MathF.PI / 180f);
        }


        public static float RadiansToDegrees(float radians)
        {
            return radians * (180f / MathF.PI);
        }


        public static float AngleFromVector2(MyVector2 v)
        {
            return MathF.Atan2(v.y, v.x);
        }

        public static MyVector2 Vector2FromAngle(float radians)
        {
            return new MyVector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        public static MyVector3 ForwardFromYawPitch(float yawRadians, float pitchRadians)
        {
            return new MyVector3(MathF.Sin(yawRadians) * MathF.Cos(pitchRadians), MathF.Sin(pitchRadians), MathF.Cos(yawRadians) * MathF.Cos(pitchRadians));
        }

        public static MyVector3 CrossProduct(MyVector3 a, MyVector3 b)
        {
            return new MyVector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        // ----------------------------------
        // LERP
        // ----------------------------------
        public static float Clamp01(float value)
        {
            if (value < 0) return 0;
            if (value > 1) return 1;
            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            t = Clamp01(t);
            return a + (b - a) * t;
        }

        public static MyVector3 Lerp(MyVector3 a, MyVector3 b, float t)
        {
            t = Clamp01(t);
            return Add(a, Scale(Subtract(b, a), t));
        }

        // ----------------------------------
        // EASING
        // ----------------------------------
        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }

        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(1 - t, 2) * 2;
        }

        // ----------------------------------
        // NON-UNIFORM SCALING
        // ----------------------------------

        public static MyVector3 NonUniformScale(MyVector3 v, MyVector3 scale)
        {
            return new MyVector3(v.x * scale.x, v.y * scale.y, v.z * scale.z);
        }

        // ----------------------------------
        // BASIS TRANSFORMATION
        // ----------------------------------

        public static MyVector3 DirectionFromRotation(MyVector3 localDir, MyVector3 right, MyVector3 up, MyVector3 forward)
        {
            return Add(Add(Scale(right, localDir.x), Scale(up, localDir.y)), Scale(forward, localDir.z));
        }

        public static MyVector3 LocalToWorld(MyVector3 p, MyVector3 localPos, MyVector3 right, MyVector3 up, MyVector3 forward, MyVector3 scale)
        {
            MyVector3 scaledPos = NonUniformScale(localPos, scale);
            MyVector3 rotatedPos = DirectionFromRotation(scaledPos, right, up, forward);
            return Add(rotatedPos, p);
        }

        public static MyVector3 RotateAroundAxis(MyVector3 point, MyVector3 axis, float angleRad)
        {
            axis = axis.Normalize();

            float cos = MathF.Cos(angleRad);
            float sin = MathF.Sin(angleRad);

            // Rodrigues' rotation formula for rotating a point around an arbitrary axis in 3D space
            return Add(Add(Scale(point, cos), Scale(axis, DotProd(axis, point) * (1f - cos))), Scale(CrossProduct(axis, point), sin));
        }
    }
}