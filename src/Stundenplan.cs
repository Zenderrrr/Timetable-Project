using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    public class Stundenplan
    {
        public const int TAGE = 5;
        public const int STUNDEN = 8;

        
        public Stunde[,] Matrix { get; set; }

        // Gewichtungen (einstellbar)
        public double GewichtRandstunden { get; set; } = 1.0;
        public double GewichtZwischenstunden { get; set; } = 1.0;
        public double GewichtRessourcen { get; set; } = 0.2;

        // Uhrzeiten für jede Stunde
        private static readonly string[] Uhrzeiten = {
            "08:00-08:45",    // 1. Stunde
            "08:45-09:30",    // 2. Stunde  
            "09:50-10:35",    // 3. Stunde
            "10:35-11:20",    // 4. Stunde
            "11:40-12:25",    // 5. Stunde
            "12:25-13:10",    // 6. Stunde
            "13:30-14:15",    // 7. Stunde
            "14:15-15:00"     // 8. Stunde
        };

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
    
    
    string[] uhrzeiten = {
        "08:00-08:45", "08:45-09:30", "09:50-10:35", "10:35-11:20",
        "11:40-12:25", "12:25-13:10", "13:30-14:15", "14:15-15:00"
    };

    
    Console.WriteLine("\n" + new string('=', 120));
    Console.Write("Stunde | Zeit\t|");
    foreach (var tag in tage)
    {
        Console.Write($" {tag,-18} |");
    }
    Console.WriteLine("\n" + new string('=', 120));

    // Stunden
    for (int stunde = 0; stunde < STUNDEN; stunde++)
    {
        Console.Write($" {stunde + 1,-5} | {uhrzeiten[stunde]}\t|");
        
        for (int tag = 0; tag < TAGE; tag++)
        {
            var stundeDaten = Matrix[tag, stunde];
            if (stundeDaten != null)
            {
                string eintrag = $"{stundeDaten.Fach} ({stundeDaten.Klasse})";
                Console.Write($" {eintrag,-18} |");
            }
            else
            {
                Console.Write($" {"-",-18} |");
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine(new string('=', 120));
}

// Zusätzlich: Detail-Ansicht für mehr Infos
public void AnzeigenDetail()
{
    string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
    string[] uhrzeiten = {
        "08:00-08:45", "08:45-09:30", "09:50-10:35", "10:35-11:20",
        "11:40-12:25", "12:25-13:10", "13:30-14:15", "14:15-15:00"
    };

    for (int tag = 0; tag < TAGE; tag++)
    {
        Console.WriteLine($"\n┌────────────────── {tage[tag]} ──────────────────");
        bool hatEintraege = false;
        
        for (int stunde = 0; stunde < STUNDEN; stunde++)
        {
            var stundeDaten = Matrix[tag, stunde];
            if (stundeDaten != null)
            {
                Console.WriteLine($"│ {stunde + 1}. Stunde ({uhrzeiten[stunde]}):");
                Console.WriteLine($"│   {stundeDaten.Fach} mit {stundeDaten.Lehrperson}");
                Console.WriteLine($"│   Raum: {stundeDaten.Raum}, Klasse: {stundeDaten.Klasse}");
                Console.WriteLine($"├──────────────────────────────────────────");
                hatEintraege = true;
            }
        }
        
        if (!hatEintraege)
        {
            Console.WriteLine("│ Keine Einträge");
            Console.WriteLine($"├──────────────────────────────────────────");
        }
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