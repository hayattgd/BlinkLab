namespace BlinkLab.Engine.Math;

public struct Vector3
{
	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3(float x, float y) : this(x, y, 0) { }
	public Vector3(Vector2 vec, float z) : this(vec.x, vec.y, z) { }
	/// <summary>
	/// Returns new(value, value, value)
	/// </summary>
	public Vector3(float value) : this(value, value, value) { }

	public static Vector3 Zero => new(0);
	public static Vector3 One => new(1);
	public static Vector3 Infinity => new(float.PositiveInfinity);

	/// <summary>
	/// Returns (1, 0, 0)
	/// </summary>
	public static Vector3 Xaxis => new(1, 0, 0);

	/// <summary>
	/// Returns (0, 1, 0)
	/// </summary>
	public static Vector3 Yaxis => new(0, 1, 0);

	/// <summary>
	/// Returns (0, 0, 1)
	/// </summary>
	public static Vector3 Zaxis => new(0, 0, 1);

	public float x, y, z = 0;

	public static Vector3 Scale(Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);
	public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	=> new(
		a.x + (b.x - a.x) * t,
		a.y + (b.y - a.y) * t,
		a.z + (b.z - a.z) * t
	);

	public static Vector3 operator +(Vector3 lhs, Vector3 rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
	public static Vector3 operator -(Vector3 lhs, Vector3 rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);

	public static Vector3 operator *(Vector3 vector, float scalar) => new(vector.x * scalar, vector.y * scalar, vector.z * scalar);
	public static Vector3 operator *(float scalar, Vector3 vector) => vector * scalar;
	public static Vector3 operator /(Vector3 vector, float scalar) => new(vector.x / scalar, vector.y / scalar, vector.z / scalar);

	public static implicit operator Vector3(Vector2 vec) => new(vec.x, vec.y);
}
