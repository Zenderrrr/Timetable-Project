using System;
using System.Collections.Generic;
using Timetable_Project;

namespace Timetable_Project.Tests
{
    /// <summary>
    /// Tests for basic entity classes: Fach, Lehrperson, Raum, Schueler_in, Stunde
    /// </summary>
    public class EntityTests
    {
        private static int testsRun = 0;
        private static int testsPassed = 0;
        private static int testsFailed = 0;

        public static void RunAllTests()
        {
            Console.WriteLine("\n=== EntityTests ===\n");
            
            // Fach Tests
            Fach_Constructor_SetsPropertiesCorrectly();
            Fach_Properties_CanBeModified();
            
            // Lehrperson Tests
            Lehrperson_Constructor_SetsPropertiesCorrectly();
            Lehrperson_CanAddFaecher();
            Lehrperson_IstVerfuegbar_ReturnsTrue_WhenNoRestrictions();
            Lehrperson_IstVerfuegbar_RespectsVerfuegbarkeit();
            Lehrperson_IstVerfuegbar_ReturnsTrue_ForEmptyTag();
            
            // Raum Tests
            Raum_Constructor_SetsPropertiesCorrectly();
            Raum_VerfuegbarDefaultsToTrue();
            Raum_VerfuegbarCanBeChanged();
            
            // Schueler Tests
            Schueler_Constructor_SetsPropertiesCorrectly();
            Schueler_CanAddFaecher();
            
            // Stunde Tests
            Stunde_CanBeCreated();
            Stunde_PropertiesCanBeSet();

            Console.WriteLine($"\n--- EntityTests Summary ---");
            Console.WriteLine($"Tests Run: {testsRun}");
            Console.WriteLine($"Passed: {testsPassed}");
            Console.WriteLine($"Failed: {testsFailed}");
        }

        #region Test Helpers

        private static void Assert(bool condition, string testName, string message = "")
        {
            testsRun++;
            if (condition)
            {
                testsPassed++;
                Console.WriteLine($"✓ {testName}");
            }
            else
            {
                testsFailed++;
                Console.WriteLine($"✗ {testName}: {message}");
            }
        }

        #endregion

        #region Fach Tests

        public static void Fach_Constructor_SetsPropertiesCorrectly()
        {
            var fach = new Fach(1, "Mathematik", 5);
            Assert(fach.Id == 1 && fach.Title == "Mathematik" && fach.Wochenstunden == 5, 
                   "Fach_Constructor_SetsPropertiesCorrectly");
        }

        public static void Fach_Properties_CanBeModified()
        {
            var fach = new Fach(1, "Mathematik", 5);
            fach.Title = "Physik";
            fach.Wochenstunden = 3;
            Assert(fach.Title == "Physik" && fach.Wochenstunden == 3, 
                   "Fach_Properties_CanBeModified");
        }

        #endregion

        #region Lehrperson Tests

        public static void Lehrperson_Constructor_SetsPropertiesCorrectly()
        {
            var lp = new Lehrperson(1, "Dr. Schmidt");
            Assert(lp.Id == 1 && lp.Name == "Dr. Schmidt" && lp.Faecher != null && lp.Faecher.Count == 0, 
                   "Lehrperson_Constructor_SetsPropertiesCorrectly");
        }

        public static void Lehrperson_CanAddFaecher()
        {
            var lp = new Lehrperson(1, "Dr. Schmidt");
            lp.Faecher.Add("Mathematik");
            lp.Faecher.Add("Physik");
            Assert(lp.Faecher.Count == 2 && lp.Faecher.Contains("Mathematik") && lp.Faecher.Contains("Physik"), 
                   "Lehrperson_CanAddFaecher");
        }

        public static void Lehrperson_IstVerfuegbar_ReturnsTrue_WhenNoRestrictions()
        {
            var lp = new Lehrperson(1, "Dr. Schmidt");
            Assert(lp.IstVerfuegbar("Montag") && lp.IstVerfuegbar("Dienstag"), 
                   "Lehrperson_IstVerfuegbar_ReturnsTrue_WhenNoRestrictions");
        }

