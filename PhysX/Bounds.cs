namespace PhysX
{
    public class Bounds
    {
        public Point Point0 { get; protected set; }
        public Point Point1 { get; protected set; }

        public Point Scale => Point1 - Point0;
        public decimal Width => Scale.X;
        public decimal Height => Scale.Y;
        public decimal Depth => Scale.Z;

        public Bounds(Point point0, Point point1)
        {
            Point0 = point0;
            Point1 = point1;
        }

        public override string ToString() =>
            $"({Point.Round(Point0, Enviroment.RoundAmount)}, " +
            $"({Point.Round(Point1, Enviroment.RoundAmount)}";
    }
}
