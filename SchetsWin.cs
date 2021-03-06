﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Drawing.Imaging;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        string fileNaam;
        public SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        /// <summary>
        /// Sla de file op
        /// Als dit gelukt is, is de schets onveranderd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opslaan(object sender, EventArgs e)
        {
            if (Opslag.SlaOp(schetscontrol.getekendeObjecten, fileNaam))
            {
                schetscontrol.Schets.veranderd = false;
            }
        }

        /// <summary>
        /// Sla de file op
        /// Als dit gelukt is, is de schets onveranderd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opslaanAls(object sender, EventArgs e)
        {
            if (Opslag.SlaOp(schetscontrol.getekendeObjecten))
            {
                schetscontrol.Schets.veranderd = false;
            }
        }

        /// <summary>
        /// Converteer de bitmap naar een afbeelding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void converteer(object sender, EventArgs e)
        {
            Opslag.Converteer(schetscontrol.Schets.ToBitmap());
        }

        /// <summary>
        /// Verander de afmeting van het tekenpaneel
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ea"></param>
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        /// <summary>
        /// Maak het menu waarin je de tools kan selecteren
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        /// <summary>
        /// Maak de buttons waarin je de tools kan selecteren
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        /// <summary>
        /// Sluit de tekening als er geen verandering is
        /// of als de gebruiker bevestiging geeft dat de verandering niet uit maakt
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        private void afsluiten(object obj, EventArgs ea)
        {
            if (VeranderingsWaarschuwing("Weet u zeker dat u de schets wilt sluiten?\n U heeft onopgeslagen veranderingen."))
                this.Close();
        }

        /// <summary>
        /// Initialiseer de schetswindow
        /// </summary>
        /// <param name="getekendeObjecten"></param>
        /// <param name="fileNaam"></param>
        public SchetsWin(List<IVorm> getekendeObjecten = null, string fileNaam = null)
        {
            this.FormClosing += this.SchetsWinSluiten;
            this.fileNaam = fileNaam;
            ISchetsTool[] deTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new OvaalTool()
                                    , new VolOvaalTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan", "White"
                                 };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            if (getekendeObjecten == null)
                schetscontrol = new SchetsControl();
            else
                schetscontrol = new SchetsControl(getekendeObjecten);
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                huidigeTool.MuisVast(schetscontrol, mea.Location);
            };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisDrag(schetscontrol, mea.Location);
            };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisLos(schetscontrol, mea.Location);
                vast = false;
            };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                huidigeTool.Letter(schetscontrol, kpea.KeyChar);
            };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        /// <summary>
        /// Maak het file menu
        /// </summary>
        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Opslaan", null, this.opslaan);
            menu.DropDownItems.Add("Opslaan als...", null, this.opslaanAls);
            menu.DropDownItems.Add("Converteren", null, this.converteer);
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        /// <summary>
        /// Maak het tool menu
        /// </summary>
        /// <param name="tools"></param>
        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        /// <summary>
        /// Maak het aktie menu
        /// </summary>
        /// <param name="kleuren"></param>
        private void maakAktieMenu(String[] kleuren)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        /// <summary>
        /// Maak de tool buttons
        /// </summary>
        /// <param name="tools"></param>
        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        /// <summary>
        /// Maak de aktie buttons
        /// </summary>
        /// <param name="kleuren"></param>
        private void maakAktieButtons(String[] kleuren)
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);
            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Penkleur:"; 
            l.Location = new Point(180, 3); 
            l.AutoSize = true;               
            paneel.Controls.Add(l);
            
            cbb = new ComboBox(); cbb.Location = new Point(240, 0); 
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
        }

        /// <summary>
        /// Geef een waarschuwing als de schets veranderd is
        /// </summary>
        /// <param name="waarschuwing"></param>
        /// <returns></returns>
        private bool VeranderingsWaarschuwing(string waarschuwing)
        {
            if (schetscontrol.Schets.veranderd)
            {
                DialogResult result = MessageBox.Show(waarschuwing, "Schets veranderd", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    return true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initialiseer de SchetsWin
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SchetsWin
            // 
            this.ClientSize = new System.Drawing.Size(148, 135);
            this.Name = "SchetsWin";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Sluit de SchetsWin als er geen veranderingen zijn
        /// of als de gebruiker bevestiging geeft dat de verandering niet uit maakt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SchetsWinSluiten(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !VeranderingsWaarschuwing("Weet u zeker dat u de schets wilt sluiten?\n U heeft onopgeslagen veranderingen.");
        }
    }
}
