using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SchetsEditor
{
    public interface IVorm
    {
        void Teken(SchetsControl s);
        bool OpGeklikt(SchetsControl s, Point p);
    }

    public abstract class Vorm : IVorm
    {
        protected Point startPunt, eindPunt;
        protected Color kleur;

        public Vorm(Point startPunt, Point eindPunt, Color kleur)
        {
            this.startPunt = startPunt;
            this.eindPunt = eindPunt;
            this.kleur = kleur;
        }

        public override string ToString()
        {
            return this.GetType() + " " + startPunt.X + " " + startPunt.Y + " " + eindPunt.X + " " + eindPunt.Y + " " + kleur.R + " " + kleur.G + " " + kleur.B + "\n";
        }

        /// <summary>
        /// Teken het object
        /// </summary>
        /// <param name="s"></param>
        public abstract void Teken(SchetsControl s);
        public abstract bool OpGeklikt(SchetsControl s, Point p);
    }

    public abstract class Tekst : IVorm
    {
        protected Point startPunt;
        protected Color kleur;
        protected char letter;

        public Tekst(Point startPunt, Color kleur, char letter)
        {
            this.startPunt = startPunt;
            this.kleur = kleur;
            this.letter = letter;
        }

        public override string ToString()
        {
            return this.GetType() + " " + startPunt.X + " " + startPunt.Y + " " + kleur.R + " " + kleur.G + " " + kleur.B + " " + letter + "\n";
        }

        public abstract void Teken(SchetsControl s);
        public abstract bool OpGeklikt(SchetsControl s, Point p);
    }

    public class Letter : Tekst
    {
        public Letter(Point startPunt, Color kleur, char letter) : base(startPunt, kleur, letter) { }

        public override void Teken(SchetsControl s)
        {
            new TekstTool().Letter(s, this.letter, new SolidBrush(this.kleur), this.startPunt);
        }

        /// <summary>
        /// Bereken of de klik binnen de 'bounds' van de letter zijn
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns>True of False</returns>
        public override bool OpGeklikt(SchetsControl s, Point p)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            SizeF sz = gr.MeasureString(letter.ToString(), font, startPunt, StringFormat.GenericTypographic);
            return (p.X >= startPunt.X && p.X <= startPunt.X + sz.Width && p.Y >= startPunt.Y && p.Y <= startPunt.Y + sz.Height);
        }
    }

    public class Rechthoek : Vorm
    {
        public Rechthoek(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) { }

        public override void Teken(SchetsControl s)
        {
            new RechthoekTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }

        /// <summary>
        /// Controleer of de klik op een van de randen van de rechthoek is
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns>True of False</returns>
        public override bool OpGeklikt(SchetsControl s, Point p)
        {
            int bovengrens = startPunt.Y < eindPunt.Y ? startPunt.Y : eindPunt.Y;
            int ondergrens = startPunt.Y < eindPunt.Y ? eindPunt.Y : startPunt.Y;
            int linkergrens = startPunt.X < eindPunt.X ? startPunt.X : eindPunt.X;
            int rechtergrens = startPunt.X < eindPunt.X ? eindPunt.X : startPunt.X;
            return (
                    // Controleer of de klik op de linkergrens was
                    (p.X >= linkergrens - 5 && p.X <= linkergrens + 5 && p.Y >= bovengrens - 5 && p.Y <= ondergrens + 5) ||
                    // Of de rechtgrens
                    (p.X >= rechtergrens - 5 && p.X <= rechtergrens + 5 && p.Y >= bovengrens -5 && p.Y <= ondergrens + 5) ||
                    // Of de bovengrens
                    (p.X >= linkergrens - 5 && p.X <= rechtergrens + 5 && p.Y >= bovengrens - 5 && p.Y <= bovengrens + 5) ||
                    // Of de ondergrens
                    (p.X >= linkergrens - 5 && p.X <= rechtergrens + 5 && p.Y >= ondergrens - 5 && p.Y <= ondergrens + 5)
                );
        }
    }

    public class VolRechthoek : Rechthoek
    {
        public VolRechthoek(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) { }

        public override void Teken(SchetsControl s)
        {
            new VolRechthoekTool().Compleet(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }

        /// <summary>
        /// Controleer of er binnen de rechthoek is geklikt
        /// Dit gebeurt door te kijen of zowel p.X als p.Y binnen de rechthoek liggen
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns>True of False</returns>
        public override bool OpGeklikt(SchetsControl s, Point p)
        {
            int width = Math.Abs(eindPunt.X - startPunt.X);
            int height = Math.Abs(eindPunt.Y - startPunt.Y);
            return (p.X >= startPunt.X && p.X <= startPunt.X + width && p.Y >= startPunt.Y && p.Y <= startPunt.Y + height);
        }
    }

    public class Ovaal : Vorm
    {
        public Ovaal(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) { }

        public override void Teken(SchetsControl s)
        {
            new OvaalTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }

        /// <summary>
        /// Controleer of de klik binnen de ovaal is
        /// Gebaseerd op: http://stackoverflow.com/questions/13285007/how-to-determine-if-a-point-is-within-an-ellipse
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool OpGeklikt(SchetsControl s, Point p)
        {
            int width = Math.Abs(eindPunt.X - startPunt.X);
            int height = Math.Abs(eindPunt.Y - startPunt.Y);
            int links = startPunt.X < eindPunt.X ? startPunt.X : eindPunt.X;
            int boven = startPunt.Y < eindPunt.Y ? startPunt.Y : eindPunt.Y;

            Point middelpunt = new Point(links + (width / 2), boven + (height / 2));

            double radiusX = (double)(width / 2);
            double radiusY = (double)(height / 2);

            if (radiusX <= 0.0 || radiusY <= 0.0)
                return false;

            Point genormaliseerdPunt = new Point(p.X - middelpunt.X, p.Y - middelpunt.Y);

            return ((double)(genormaliseerdPunt.X * genormaliseerdPunt.X)
                     / (radiusX * radiusX)) + ((double)(genormaliseerdPunt.Y * genormaliseerdPunt.Y) / (radiusY * radiusY))
                <= 1.0;
        }
    }

    public class VolOvaal : Ovaal
    {
        public VolOvaal(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) { }

        public override void Teken(SchetsControl s)
        {
            new VolOvaalTool().Compleet(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }

    public class Lijn : Vorm
    {
        public Lijn(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) { }

        public override void Teken(SchetsControl s)
        {
            new LijnTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }

        /// <summary>
        /// Bereken de afstand tot de lijn startpunt, eindpunt
        /// Controleer vervolgens of deze afstand minder dan 5 is
        /// Gebaseerd op: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns>True of False</returns>
        public override bool OpGeklikt(SchetsControl s, Point p)
        {
            float px = eindPunt.X - startPunt.X;
            float py = eindPunt.Y - startPunt.Y;
            float temp = (px * px) + (py * py);
            float u = ((p.X - startPunt.X) * px + (p.Y - startPunt.Y) * py) / (temp);

            if (u > 1)
                u = 1;
            else if (u < 0)
                u = 0;

            float x = startPunt.X + u * px;
            float y = startPunt.Y + u * py;

            float dx = x - p.X;
            float dy = y - p.Y;
            double afstand = Math.Sqrt(dx * dx + dy * dy);
            return afstand <= 5;
        }
    }
}
