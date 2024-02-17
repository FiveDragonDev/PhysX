namespace PhysX
{
    public struct Point
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }

        public Point(decimal v) : this(v, v, v) { }
        public Point(decimal x, decimal y) : this(x, y, 0) { }
        public Point(decimal x, decimal y, decimal z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point operator -(Point a)
        { return new Point(-a.X, -a.Y, -a.Z); }
        public static Point operator +(Point a, Point b)
        { return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Point operator -(Point a, Point b)
        { return a + (-b); }
        public static Point operator *(Point a, Point b)
        { return new Point(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static Point operator /(Point a, Point b)
        { return new Point(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }
        public static Point operator *(Point a, decimal b)
        { return new Point(a.X * b, a.Y * b, a.Z * b); }
        public static Point operator /(Point a, decimal b)
        { return new Point(a.X / b, a.Y / b, a.Z / b); }
        public static Point operator +(Point a, decimal b)
        { return new Point(a.X + b, a.Y + b, a.Z + b); }
        public static Point operator %(Point a, decimal b)
        { return new Point(a.X % b, a.Y % b, a.Z % b); }
        public static Point operator -(Point a, decimal b)
        { return a + (-b); }

        public readonly decimal Magnitude => (decimal)Math.Sqrt((double)SqrMagnitude);
        public readonly decimal SqrMagnitude => X * X + Y * Y + Z * Z;
        public readonly Point Normalized => this / Magnitude;

        public static Point Round(Point value, int amount = 0)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(amount.ToString());
            return new()
            {
                X = MathD.Round(value.X, amount),
                Y = MathD.Round(value.Y, amount),
                Z = MathD.Round(value.Z, amount),
            };
        }

        public static Point FromJson(string @string)
        {
            @string = @string.Trim();
            @string = @string.Split('(')[1].Split(')')[0];
            string[] strings = @string.Split(", ");
            decimal[] decimals = new decimal[3];
            for (int i = 0; i < decimals.Length; i++)
                decimals[i] = Convert.ToDecimal(strings[i]);
            return new(decimals[0], decimals[1], decimals[2]);
        }
        public readonly string ToJson() => $"({X}, {Y}, {Z})";

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !(a == b);

        public override readonly string ToString() => $"({X}, {Y}, {Z})";
        public override readonly bool Equals(object? obj) => obj is Point point &&
                   X == point.X &&
                   Y == point.Y &&
                   Z == point.Z;
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);
    }
}
