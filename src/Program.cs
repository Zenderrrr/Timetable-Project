using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Timetable_Project.Validation;

namespace Timetable_Project
{
    /// <summary>
    /// Container-Klasse für alle Daten des Stundenplan-Systems
    /// </summary>
    public class Daten
    {
        public List<Fach> Faecher { get; set; } = new();
        public List<Lehrperson> Lehrpersonen { get; set; } = new();
        public List<Raum> Raeume { get; set; } = new();
        public List<Schueler_in> Schueler { get; set; } = new();
        public List<Stunde> Stunden { get; set; } = new();
    }

    class Program
    {
        const string DATEI = "daten.json";
        private static Daten daten = new Daten();

        static void Main()
        {
            LoadDaten();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== STUNDENPLAN MANAGER ===");
                Console.WriteLine("1. Daten anzeigen");
                Console.WriteLine("2. Fach hinzufügen");
                Console.WriteLine("3. Lehrperson hinzufügen");
                Console.WriteLine("4. Schüler hinzufügen");
                Console.WriteLine("5. Raum hinzufügen");
                Console.WriteLine("6. Verfügbarkeit verwalten");
                Console.WriteLine("7. Stundenplan generieren");
                Console.WriteLine("8. Stundenplan anzeigen");
                Console.WriteLine("0. Beenden");
                Console.Write("Auswahl: ");
                
                var input = Console.ReadLine();
                switch (input ?? "")
                {
                    case "1": ShowAllData(); break;
                    case "2": AddFach(); break;
                    case "3": AddLehrperson(); break;
                    case "4": AddSchueler(); break;
                    case "5": AddRaum(); break;
                    case "6": VerfuegbarkeitVerwalten(); break;
                    case "7": GeneratePlan(); break;
                    case "8": ShowPlan(); break;
                    case "0": SaveDaten(); return;
                    default: Console.WriteLine("Ungültige Auswahl!"); WaitForKey(); break;
                }
            }
        }

        static void ShowAllData()
        {
            Console.Clear();
            Console.WriteLine("=== ALLE DATEN ===");
            Console.WriteLine($"\nFÄCHER ({daten.Faecher.Count}):");
            foreach (var f in daten.Faecher)
                Console.WriteLine($"- {f.Title} ({f.Wochenstunden} WS)");

            Console.WriteLine($"\nLEHRPERSONEN ({daten.Lehrpersonen.Count}):");
            foreach (var l in daten.Lehrpersonen)
                Console.WriteLine($"- {l.Name}: {string.Join(", ", l.Faecher)}");

            Console.WriteLine($"\nSCHÜLER ({daten.Schueler.Count}):");
            foreach (var s in daten.Schueler)
                Console.WriteLine($"- {s.Name} ({s.Klasse}): {string.Join(", ", s.Faecher)}");

            Console.WriteLine($"\nRÄUME ({daten.Raeume.Count}):");
            foreach (var r in daten.Raeume)
                Console.WriteLine($"- {r.Bezeichnung} ({r.Kapazitaet} Plätze)");

            WaitForKey();
        }

        static void AddFach()
        {
            Console.Clear();
            Console.WriteLine("=== FACH HINZUFÜGEN ===");
            Console.Write("Fachname: ");
            string name = Console.ReadLine();
            Console.Write("Wochenstunden: ");
            int ws = int.Parse(Console.ReadLine());
            int id = daten.Faecher.Count > 0 ? daten.Faecher.Max(f => f.Id) + 1 : 1;
            var neuesFach = new Fach(id, name, ws);
            daten.Faecher.Add(neuesFach);
            SaveDaten();
            Console.WriteLine($"Fach '{name}' hinzugefügt! (ID: {id})");
            WaitForKey();
        }

