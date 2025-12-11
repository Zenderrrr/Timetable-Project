namespace Timetable_Project.Generation
{
    public class TestDataGenerator
    {
        public List<Lehrperson> GenerateLehrer(int count)
        {
            var list = new List<Lehrperson>();
            for (int i = 0; i < count; i++)
                list.Add(new Lehrperson(i + 1, $"Lehrer{i + 1}"));
            return list;
        }

        public List<Raum> GenerateRaeume(int count)
        {
            var list = new List<Raum>();
            for (int i = 0; i < count; i++)
                list.Add(new Raum(i + 1, $"R{i + 1}", 30));
            return list;
        }
    }
}
