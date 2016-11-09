using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SchetsEditor
{
    class Opslag
    {
        /// <summary>
        /// Sla de getekende objecten op in een .sp bestand
        /// </summary>
        /// <param name="getekendeObjecten"></param>
        /// <param name="fileNaam"></param>
        /// <returns></returns>
        public static bool SlaOp(List<IVorm> getekendeObjecten, string fileNaam = null)
        {
            // Als er geen fileNaam is opgegeven gaan we uit van een 'Opslaan als'
            if (fileNaam == null)
            {
                // Maak een SaveFileDialog waarin we alleen .sp bestanden toelaten
                SaveFileDialog saveVenster = new SaveFileDialog();
                saveVenster.Filter = "SketchPlus|*.sp";
                saveVenster.DefaultExt = "sp";
                if (saveVenster.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveVenster.FileName) == ".sp")
                    {
                        File.WriteAllText(saveVenster.FileName, FileTekst(getekendeObjecten));
                        return true;
                    }
                    // Als de extensie niet gelijk is aan .sp laten we een error zien
                    OngeldigeExtensie();
                }
            } else
            {
                // Schrijf de getekende objecten weg in het bestand wat geopend is
                File.WriteAllText(fileNaam, FileTekst(getekendeObjecten));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Maak een string van alle getekende objecten, zodat we deze kunnen opslaan
        /// </summary>
        /// <param name="getekendeObjecten"></param>
        /// <returns></returns>
        private static string FileTekst(List<IVorm> getekendeObjecten)
        {
            string tekst = "";
            foreach (IVorm vorm in getekendeObjecten)
            {
                tekst += vorm.ToString();
            }
            return tekst;
        }

        /// <summary>
        /// Sla de bitmap op als een Jpeg, Png of gewoon als een bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        public static void Converteer(Bitmap bitmap)
        {
            SaveFileDialog saveVenster = new SaveFileDialog();
            saveVenster.Filter = "Image|*.jpg;*.png;*.bmp";
            saveVenster.DefaultExt = "png";
            if (saveVenster.ShowDialog() == DialogResult.OK)
            {
                string extensie = Path.GetExtension(saveVenster.FileName);
                switch (extensie)
                {
                    case ".jpg":
                        bitmap.Save(saveVenster.FileName, ImageFormat.Jpeg);
                        break;
                    case ".png":
                        bitmap.Save(saveVenster.FileName, ImageFormat.Png);
                        break;
                    case ".bmp":
                        bitmap.Save(saveVenster.FileName, ImageFormat.Bmp);
                        break;
                    default:
                        OngeldigeExtensie();
                        break;
                }
            }
        }

        /// <summary>
        /// Geef een error van een ongeldige extensie
        /// </summary>
        private static void OngeldigeExtensie()
        {
            MessageBox.Show("De extensie van het opgegeven bestand is ongeldig", "Ongeldige extensie", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Laad de geopende file in het geheugen
        /// Controleer per regel of deze overeenkomt met een object wat we kunnen tekenen
        /// Als dit zo is, maken we dit object zodat we deze later kunnen tekenen
        /// </summary>
        /// <param name="fileNaam"></param>
        /// <returns></returns>
        public static List<IVorm> Laad(string fileNaam)
        {
            string[] regels = File.ReadAllLines(fileNaam);
            List<IVorm> getekendeObjecten = new List<IVorm>();
            foreach (string regel in regels)
            {
                string[] parameters = regel.Split(' ');
                try
                {
                    Point startPunt = new Point(int.Parse(parameters[1]), int.Parse(parameters[2]));
                    // Als het een letter is, is het opgeslagen als SchetsEditor.Letter startpunt.X startpunt.Y Color.R Color.G Color.B Letter
                    if (parameters[0] == "SchetsEditor.Letter")
                    {
                        Color kleur = Color.FromArgb(int.Parse(parameters[3]), int.Parse(parameters[4]), int.Parse(parameters[5]));
                        char letter = parameters[7].ToCharArray()[0];
                    }
                    // Als het iets anders is, is het opgeslagen als SchetsEditor.Object startpunt.X startpunt.Y eindpunt.X eindpunt.Y Color.R Color.G Color.B
                    else
                    {
                        Point eindPunt = new Point(int.Parse(parameters[3]), int.Parse(parameters[4]));
                        Color kleur = Color.FromArgb(int.Parse(parameters[5]), int.Parse(parameters[6]), int.Parse(parameters[7]));
                        switch(parameters[0])
                        {
                            case "SchetsEditor.Lijn":
                                getekendeObjecten.Add(new Lijn(startPunt, eindPunt, kleur));
                                break;
                            case "SchetsEditor.Rechthoek":
                                getekendeObjecten.Add(new Rechthoek(startPunt, eindPunt, kleur));
                                break;
                            case "SchetsEditor.VolRechthoek":
                                getekendeObjecten.Add(new VolRechthoek(startPunt, eindPunt, kleur));
                                break;
                            case "SchetsEditor.Ovaal":
                                getekendeObjecten.Add(new Ovaal(startPunt, eindPunt, kleur));
                                break;
                            case "SchetsEditor.VolOvaal":
                                getekendeObjecten.Add(new VolOvaal(startPunt, eindPunt, kleur));
                                break;
                        }
                    }
                // Als er ergens iets fout gaat zien we de regels als ongeldig en gaan we naar de volgende
                } catch (Exception e)
                {
                    continue;
                }
            }
            return getekendeObjecten;
        }
    }
}
