namespace Timetable_Project
{
    // Eine ganz einfache Bewertung: Zählt einfach die Stunden
    public class SimpleScheduleEvaluator : IScheduleEvaluator
    {
        public int Bewerte(Stundenplan plan)
        {
            // Gibt einfach die Anzahl der Stunden zurück
            return plan.ToList().Count;
        }
    }
}