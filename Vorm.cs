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
        
        public abstract void Teken(SchetsControl s);
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
    }

    public class Letter : Tekst
    {
        public Letter(Point startPunt, Color kleur, char letter) : base(startPunt, kleur, letter) {}

        public override void Teken(SchetsControl s)
        {
            new TekstTool().Letter(s, this.letter, new SolidBrush(this.kleur), this.startPunt);
        }
    }

    public class Rechthoek : Vorm
    {
        public Rechthoek(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) {}

        public override void Teken(SchetsControl s)
        {
            new RechthoekTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }

    public class VolRechthoek : Vorm
    {
        public VolRechthoek(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) {}

        public override void Teken(SchetsControl s)
        {
            new VolRechthoekTool().Compleet(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }

    public class Ovaal : Vorm
    {
        public Ovaal(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) {}

        public override void Teken(SchetsControl s)
        {
            new OvaalTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }

    public class VolOvaal : Vorm
    {
        public VolOvaal(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) {}

        public override void Teken(SchetsControl s)
        {
            new VolOvaalTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }

    public class Lijn : Vorm
    {
        public Lijn(Point startPunt, Point eindPunt, Color kleur) : base(startPunt, eindPunt, kleur) {}

        public override void Teken(SchetsControl s)
        {
            new LijnTool().Bezig(s.MaakBitmapGraphics(), startPunt, eindPunt, new SolidBrush(this.kleur));
        }
    }
}
