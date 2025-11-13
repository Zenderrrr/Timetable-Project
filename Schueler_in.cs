using System.Collections.Generic;

namespace Timetable_Project
{
    public class Schueler_in
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Alter { get; set; }
        public string Klasse { get; set; }
        public List<string> Faecher { get; set; } = new();


        public Schueler_in(int id, string name, int alter, string klasse)
        {
            Id = id;
            Name = name;
            Alter = alter;
            Klasse = klasse;
        }
    }
}