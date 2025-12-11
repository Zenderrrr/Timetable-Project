namespace Timetable_Project
{
    // Prüft, ob jemand oder ein Raum verfügbar ist
    public interface IAvailabilityProvider
    {
        bool IstVerfuegbar(string name, int stunde);
    }
}