        public static void Lehrperson_IstVerfuegbar_RespectsVerfuegbarkeit()
        {
            var lp = new Lehrperson(1, "Dr. Schmidt");
            lp.Verfuegbarkeit["Montag"] = false;
            lp.Verfuegbarkeit["Dienstag"] = true;
            Assert(!lp.IstVerfuegbar("Montag") && lp.IstVerfuegbar("Dienstag"), 
                   "Lehrperson_IstVerfuegbar_RespectsVerfuegbarkeit");
        }

        public static void Lehrperson_IstVerfuegbar_ReturnsTrue_ForEmptyTag()
        {
            var lp = new Lehrperson(1, "Dr. Schmidt");
            Assert(lp.IstVerfuegbar("") && lp.IstVerfuegbar(null), 
                   "Lehrperson_IstVerfuegbar_ReturnsTrue_ForEmptyTag");
        }

        #endregion

        #region Raum Tests

        public static void Raum_Constructor_SetsPropertiesCorrectly()
        {
            var raum = new Raum(1, "A101", 30);
            Assert(raum.Id == 1 && raum.Bezeichnung == "A101" && raum.Kapazitaet == 30 && raum.Verfuegbar, 
                   "Raum_Constructor_SetsPropertiesCorrectly");
        }

        public static void Raum_VerfuegbarDefaultsToTrue()
        {
            var raum = new Raum(5, "B202", 25);
            Assert(raum.Verfuegbar, "Raum_VerfuegbarDefaultsToTrue");
        }

        public static void Raum_VerfuegbarCanBeChanged()
        {
            var raum = new Raum(1, "A101", 30);
            raum.Verfuegbar = false;
            Assert(!raum.Verfuegbar, "Raum_VerfuegbarCanBeChanged");
        }

        #endregion

        #region Schueler_in Tests

        public static void Schueler_Constructor_SetsPropertiesCorrectly()
        {
            var schueler = new Schueler_in(1, "Max Mustermann", 16, "10A");
            Assert(schueler.Id == 1 && schueler.Name == "Max Mustermann" && 
                   schueler.Alter == 16 && schueler.Klasse == "10A" && 
                   schueler.Faecher != null && schueler.Faecher.Count == 0, 
                   "Schueler_Constructor_SetsPropertiesCorrectly");
        }

        public static void Schueler_CanAddFaecher()
        {
            var schueler = new Schueler_in(1, "Max Mustermann", 16, "10A");
            schueler.Faecher.Add("Deutsch");
            schueler.Faecher.Add("Englisch");
            schueler.Faecher.Add("Mathematik");
            Assert(schueler.Faecher.Count == 3 && 
                   schueler.Faecher.Contains("Deutsch") && 
                   schueler.Faecher.Contains("Englisch") && 
                   schueler.Faecher.Contains("Mathematik"), 
                   "Schueler_CanAddFaecher");
        }

        #endregion

        #region Stunde Tests

        public static void Stunde_CanBeCreated()
        {
            var stunde = new Stunde
            {
                Fach = "Mathematik",
                Lehrperson = "Dr. Schmidt",
                Raum = "A101",
                Klasse = "10A",
                Tag = "Montag",
                StundeNummer = 1
            };
            Assert(stunde.Fach == "Mathematik" && stunde.Lehrperson == "Dr. Schmidt" && 
                   stunde.Raum == "A101" && stunde.Klasse == "10A" && 
                   stunde.Tag == "Montag" && stunde.StundeNummer == 1, 
                   "Stunde_CanBeCreated");
        }

        public static void Stunde_PropertiesCanBeSet()
        {
            var stunde = new Stunde();
            stunde.Fach = "Physik";
            stunde.Lehrperson = "Prof. Meyer";
            stunde.Raum = "B202";
            stunde.Klasse = "11B";
            stunde.Tag = "Dienstag";
            stunde.StundeNummer = 3;
            Assert(stunde.Fach == "Physik" && stunde.Lehrperson == "Prof. Meyer" && 
                   stunde.Raum == "B202" && stunde.Klasse == "11B" && 
                   stunde.Tag == "Dienstag" && stunde.StundeNummer == 3, 
                   "Stunde_PropertiesCanBeSet");
        }

        #endregion
    }
}
