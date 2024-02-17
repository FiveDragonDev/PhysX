using static System.Math;

namespace PhysX
{
    public static class MathD
    {
        public static decimal Round(decimal value, int amount = 0)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(amount.ToString());
            value = Math.Round(value * (int)Pow(10, amount)) / (int)Pow(10, amount);
            return value;
        }
        public static decimal Lerp(decimal a, decimal b, decimal t) => a + (t * (b - a));
        public static decimal Clamp(decimal value, decimal min, decimal max) =>
            Min(Max(value, min), max);
    }
}
