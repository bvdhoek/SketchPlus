using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;
        protected SchetsControl schetsControl;
        protected Color kwastKleur;
        protected IVorm laatstGetekendObject;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            schetsControl = s;
            startpunt = p;
        }

        public virtual void MuisLos(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.PenKleur);
            s.Schets.veranderd = true;
        }

        public virtual Color KwastKleur(Brush kwast)
        {
            SolidBrush brush = (SolidBrush)kwast;
            return brush.Color;
        }

        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
        public abstract void Letter(SchetsControl s, char c, Brush kwast, Point startPunt);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            this.Letter(s, c, kwast, startpunt);
        }

        /// <summary>
        /// Teken de letter en zet het startpunt += de breedte van de letter
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <param name="kwast"></param>
        /// <param name="startpunt"></param>
        public override void Letter(SchetsControl s, char c, Brush kwast, Point startpunt)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = gr.MeasureString(tekst, font, startpunt, StringFormat.GenericTypographic);
                gr.DrawString(tekst, font, kwast, startpunt, StringFormat.GenericTypographic);
                schetsControl.Schets.veranderd = true;
                schetsControl.getekendeObjecten.Add(new Letter(startpunt, KwastKleur(kwast), c));
                this.startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }

        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }

        /// <summary>
        /// Teken het object en voeg het toe aan de lijst van getekende objecten
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
            if (laatstGetekendObject != null)
            {
                schetsControl.Schets.veranderd = true;
                schetsControl.getekendeObjecten.Add(laatstGetekendObject);
            }
            s.Invalidate();
        }

        public override void Letter(SchetsControl s, char c) {}
        public override void Letter(SchetsControl s, char c, Brush kwast, Point startPunt) {}

        public abstract void Bezig(Graphics g, Point p1, Point p2);
        public abstract void Bezig(Graphics g, Point p1, Point p2, Brush kwast);

        public virtual void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.Bezig(g, p1, p2, kwast);
        }

        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            kwastKleur = schetsControl.PenKleur;
            laatstGetekendObject = new Rechthoek(p1, p2, kwastKleur);
            g.DrawRectangle(MaakPen(this.kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            kwastKleur = KwastKleur(kwast);
            laatstGetekendObject = new Rechthoek(p1, p2, kwastKleur);
            g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            kwastKleur = schetsControl.PenKleur;
            laatstGetekendObject = new VolRechthoek(p1, p2, kwastKleur);
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }

        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            kwastKleur = KwastKleur(kwast);
            laatstGetekendObject = new VolRechthoek(p1, p2, kwastKleur);
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class OvaalTool : TweepuntTool
    {
        public override string ToString() { return "ovaal"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            kwastKleur = schetsControl.PenKleur;
            laatstGetekendObject = new Ovaal(p1, p2, kwastKleur);
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));   
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            kwastKleur = KwastKleur(kwast);
            laatstGetekendObject = new Ovaal(p1, p2, kwastKleur);
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class VolOvaalTool : OvaalTool
    {
        public override string ToString() { return "ovlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            kwastKleur = schetsControl.PenKleur;
            laatstGetekendObject = new VolOvaal(p1, p2, kwastKleur);
            g.FillEllipse(kwast, Punten2Rechthoek(p1, p2));
        }

        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            kwastKleur = KwastKleur(kwast);
            laatstGetekendObject = new VolOvaal(p1, p2, kwastKleur);
            g.FillEllipse(kwast, Punten2Rechthoek(p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            kwastKleur = schetsControl.PenKleur;
            laatstGetekendObject = new Lijn(p1, p2, kwastKleur);
            g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            kwastKleur = KwastKleur(kwast);
            laatstGetekendObject = new Lijn(p1, p2, kwastKleur);
            g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        /// <summary>
        /// Teken elke keer als de muis wordt verplaatst een nieuw object van het type lijn
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : ISchetsTool
    {
        public override string ToString() { return "gum"; }

        public void Letter(SchetsControl s, char c) {}
        public void MuisDrag(SchetsControl s, Point p) {}
        public void MuisVast(SchetsControl s, Point p) { }

        /// <summary>
        /// Verwijder het hoogst gelegen object (als laatst getekende),
        /// waarop geklikt wordt, uit de lijst van getekende objecten
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        public void MuisLos(SchetsControl s, Point p)
        {
            if (s.getekendeObjecten.Remove(s.getekendeObjecten.FindLast(vorm => vorm.OpGeklikt(s, p))))
                s.TekenVormen();
        }
    }
}
