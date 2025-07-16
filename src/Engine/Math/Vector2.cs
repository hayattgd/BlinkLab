namespace BlinkLab.Engine.Math;

public struct Vector2
{
	public Vector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}
	/// <summary>
	/// Returns new(value, value)
	/// </summary>
	public Vector2(float value) : this(value, value) { }

	public static Vector2 Zero => new(0);
	public static Vector2 One => new(1);
	public static Vector2 Infinity => new(float.PositiveInfinity);

	/// <summary>
	/// Returns (1, 0)
	/// </summary>
	public static Vector2 Xaxis => new(1, 0);

	/// <summary>
	/// Returns (0, 1)
	/// </summary>
	public static Vector2 Yaxis => new(0, 1);

	public float x, y = 0;

	public static Vector2 Scale(Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);
	public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
	=> new(
		a.x + (b.x - a.x) * t,
		a.y + (b.y - a.y) * t
	);

	public static Vector2 operator +(Vector2 lhs, Vector2 rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
	public static Vector2 operator -(Vector2 lhs, Vector2 rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);

	public static Vector2 operator *(Vector2 vector, float scalar) => new(vector.x * scalar, vector.y * scalar);
	public static Vector2 operator *(float scalar, Vector2 vector) => vector * scalar;
	public static Vector2 operator /(Vector2 vector, float scalar) => new(vector.x / scalar, vector.y / scalar);

	public static implicit operator Vector2(Vector3 vec) => new(vec.x, vec.y);
}
