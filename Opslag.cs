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
        public static bool SlaOp(List<IVorm> getekendeObjecten, string fileNaam = null)
        {
            SaveFileDialog saveVenster = new SaveFileDialog();
            saveVenster.Filter = "SketchPlus|*.sp";
            saveVenster.DefaultExt = "sp";
            if (fileNaam == null)
            {
                if (saveVenster.ShowDialog() == DialogResult.OK && Path.GetExtension(saveVenster.FileName) == ".sp")
                {
                    File.WriteAllText(saveVenster.FileName, FileTekst(getekendeObjecten));
                    return true;
                }
            } else
            {
                File.WriteAllText(fileNaam, FileTekst(getekendeObjecten));
                return true;
            }
            return false;
        }

        private static string FileTekst(List<IVorm> getekendeObjecten)
        {
            string tekst = "";
            foreach (IVorm vorm in getekendeObjecten)
            {
                tekst += vorm.ToString();
            }
            return tekst;
        }

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
                    if (parameters[0] == "SchetsEditor.Letter")
                    {
                        Color kleur = Color.FromArgb(int.Parse(parameters[3]), int.Parse(parameters[4]), int.Parse(parameters[5]));
                        char letter = parameters[7].ToCharArray()[0];
                    } else
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
                } catch (Exception e)
                {
                    continue;
                }
            }
            return getekendeObjecten;
        }
    }
}
