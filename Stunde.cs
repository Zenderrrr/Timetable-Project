namespace Timetable_Project
{
    public class Stunde
    {
        public string Fach { get; set; }
        public string Lehrperson { get; set; }
        public string Raum { get; set; }
        public string Klasse { get; set; }
        public string Tag { get; set; }        // z.B. "Montag"
        public int StundeNummer { get; set; }  // 1..STUNDEN

        
    }
}