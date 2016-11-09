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

        /// <summary>
        /// Initialiseer de SchetsControl
        /// Teken de objecten als de meegegeven zijn
        /// </summary>
        /// <param name="getekendeObjecten"></param>
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
                // Teken elke vorm in de meegegeven objecten
                foreach (IVorm vorm in getekendeObjecten)
                {
                    vorm.Teken(this);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Teken de schets
        /// </summary>
        /// <param name="o"></param>
        /// <param name="pea"></param>
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }

        /// <summary>
        /// Teken de schets aan de hand van de huidige lijst van getekende objecten
        /// </summary>
        public void TekenVormen()
        {
            schets.Schoon();
            foreach (IVorm vorm in getekendeObjecten)
            {
                vorm.Teken(this);
            }
            Invalidate();
        }

        /// <summary>
        /// Verander de afmeting van de schets
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ea"></param>
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }

        /// <summary>
        /// Maak de bitmapgraphics aan de hand van de schets
        /// </summary>
        /// <returns></returns>
        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        /// <summary>
        /// Maak de schets leeg
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ea"></param>
        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }

        /// <summary>
        /// Roteer de schets
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ea"></param>
        public void Roteer(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }

        /// <summary>
        /// Verander de penkleur
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        /// <summary>
        /// Verander de penkleur via het menu
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        /// <summary>
        /// Initialiseer de SchetsControl
        /// </summary>
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
