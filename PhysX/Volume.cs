namespace PhysX
{
    public static class Volume
    {
        /// <summary>
        /// Returns the volume of the ellipse via formula:
        /// V=(4/3)*PI*a*b*c
        /// </summary>
        /// <param name="x">Length</param>
        /// <param name="y">Height</param>
        /// <param name="z">Depth</param>
        /// <returns>Volume of the ellipse</returns>
        public static decimal Ellipse(decimal x, decimal y, decimal z) => 4m / 3 *
            (decimal)Math.PI * x * y * z;
        /// <summary>
        /// Returns the volume of the sphere via formula:
        /// V=(4/3)*PI*R^3
        /// </summary>
        /// <param name="radius">Radius of sphere</param>
        /// <returns>Volume of the sphere</returns>
        public static decimal Sphere(decimal radius) => Ellipse(radius, radius, radius);
        /// <summary>
        /// Returns the volume of the cube via formula:
        /// V=a*b*c
        /// </summary>
        /// <param name="x">Length</param>
        /// <param name="y">Height</param>
        /// <param name="z">Depth</param>
        /// <returns>Volume of the cube</returns>
        public static decimal Cube(decimal x, decimal y, decimal z) => x * y * z;
        /// <summary>
        /// Returns the volume of the cube via formula:
        /// V=a^3
        /// </summary>
        /// <param name="size">Size of side of the cube</param>
        /// <returns>Volume of the cube</returns>
        public static decimal Cube(decimal size) => Cube(size, size, size);
    }
}
