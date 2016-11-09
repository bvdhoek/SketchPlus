using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        /// <summary>
        /// Initialiseer het hoofdscherm
        /// </summary>
        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }

        /// <summary>
        /// Maak het file menu en koppel alle methoden eraan
        /// </summary>
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Openen", null, this.openen);
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        /// <summary>
        /// Maak het help menu en koppel alle methoden eraan
        /// </summary>
        private void maakHelpMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }

        /// <summary>
        /// Laat informatie zien over de applicatie
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ea"></param>
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("Schets versie 1.0\n(c) UU Informatica 2010"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        /// <summary>
        /// Open een nieuwe lege tekening door een SchetsWin te maken zonder parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nieuw(object sender, EventArgs e)
        {
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }


        /// <summary>
        /// Laad een tekening uit een bestand en geef deze mee aan het SchetsWin
        /// zodat die de tekening correct kan laten zien
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openen(object sender, EventArgs e)
        {
            OpenFileDialog b = new OpenFileDialog();
            b.Filter = "SketchPlus|*.sp";
            if (b.ShowDialog() == DialogResult.OK)
            {
                List<IVorm> getekendeObjecten = Opslag.Laad(b.FileName);
                SchetsWin s = new SchetsWin(getekendeObjecten, b.FileName);
                s.MdiParent = this;
                s.Show();
            }
        }

        /// <summary>
        /// Sluit de applicatie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void afsluiten(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
