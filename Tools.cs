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
        protected IVorm laatstGetekendObject;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            schetsControl = s;
            startpunt = p;
        }

        public virtual void MuisLos(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.PenKleur);
            Console.WriteLine(laatstGetekendObject);
            if (laatstGetekendObject != null)
                schetsControl.getekendeObjecten.Add(laatstGetekendObject);
            s.Schets.veranderd = true;
        }

        public virtual Color KwastKleur()
        {
            return schetsControl.PenKleur;
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

        public override void Letter(SchetsControl s, char c, Brush kwast, Point startpunt)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = gr.MeasureString(tekst, font, startpunt, StringFormat.GenericTypographic);
                gr.DrawString(tekst, font, kwast, startpunt, StringFormat.GenericTypographic);
                schetsControl.getekendeObjecten.Add(new Letter(startpunt, KwastKleur(), c));
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
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

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
            s.Invalidate();
        }

        public override void Letter(SchetsControl s, char c)
        {
        }

        public override void Letter(SchetsControl s, char c, Brush kwast, Point startPunt)
        {
        }

        public abstract void Bezig(Graphics g, Point p1, Point p2);
        public abstract void Bezig(Graphics g, Point p1, Point p2, Brush kwast);

        public virtual void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            this.Compleet(g, p1, p2);
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
            laatstGetekendObject = new Rechthoek(p1, p2, KwastKleur());
            g.DrawRectangle(MaakPen(this.kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            Bezig(g, p1, p2);
        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            laatstGetekendObject = new VolRechthoek(p1, p2, KwastKleur());
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class OvaalTool : TweepuntTool
    {
        public override string ToString() { return "ovaal"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            laatstGetekendObject = new Ovaal(p1, p2, KwastKleur());
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));   
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            Bezig(g, p1, p2);
        }
    }

    public class VolOvaalTool : OvaalTool
    {
        public override string ToString() { return "ovlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            laatstGetekendObject = new VolOvaal(p1, p2, KwastKleur());
            g.FillEllipse(kwast, Punten2Rechthoek(p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            laatstGetekendObject = new Lijn(p1, p2, KwastKleur());
            g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }

        public override void Bezig(Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.kwast = kwast;
            Bezig(g, p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
    }
}
