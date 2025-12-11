namespace Timetable_Project
{
    // Bewertet einen Stundenplan
    public interface IScheduleEvaluator
    {
        int Bewerte(Stundenplan plan);
    }
}