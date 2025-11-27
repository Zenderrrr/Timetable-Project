using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    /// <summary>
    /// Repräsentiert einen vollständigen Stundenplan mit Matrix-Struktur
    /// </summary>
    public class Stundenplan
    {
        public const int TAGE = 5;
        public const int STUNDEN = 8;
        public static readonly string[] TAGE_NAMEN = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
        
        public Stunde[,] Matrix { get; set; }

        public double GewichtRandstunden { get; set; } = 1.0;
        public double GewichtZwischenstunden { get; set; } = 1.0;
        public double GewichtRessourcen { get; set; } = 0.2;

        public Stundenplan()
        {
            Matrix = new Stunde[TAGE, STUNDEN];
        }

        /// <summary>
        /// Prüft ob ein bestimmter Zeitpunkt im Stundenplan frei ist
        /// </summary>
        public bool IstFrei(int tag, int stunde)
        {
            return Matrix[tag, stunde] == null;
        }

        /// <summary>
        /// Trägt eine Stunde an einer bestimmten Position im Stundenplan ein
        /// </summary>
        public bool Eintragen(int tag, int stunde, Stunde s)
        {
            if (!IstFrei(tag, stunde)) return false;
            Matrix[tag, stunde] = s;
            return true;
        }

        /// <summary>
        /// Zeigt den Stundenplan in tabellarischer Form an
        /// </summary>
        public void Anzeigen()
        {
            string[] uhrzeiten = { "08:00-08:45", "08:45-09:30", "09:50-10:35", "10:35-11:20", "11:40-12:25", "12:25-13:10", "13:30-14:15", "14:15-15:00" };

            Console.WriteLine("\n" + new string('=', 120));
            Console.Write("Stunde | Zeit\t|");
            foreach (var tag in TAGE_NAMEN)
            {
                Console.Write($" {tag,-18} |");
            }
            Console.WriteLine("\n" + new string('=', 120));

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

        /// <summary>
        /// Zeigt den Stundenplan mit detaillierten Informationen an
        /// </summary>
        public void AnzeigenDetail()
        {
            for (int tag = 0; tag < TAGE; tag++)
            {
                Console.WriteLine($"\n┌────────────────── {TAGE_NAMEN[tag]} ──────────────────");
                bool hatEintraege = false;
                for (int stunde = 0; stunde < STUNDEN; stunde++)
                {
                    var stundeDaten = Matrix[tag, stunde];
                    if (stundeDaten != null)
                    {
                        Console.WriteLine($"│ {stunde + 1}. Stunde:");
                        Console.WriteLine($"│ {stundeDaten.Fach} mit {stundeDaten.Lehrperson}");
                        Console.WriteLine($"│ Raum: {stundeDaten.Raum}, Klasse: {stundeDaten.Klasse}");
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

        /// <summary>
        /// Bewertet die Qualität des Stundenplans anhand verschiedener Kriterien
        /// </summary>
        /// <returns>Bewertungsscore zwischen 0 und 100</returns>
        public double BewertePlan(out double randstrafe, out double zwischenstrafe, out double ressourcenstrafe)
        {
            double score = 100.0;
            randstrafe = 0.0;
            zwischenstrafe = 0.0;
            ressourcenstrafe = 0.0;

            for (int t = 0; t < TAGE; t++)
            {
                if (Matrix[t, 0] != null) randstrafe += GewichtRandstunden;
                if (Matrix[t, STUNDEN - 1] != null) randstrafe += GewichtRandstunden;
            }

            for (int t = 0; t < TAGE; t++)
            {
                bool unterrichtBegonnen = false;
                int lueckenCount = 0;

                for (int s = 0; s < STUNDEN; s++)
                {
                    if (Matrix[t, s] != null)
                    {
                        if (unterrichtBegonnen && lueckenCount > 0)
                        {
                            zwischenstrafe += GewichtZwischenstunden * lueckenCount;
                        }
                        unterrichtBegonnen = true;
                        lueckenCount = 0;
                    }
                    else if (unterrichtBegonnen)
                    {
                        lueckenCount++;
                    }
                }
            }

            var raeume = Matrix.Cast<Stunde>().Where(x => x != null).Select(x => x.Raum).Distinct().Count();
            ressourcenstrafe = raeume * GewichtRessourcen;

            score -= (randstrafe + zwischenstrafe + ressourcenstrafe);
            return Math.Max(0, score);
        }

        /// <summary>
        /// Konvertiert die Matrix in eine flache Liste von Stunden
        /// </summary>
        public List<Stunde> ToList()
        {
            var list = new List<Stunde>();
            for (int t = 0; t < TAGE; t++)
            {
                for (int s = 0; s < STUNDEN; s++)
                {
                    var st = Matrix[t, s];
                    if (st != null)
                    {
                        st.Tag = TAGE_NAMEN[t];
                        st.StundeNummer = s + 1;
                        list.Add(st);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Erstellt einen Stundenplan aus einer Liste von Stunden
        /// </summary>
        public static Stundenplan FromList(List<Stunde> list)
        {
            var plan = new Stundenplan();
            if (list == null) return plan;

            foreach (var st in list)
            {
                if (string.IsNullOrEmpty(st.Tag)) continue;
                
                int tagIndex = Array.IndexOf(TAGE_NAMEN, st.Tag);
                if (tagIndex < 0) continue;
                
                int stundeIndex = Math.Clamp(st.StundeNummer - 1, 0, STUNDEN - 1);
                if (plan.IstFrei(tagIndex, stundeIndex))
                {
                    plan.Matrix[tagIndex, stundeIndex] = st;
                }
            }
            return plan;
        }
    }
}