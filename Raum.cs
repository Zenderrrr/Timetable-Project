using System;
using System.Collections.Generic;
using System.Linq;
namespace Timetable_Project
{
    public class Raum
    {
        public int Id { get; }
        public string Bezeichnung { get; }
        public int Kapazitaet { get; }
        public bool Verfuegbar { get; internal set; }

        public Raum(int id, string bezeichnung, int kapazitaet)
        {
            Id = id;
            Bezeichnung = bezeichnung;
            Kapazitaet = kapazitaet;
            Verfuegbar = true;
        }
    }
}