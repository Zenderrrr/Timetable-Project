namespace Timetable_Project
{
    public class Raum
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int Kapazitaet { get; set; }
        public bool Verfuegbar { get; set; } = true;

        

        public Raum(int id, string bezeichnung, int kapazitaet)
        {
            Id = id;
            Bezeichnung = bezeichnung;
            Kapazitaet = kapazitaet;
            Verfuegbar = true;
        }
    }
}
