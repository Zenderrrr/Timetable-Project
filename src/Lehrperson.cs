using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    /// <summary>
    /// Repräsentiert eine Lehrperson mit ihren Eigenschaften und Verfügbarkeiten
    /// </summary>
    public class Lehrperson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Faecher { get; set; } = new();
        public Dictionary<string, bool> Verfuegbarkeit { get; set; } = new();
        public Dictionary<string, List<bool>> TagesVerfuegbarkeit { get; set; } = new();

        public Lehrperson(int id, string name)
        {
            Id = id;
            Name = name;
            InitStandardVerfuegbarkeit();
        }

        private void InitStandardVerfuegbarkeit()
        {
            string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
            
            foreach (var tag in tage)
            {
                Verfuegbarkeit[tag] = true;
                
                TagesVerfuegbarkeit[tag] = new List<bool>();
                for (int i = 0; i < 8; i++)
                {
                    TagesVerfuegbarkeit[tag].Add(true);
                }
            }
        }

        /// <summary>
        /// Prüft die allgemeine Verfügbarkeit einer Lehrperson an einem bestimmten Tag
        /// </summary>
        public bool IstVerfuegbar(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return true;
            if (!Verfuegbarkeit.ContainsKey(tag)) return true;
            return Verfuegbarkeit[tag];
        }

        /// <summary>
        /// Prüft die stundenweise Verfügbarkeit einer Lehrperson an einem bestimmten Tag
        /// </summary>
        public bool IstVerfuegbar(string tag, int stunde)
        {
            if (string.IsNullOrEmpty(tag) || stunde < 0 || stunde >= 8) return true;
            if (!TagesVerfuegbarkeit.ContainsKey(tag)) return true;
            if (stunde >= TagesVerfuegbarkeit[tag].Count) return true;
            
            return TagesVerfuegbarkeit[tag][stunde];
        }

        /// <summary>
        /// Setzt die Verfügbarkeit einer Lehrperson für einen bestimmten Tag und Stunde
        /// </summary>
        public void SetzeVerfuegbarkeit(string tag, int stunde, bool verfuegbar)
        {
            if (TagesVerfuegbarkeit.ContainsKey(tag) && stunde >= 0 && stunde < TagesVerfuegbarkeit[tag].Count)
            {
                TagesVerfuegbarkeit[tag][stunde] = verfuegbar;
            }
        }
    }
}