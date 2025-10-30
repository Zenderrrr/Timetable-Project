using System;
using System.Collections.Generic;
using System.Linq;
namespace Timetable_Project
{
    public class Lehrperson
    {
        public int Id { get; }
        public string Name { get; }
        public List<string> Faecher { get; }
        public Dictionary<string, bool> Verfuegbarkeit { get; }

        public Lehrperson(int id, string name)
        {
            Id = id;
            Name = name;
            Faecher = new List<string>();
            Verfuegbarkeit = new Dictionary<string, bool>();
        }
    }
}