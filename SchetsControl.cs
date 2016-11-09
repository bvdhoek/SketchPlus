using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;
        public List<IVorm> getekendeObjecten = new List<IVorm>();

        public Color PenKleur
        {
            get { return penkleur; }
        }

        public Schets Schets
        {
            get { return schets; }
        }

        public SchetsControl(List<IVorm> getekendeObjecten = null)
        {
            this.ClientSize = new Size(777, 498);
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            if (getekendeObjecten != null)
            {
                this.getekendeObjecten = getekendeObjecten;
                foreach (IVorm vorm in getekendeObjecten)
                {
                    vorm.Teken(this);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }

        public void TekenVormen()
        {
            schets.Schoon();
            foreach (IVorm vorm in getekendeObjecten)
            {
                vorm.Teken(this);
            }
            Invalidate();
        }

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }

        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }

        public void Roteer(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }

        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SchetsControl
            // 
            this.Name = "SchetsControl";
            this.Size = new System.Drawing.Size(777, 498);
            this.ResumeLayout(false);
        }
    }
}
