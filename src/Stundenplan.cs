using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    public class Stundenplan
    {
        public const int TAGE = 5;
        public const int STUNDEN = 8;

        // Matrix [TagIndex, StundeIndex]
        public Stunde[,] Matrix { get; set; }

        // Gewichtungen (einstellbar)
        public double GewichtRandstunden { get; set; } = 1.0;
        public double GewichtZwischenstunden { get; set; } = 1.0;
        public double GewichtRessourcen { get; set; } = 0.2;

        public Stundenplan()
        {
            Matrix = new Stunde[TAGE, STUNDEN];
        }

        public bool IstFrei(int tag, int stunde)
        {
            return Matrix[tag, stunde] == null;
        }

        public bool Eintragen(int tag, int stunde, Stunde s)
        {
            if (!IstFrei(tag, stunde)) return false;
            Matrix[tag, stunde] = s;
            return true;
        }

        public void Anzeigen()
        {
            string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };

            for (int t = 0; t < TAGE; t++)
            {
                Console.WriteLine($"\n--- {tage[t]} ---");
                bool leer = true;
                for (int s = 0; s < STUNDEN; s++)
                {
                    var st = Matrix[t, s];
                    if (st != null)
                    {
                        Console.WriteLine($"{s + 1}. Stunde: {st.Fach} ({st.Klasse}) mit {st.Lehrperson} in {st.Raum}");
                        leer = false;
                    }
                }
                if (leer) Console.WriteLine("(keine Einträge)");
            }
        }

        public double BewertePlan(out double randstrafe, out double zwischenstrafe, out double ressourcenstrafe)
        {
            double score = 100.0;
            randstrafe = 0.0;
            zwischenstrafe = 0.0;
            ressourcenstrafe = 0.0;

            // Randzeiten
            for (int t = 0; t < TAGE; t++)
            {
                if (Matrix[t, 0] != null)
                {
                    randstrafe += GewichtRandstunden;
                }
                if (Matrix[t, STUNDEN - 1] != null)
                {
                    randstrafe += GewichtRandstunden;
                }
            }
            score -= randstrafe;

            // Zwischenstunden: wenn nach einer belegten Stunde später eine Lücke kommt und danach erneut belegung
            for (int t = 0; t < TAGE; t++)
            {
                bool begonnen = false;
                for (int s = 0; s < STUNDEN; s++)
                {
                    if (Matrix[t, s] != null)
                    {
                        begonnen = true;
                    }
                    else if (begonnen)
                    {
                        // jede Lücke nach Start kostet GewichtZwischenstunden
                        zwischenstrafe += GewichtZwischenstunden;
                    }
                }
            }
            score -= zwischenstrafe;

            // Ressourcen: weniger verschiedene Räume ist besser
            var raeume = Matrix.Cast<Stunde>().Where(x => x != null).Select(x => x.Raum).Distinct().Count();
            // je mehr Räume, desto größer GewichtRessourcen
            ressourcenstrafe = raeume * GewichtRessourcen;
            score -= ressourcenstrafe;

            if (score < 0) score = 0;
            return score;
        }

        public List<Stunde> ToList()
        {
            var list = new List<Stunde>();
            string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };

            for (int t = 0; t < TAGE; t++)
            {
                for (int s = 0; s < STUNDEN; s++)
                {
                    var st = Matrix[t, s];
                    if (st != null)
                    {
                        st.Tag = tage[t];
                        st.StundeNummer = s + 1;
                        list.Add(st);
                    }
                }
            }
            return list;
        }

        public static Stundenplan FromList(List<Stunde> list)
        {
            var plan = new Stundenplan();
            if (list == null) return plan;

            var tage = new Dictionary<string, int>
            {
                { "Montag", 0 }, { "Dienstag", 1 }, { "Mittwoch", 2 }, { "Donnerstag", 3 }, { "Freitag", 4 }
            };

            foreach (var st in list)
            {
                if (string.IsNullOrEmpty(st.Tag)) continue;
                if (!tage.ContainsKey(st.Tag)) continue;
                int t = tage[st.Tag];
                int s = Math.Clamp(st.StundeNummer - 1, 0, STUNDEN - 1);
                if (plan.IstFrei(t, s))
                {
                    plan.Matrix[t, s] = st;
                }
            }

            return plan;
        }
    }
}
