using System.Drawing;

namespace PhysX
{
    public class Canvas
    {
        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;

        protected Bitmap _bitmap;
        protected Graphics _graphics;

        public Canvas(int width, int height)
        {
            _bitmap = new(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            Fill(Color.Black);
        }

        /// <summary>
        /// Fills all canvas using seted color
        /// </summary>
        /// <param name="color">Color that's canvas will filled</param>
        public void Fill(Color color) => _graphics.Clear(color);

        /// <summary>
        /// Sets pixel on canvas in seted coordinates and color
        /// </summary>
        /// <param name="color">The color that the pixel will be colored with</param>
        public void SetPixel(int x, int y, Color color)
        {
            if (CheckBounds(x, y)) _bitmap.SetPixel(x, y, color);
        }

        /// <summary>
        /// Sets the text you wrote on canvas in seted coordinates
        /// </summary>
        /// <param name="text">The text that will write on canvas</param>
        /// <param name="color">The color that will filled the text on canvas</param>
        /// <param name="fontSize">The font size of wrote text</param>
        public void SetText(int x, int y, string? text, Color color, float fontSize = 10)
        {
            if (CheckBounds(x, y))
                _graphics.DrawString(text, new(FontFamily.GenericSansSerif, fontSize),
                    new SolidBrush(color), x, y);
        }

        /// <summary>
        /// Returns the color from seted coordinates
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            if (CheckBounds(x, y)) return _bitmap.GetPixel(x, y);
            return new();
        }

        public bool CheckBounds(int x, int y) => (x >= 0 && x < Width) && (y >= 0 && y < Height);
        /// <summary>
        /// Saves canvas as file
        /// </summary>
        /// <param name="filename">Filename/path of result file</param>
        public void Save(string filename) => _bitmap.Save(filename);
        /// <summary>
        /// Saves canvas as file
        /// </summary>
        /// <param name="filename">Filename/path of result file</param>
        /// <param name="format"><para>Format of image file</para>
        /// <para>Supported formats: bmp, emf, exif, gif, guid, heif, 
        /// icon, jpeg, png, tiff, webp, wmf</para>
        /// </param>
        public void Save(string filename, string format) => Save($"{filename}.{format}");
    }
}
