using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Timetable_Project
{
    class Program
    {
        const string DATEI = "daten.json";

        static void Main()
        {
            // Daten aus JSON (wenn vorhanden) laden, andernfalls Beispiel erstellen
            Daten daten;
            if (File.Exists(DATEI))
            {
                try
                {
                    var json = File.ReadAllText(DATEI);
                    daten = JsonSerializer.Deserialize<Daten>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (daten == null) daten = ErstelleBeispielDaten();
                    Console.WriteLine("Daten aus 'daten.json' geladen.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Laden von 'daten.json': " + ex.Message);
                    daten = ErstelleBeispielDaten();
                }
            }
            else
            {
                daten = ErstelleBeispielDaten();
                SaveDaten(daten);
                Console.WriteLine("Beispiel-Daten erstellt und gespeichert in 'daten.json'.");
            }

            // falls es bereits einen gespeicherten Stundenplan gibt, diesen laden und anzeigen
            if (daten.Stunden != null && daten.Stunden.Count > 0)
            {
                var planGespeichert = Stundenplan.FromList(daten.Stunden);

                
                if (daten.GewichtRandstunden.HasValue) planGespeichert.GewichtRandstunden = daten.GewichtRandstunden.Value;
                if (daten.GewichtZwischenstunden.HasValue) planGespeichert.GewichtZwischenstunden = daten.GewichtZwischenstunden.Value;
                if (daten.GewichtRessourcen.HasValue) planGespeichert.GewichtRessourcen = daten.GewichtRessourcen.Value;

                Console.WriteLine("\n=== Gespeicherter Stundenplan ===");
                planGespeichert.Anzeigen();
                var score = planGespeichert.BewertePlan(out var r, out var z, out var res);
                Console.WriteLine($"\nBewertung: {score:F2} (Rand: {r:F2}, Zwischen: {z:F2}, Ressourcen: {res:F2})");
                return;
            }

            
            var planer = new Planer(daten.Schueler, daten.Lehrpersonen, daten.Raeume);
            var planNeu = planer.ErstellePlan();

            
            planNeu.GewichtRandstunden = 1.5;
            planNeu.GewichtZwischenstunden = 1.0;
            planNeu.GewichtRessourcen = 0.3;

            Console.WriteLine("\n=== Neuer erzeugter Stundenplan ===");
            planNeu.Anzeigen();

            var scoreNeu = planNeu.BewertePlan(out var rr, out var zz, out var rres);
            Console.WriteLine($"\nBewertung: {scoreNeu:F2} (Rand: {rr:F2}, Zwischen: {zz:F2}, Ressourcen: {rres:F2})");

            // daten und plan in daten.json speichern
            daten.Stunden = planNeu.ToList();

            // Gewichtungen speichern
            daten.GewichtRandstunden = planNeu.GewichtRandstunden;
            daten.GewichtZwischenstunden = planNeu.GewichtZwischenstunden;
            daten.GewichtRessourcen = planNeu.GewichtRessourcen;

            SaveDaten(daten);
            Console.WriteLine("\nPlan wurde in 'daten.json' gespeichert.");
            
        }

        static void SaveDaten(Daten d)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(d, options);
            File.WriteAllText(DATEI, json);
        }

        static Daten ErstelleBeispielDaten()
        {
            var f1 = new Fach(1, "Mathematik", 3);
            var f2 = new Fach(2, "Deutsch", 3);
            var f3 = new Fach(3, "Englisch", 2);

            var l1 = new Lehrperson(1, "Herr Meier");
            l1.Faecher.Add("Mathematik");

            var l2 = new Lehrperson(2, "Frau Schulz");
            l2.Faecher.Add("Deutsch");
            l2.Verfuegbarkeit["Dienstag"] = false;

            var l3 = new Lehrperson(3, "Frau Becker");
            l3.Faecher.Add("Englisch");

            var r1 = new Raum(1, "R101", 30);
            var r2 = new Raum(2, "R202", 25);

            var s1 = new Schueler_in(1, "Anna", 15, "10A");
            s1.Faecher.AddRange(new[] { "Mathematik", "Deutsch" });

            var s2 = new Schueler_in(2, "Lukas", 16, "10B");
            s2.Faecher.AddRange(new[] { "Mathematik", "Englisch" });

            return new Daten
            {
                Faecher = new List<Fach> { f1, f2, f3 },
                Lehrpersonen = new List<Lehrperson> { l1, l2, l3 },
                Raeume = new List<Raum> { r1, r2 },
                Schueler = new List<Schueler_in> { s1, s2 },
                Stunden = new List<Stunde>()
            };
        }
    }

    
    public class Daten
    {
        public List<Fach> Faecher { get; set; } = new();
        public List<Lehrperson> Lehrpersonen { get; set; } = new();
        public List<Raum> Raeume { get; set; } = new();
        public List<Schueler_in> Schueler { get; set; } = new();

        
        public List<Stunde> Stunden { get; set; } = new();

       
        public double? GewichtRandstunden { get; set; }
        public double? GewichtZwischenstunden { get; set; }
        public double? GewichtRessourcen { get; set; }
    }
}
