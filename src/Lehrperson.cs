using System.Collections.Generic;

namespace Timetable_Project
{
    public class Lehrperson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Liste von Fächern (string Titel)
        public List<string> Faecher { get; set; } = new();
        // Verfügbarkeit pro Wochentag, z.B. "Dienstag" -> false
        public Dictionary<string, bool> Verfuegbarkeit { get; set; } = new();


        public Lehrperson(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool IstVerfuegbar(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return true;
            if (!Verfuegbarkeit.ContainsKey(tag)) return true;
            return Verfuegbarkeit[tag];
        }
    }
}