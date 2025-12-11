using System.Collections.Generic;
namespace Timetable_Project
{
    // Ganz einfache Verfügbarkeitsprüfung: Alle sind immer verfügbar
    public class SimpleAvailabilityProvider : IAvailabilityProvider
    {
        public bool IstVerfuegbar(string name, int stunde)
        {
            // Sagt immer ja
            return true;
        }
    }
}