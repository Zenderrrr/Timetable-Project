using System;
namespace Timetable_Project
{
    public class Fach
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Wochenstunden { get; set; }


        public Fach(int id, string title, int wochenstunden)
        {
            Id = id;
            Title = title;
            Wochenstunden = wochenstunden;
        }
    }
}