        static void AddLehrperson()
        {
            Console.Clear();
            Console.WriteLine("=== LEHRPERSON HINZUFÜGEN ===");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            int id = daten.Lehrpersonen.Count > 0 ? daten.Lehrpersonen.Max(l => l.Id) + 1 : 1;
            var lp = new Lehrperson(id, name);

            Console.WriteLine("Verfügbare Fächer: " + string.Join(", ", daten.Faecher.Select(f => f.Title)));
            Console.Write("Fächer (komma-getrennt): ");
            string faecherInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(faecherInput))
            {
                foreach (var fachName in faecherInput.Split(','))
                {
                    string trimmed = fachName.Trim();
                    if (daten.Faecher.Any(f => f.Title.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
                    {
                        lp.Faecher.Add(trimmed);
                    }
                    else
                    {
                        Console.WriteLine($"Warnung: Fach '{trimmed}' existiert nicht in der Datenbank!");
                    }
                }
            }

            daten.Lehrpersonen.Add(lp);
            SaveDaten();
            Console.WriteLine("Lehrperson hinzugefügt!");
            WaitForKey();
        }

        static void AddSchueler()
        {
            Console.Clear();
            Console.WriteLine("=== SCHÜLER HINZUFÜGEN ===");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Alter: ");
            int alter = int.Parse(Console.ReadLine());
            Console.Write("Klasse: ");
            string klasse = Console.ReadLine();
            int id = daten.Schueler.Count > 0 ? daten.Schueler.Max(s => s.Id) + 1 : 1;
            var s = new Schueler_in(id, name, alter, klasse);
            
            if (daten.Faecher.Count > 0)
            {
                foreach (var fach in daten.Faecher)
                {
                    s.Faecher.Add(fach.Title);
                }
                Console.WriteLine($"Dem Schüler wurden automatisch alle {daten.Faecher.Count} Fächer zugewiesen.");
            }
            else
            {
                Console.WriteLine("Hinweis: Keine Fächer vorhanden. Bitte zuerst Fächer hinzufügen.");
            }
            
            daten.Schueler.Add(s);
            SaveDaten();
            Console.WriteLine("Schüler hinzugefügt!");
            WaitForKey();
        }

        static void AddRaum()
        {
            Console.Clear();
            Console.WriteLine("=== RAUM HINZUFÜGEN ===");
            Console.Write("Raum: ");
            string raum = Console.ReadLine();
            Console.Write("Kapazität: ");
            int kap = int.Parse(Console.ReadLine());
            int id = daten.Raeume.Count > 0 ? daten.Raeume.Max(r => r.Id) + 1 : 1;
            daten.Raeume.Add(new Raum(id, raum, kap));
            SaveDaten();
            Console.WriteLine("Raum hinzugefügt!");
            WaitForKey();
        }

        static void VerfuegbarkeitVerwalten()
        {
            Console.Clear();
            Console.WriteLine("=== VERFÜGBARKEIT VERWALTEN ===");
            Console.WriteLine("1. Lehrpersonen-Verfügbarkeit");
            Console.WriteLine("2. Raum-Verfügbarkeit");
            Console.Write("Auswahl: ");
            
            switch (Console.ReadLine())
            {
                case "1":
                    LehrpersonVerfuegbarkeit();
                    break;
                case "2":
                    RaumVerfuegbarkeit();
                    break;
                default:
                    Console.WriteLine("Ungültige Auswahl!");
                    WaitForKey();
                    break;
            }
        }

        static void LehrpersonVerfuegbarkeit()
        {
            Console.Clear();
            Console.WriteLine("=== LEHRPERSON VERFÜGBARKEIT ===");
            
            if (daten.Lehrpersonen.Count == 0)
            {
                Console.WriteLine("Keine Lehrpersonen vorhanden!");
                WaitForKey();
                return;
            }

            for (int i = 0; i < daten.Lehrpersonen.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {daten.Lehrpersonen[i].Name}");
            }
            
            Console.Write("Lehrperson auswählen: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= daten.Lehrpersonen.Count)
            {
                var lp = daten.Lehrpersonen[index - 1];
                BearbeiteLehrpersonVerfuegbarkeit(lp);
            }
        }

        static void BearbeiteLehrpersonVerfuegbarkeit(Lehrperson lp)
        {
            string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
            string[] zeiten = { "08:00-08:45", "08:45-09:30", "09:50-10:35", "10:35-11:20", 
                               "11:40-12:25", "12:25-13:10", "13:30-14:15", "14:15-15:00" };

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Verfügbarkeit für {lp.Name} (X = verfügbar, O = nicht verfügbar)");
                Console.WriteLine("Taste: (T)ag wechseln, (S)tunde umschalten, (B)eenden");
                
                for (int tag = 0; tag < tage.Length; tag++)
                {
                    Console.Write($"{tage[tag]}: ");
                    for (int stunde = 0; stunde < zeiten.Length; stunde++)
                    {
                        bool verfuegbar = lp.IstVerfuegbar(tage[tag], stunde);
                        Console.Write(verfuegbar ? "[X] " : "[O] ");
                    }
                    Console.WriteLine();
                }

                Console.Write("Aktion: ");
                var input = Console.ReadLine()?.ToLower() ?? "";
                
                if (input == "b") break;
                else if (input == "t")
                {
                    Console.Write("Tag (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int tag) && tag >= 1 && tag <= 5)
                    {
                        Console.Write("Stunde (1-8): ");
                        if (int.TryParse(Console.ReadLine(), out int stunde) && stunde >= 1 && stunde <= 8)
                        {
                            bool aktuell = lp.IstVerfuegbar(tage[tag - 1], stunde - 1);
                            lp.SetzeVerfuegbarkeit(tage[tag - 1], stunde - 1, !aktuell);
                            Console.WriteLine($"Verfügbarkeit für {tage[tag - 1]}, {stunde}. Stunde geändert!");
                            WaitForKey();
                        }
                    }
                }
            }
            SaveDaten();
        }

        static void RaumVerfuegbarkeit()
        {
            Console.Clear();
            Console.WriteLine("=== RAUM VERFÜGBARKEIT ===");
            
            if (daten.Raeume.Count == 0)
            {
                Console.WriteLine("Keine Räume vorhanden!");
                WaitForKey();
                return;
            }

            for (int i = 0; i < daten.Raeume.Count; i++)
            {
                var raum = daten.Raeume[i];
                Console.WriteLine($"{i + 1}. {raum.Bezeichnung} - {(raum.Verfuegbar ? "Verfügbar" : "Nicht verfügbar")}");
            }
            
            Console.Write("Raum auswählen: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= daten.Raeume.Count)
            {
                var raum = daten.Raeume[index - 1];
                raum.Verfuegbar = !raum.Verfuegbar;
                Console.WriteLine($"Raum {raum.Bezeichnung} ist jetzt {(raum.Verfuegbar ? "verfügbar" : "nicht verfügbar")}!");
                SaveDaten();
                WaitForKey();
            }
        }

        static void GeneratePlan()
    {
     Console.Clear();
        Console.WriteLine("=== STUNDENPLAN GENERIEREN ===");

        var validator = new DataValidator();
        var errors = validator.Validate(daten);

        if (errors.Any())
        {
            Console.WriteLine("FEHLERREPORT:");
            foreach (var error in errors)
                Console.WriteLine(" - " + error);

            WaitForKey();
            return;
        }

        var planer = new Planer(daten.Schueler, daten.Lehrpersonen, daten.Raeume);
    var plan = planer.ErstellePlan();

        daten.Stunden = plan.ToList();
        SaveDaten();
        Console.WriteLine($"Stundenplan generiert! {daten.Stunden.Count} Stunden.");
        WaitForKey();
    }

        static void ShowPlan()
        {
            Console.Clear();
            if (daten.Stunden == null || daten.Stunden.Count == 0)
            {
                Console.WriteLine("Kein Stundenplan vorhanden! Generiere zuerst einen Plan.");
            }
            else
            {
                var plan = Stundenplan.FromList(daten.Stunden);
                Console.WriteLine("=== STUNDENPLAN ===");
                Console.WriteLine("Wähle Anzeige:");
                Console.WriteLine("1. Tabellarische Übersicht");
                Console.WriteLine("2. Detail-Ansicht (mit allen Infos)");
                Console.Write("Auswahl: ");
                var input = Console.ReadLine();
                if (input == "2")
                {
                    plan.AnzeigenDetail();
                }
                else
                {
                    plan.Anzeigen();
                }
                var score = plan.BewertePlan(out var r, out var z, out var res);
                Console.WriteLine($"\nBewertung: {score:F2}");
            }
            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.WriteLine("\nDrücke eine Taste um fortzufahren...");
            Console.ReadKey();
        }

        /// <summary>
        /// Lädt die Anwendungsdaten aus der JSON-Datei
        /// </summary>
        static void LoadDaten()
        {
            try
            {
                if (File.Exists(DATEI))
                {
                    string json = File.ReadAllText(DATEI);
                    daten = JsonSerializer.Deserialize<Daten>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Daten();
                    Console.WriteLine($"Daten geladen: {daten.Faecher.Count} Fächer, {daten.Schueler.Count} Schüler");
                }
                else
                {
                    Console.WriteLine("Neue Datenbank erstellt");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden: {ex.Message}");
                daten = new Daten();
            }
        }

        /// <summary>
        /// Speichert die Anwendungsdaten in eine JSON-Datei
        /// </summary>
        static void SaveDaten()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(daten, options);
                File.WriteAllText(DATEI, json);
                Console.WriteLine("Daten gespeichert!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern: {ex.Message}");
            }
        }
    }
}