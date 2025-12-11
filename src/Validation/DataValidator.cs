namespace Timetable_Project.Validation
{
    public class DataValidator
    {
        public List<string> Validate(Daten daten)
        {
            var errors = new List<string>();

            if (!daten.Lehrpersonen.Any())
                errors.Add("Keine Lehrpersonen vorhanden");

            if (!daten.Raeume.Any())
                errors.Add("Keine Räume vorhanden");

            if (!daten.Schueler.Any())
                errors.Add("Keine Schüler vorhanden");

            return errors;
        }
    }
}
