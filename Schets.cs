using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public bool veranderd = false;
        
        /// <summary>
        /// Initialiseer de schets
        /// </summary>
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }

        /// <summary>
        /// Geef de graphics van de huidige bitmap
        /// </summary>
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }

        /// <summary>
        /// Verander de afmeting van de bitmap
        /// </summary>
        /// <param name="sz"></param>
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }

        /// <summary>
        /// Teken de bitmap
        /// </summary>
        /// <param name="gr"></param>
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }

        /// <summary>
        /// Vervang de huidige bitmap met een nieuwe lege bitmap
        /// </summary>
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Roteer de huidige bitmap
        /// </summary>
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
        
        /// <summary>
        /// Geef de bitmap terug
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            return bitmap;
        }
    }
}
