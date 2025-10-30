using System;
using System.Collections.Generic;
using System.Linq;
namespace Timetable_Project
{
    public class Fach
    {
        public int Id { get; }
        public string Title { get; }
        public int Wochenstunden { get; }

        public Fach(int id, string title, int wochenstunden)
        {
            Id = id;
            Title = title;
            Wochenstunden = wochenstunden;
        }
    }
}