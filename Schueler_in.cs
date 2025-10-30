public class Schueler_in
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Alter { get; private set; }
    public string Klasse { get; private set; }
    public List<string> Faecher { get; internal set; }

    public Schueler_in(int id, string name, int alter, string klasse)
    {
        Id = id;
        Name = name;
        Alter = alter;
        Klasse = klasse;
        Faecher = new List<string>();
    }
}
