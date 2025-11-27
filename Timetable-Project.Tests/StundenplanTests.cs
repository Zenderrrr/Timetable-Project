using Timetable_Project;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Timetable_Project.Tests
{
    /// <summary>
    /// Tests for the Stundenplan class including matrix operations, scoring, and serialization
    /// </summary>
    public class StundenplanTests
    {
        private static int testsRun = 0;
        private static int testsPassed = 0;
        private static int testsFailed = 0;

        public static void RunAllTests()
        {
            Console.WriteLine("\n=== StundenplanTests ===\n");
            
            Stundenplan_Constructor_InitializesEmptyMatrix();
            Stundenplan_IstFrei_ReturnsTrueForEmptySlot();
            Stundenplan_IstFrei_ReturnsFalseForOccupiedSlot();
            Stundenplan_Eintragen_SuccessfullyAddsStunde();
            Stundenplan_Eintragen_FailsForOccupiedSlot();
            Stundenplan_BewertePlan_ReturnsMaxScoreForEmptyPlan();
            Stundenplan_BewertePlan_PenalizesRandstunden();
            Stundenplan_BewertePlan_PenalizesZwischenstunden();
            Stundenplan_BewertePlan_PenalizesMultipleRooms();
            Stundenplan_ToList_ConvertsMatrixToList();
            Stundenplan_FromList_CreatesMatrixFromList();
            Stundenplan_FromList_HandlesNullList();
            Stundenplan_FromList_IgnoresInvalidTags();
            Stundenplan_ToListAndBack_PreservesData();
            Stundenplan_Constants_AreCorrect();
            Stundenplan_DefaultWeights_AreSet();
            Stundenplan_Weights_CanBeModified();

            Console.WriteLine($"\n--- StundenplanTests Summary ---");
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

        public static void Stundenplan_Constructor_InitializesEmptyMatrix()
        {
            var plan = new Stundenplan();
            bool allNull = true;
            
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    if (plan.Matrix[t, s] != null) allNull = false;
                }
            }
            
            Assert(plan.Matrix != null && 
                   plan.Matrix.GetLength(0) == Stundenplan.TAGE && 
                   plan.Matrix.GetLength(1) == Stundenplan.STUNDEN && 
                   allNull, 
                   "Stundenplan_Constructor_InitializesEmptyMatrix");
        }

        public static void Stundenplan_IstFrei_ReturnsTrueForEmptySlot()
        {
            var plan = new Stundenplan();
            Assert(plan.IstFrei(0, 0) && plan.IstFrei(2, 3) && plan.IstFrei(4, 7), 
                   "Stundenplan_IstFrei_ReturnsTrueForEmptySlot");
        }

        public static void Stundenplan_IstFrei_ReturnsFalseForOccupiedSlot()
        {
            var plan = new Stundenplan();
            var stunde = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            plan.Eintragen(1, 2, stunde);
            Assert(!plan.IstFrei(1, 2), "Stundenplan_IstFrei_ReturnsFalseForOccupiedSlot");
        }

        public static void Stundenplan_Eintragen_SuccessfullyAddsStunde()
        {
            var plan = new Stundenplan();
            var stunde = new Stunde 
            { 
                Fach = "Mathematik", 
                Lehrperson = "Dr. Schmidt", 
                Raum = "A101", 
                Klasse = "10A",
                Tag = "Montag",
                StundeNummer = 1
            };
            bool result = plan.Eintragen(0, 0, stunde);
            Assert(result && plan.Matrix[0, 0] == stunde, 
                   "Stundenplan_Eintragen_SuccessfullyAddsStunde");
        }

        public static void Stundenplan_Eintragen_FailsForOccupiedSlot()
        {
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            plan.Eintragen(1, 2, stunde1);
            bool result = plan.Eintragen(1, 2, stunde2);
            Assert(!result && plan.Matrix[1, 2] == stunde1, 
                   "Stundenplan_Eintragen_FailsForOccupiedSlot");
        }

        public static void Stundenplan_BewertePlan_ReturnsMaxScoreForEmptyPlan()
        {
            var plan = new Stundenplan();
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);
            Assert(score == 100.0 && rand == 0.0 && zwischen == 0.0 && res == 0.0, 
                   "Stundenplan_BewertePlan_ReturnsMaxScoreForEmptyPlan");
        }

        public static void Stundenplan_BewertePlan_PenalizesRandstunden()
        {
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "A101", Klasse = "10A" };
            plan.Eintragen(0, 0, stunde1);
            plan.Eintragen(0, 7, stunde2);
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);
            Assert(rand > 0 && rand == 2.0, 
                   "Stundenplan_BewertePlan_PenalizesRandstunden", 
                   $"Expected rand penalty > 0 and == 2.0, got {rand}");
        }

        public static void Stundenplan_BewertePlan_PenalizesZwischenstunden()
        {
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "A101", Klasse = "10A" };
            plan.Eintragen(0, 1, stunde1);
            plan.Eintragen(0, 3, stunde2);
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);
            Assert(zwischen > 0, 
                   "Stundenplan_BewertePlan_PenalizesZwischenstunden", 
                   $"Expected zwischen penalty > 0, got {zwischen}");
        }

        public static void Stundenplan_BewertePlan_PenalizesMultipleRooms()
        {
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            var stunde3 = new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A" };
            plan.Eintragen(0, 2, stunde1);
            plan.Eintragen(1, 2, stunde2);
            plan.Eintragen(2, 2, stunde3);
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);
            Assert(res > 0, 
                   "Stundenplan_BewertePlan_PenalizesMultipleRooms", 
                   $"Expected res penalty > 0, got {res}");
        }

        public static void Stundenplan_ToList_ConvertsMatrixToList()
        {
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            var stunde3 = new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A" };
            
            plan.Eintragen(0, 1, stunde1);
            plan.Eintragen(2, 3, stunde2);
            plan.Eintragen(4, 5, stunde3);
            
            var list = plan.ToList();
            
            Assert(list.Count == 3 && 
                   list.Any(s => s.Fach == "Mathe" && s.Tag == "Montag" && s.StundeNummer == 2) &&
                   list.Any(s => s.Fach == "Deutsch" && s.Tag == "Mittwoch" && s.StundeNummer == 4) &&
                   list.Any(s => s.Fach == "Englisch" && s.Tag == "Freitag" && s.StundeNummer == 6), 
                   "Stundenplan_ToList_ConvertsMatrixToList");
        }

        public static void Stundenplan_FromList_CreatesMatrixFromList()
        {
            var list = new List<Stunde>
            {
                new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A", Tag = "Montag", StundeNummer = 1 },
                new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A", Tag = "Mittwoch", StundeNummer = 3 },
                new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A", Tag = "Freitag", StundeNummer = 5 }
            };
            
            var plan = Stundenplan.FromList(list);
            
            Assert(plan.Matrix[0, 0] != null && plan.Matrix[0, 0].Fach == "Mathe" &&
                   plan.Matrix[2, 2] != null && plan.Matrix[2, 2].Fach == "Deutsch" &&
                   plan.Matrix[4, 4] != null && plan.Matrix[4, 4].Fach == "Englisch", 
                   "Stundenplan_FromList_CreatesMatrixFromList");
        }

        public static void Stundenplan_FromList_HandlesNullList()
        {
            var plan = Stundenplan.FromList(null);
            bool allNull = true;
            
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    if (plan.Matrix[t, s] != null) allNull = false;
                }
            }
            
            Assert(plan != null && plan.Matrix != null && allNull, 
                   "Stundenplan_FromList_HandlesNullList");
        }

        public static void Stundenplan_FromList_IgnoresInvalidTags()
        {
            var list = new List<Stunde>
            {
                new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A", Tag = "InvalidDay", StundeNummer = 1 },
                new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A", Tag = "", StundeNummer = 2 }
            };
            
            var plan = Stundenplan.FromList(list);
            bool allNull = true;
            
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    if (plan.Matrix[t, s] != null) allNull = false;
                }
            }
            
            Assert(allNull, "Stundenplan_FromList_IgnoresInvalidTags");
        }

        public static void Stundenplan_ToListAndBack_PreservesData()
        {
            var plan1 = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10B" };
            
            plan1.Eintragen(0, 1, stunde1);
            plan1.Eintragen(2, 3, stunde2);
            
            var list = plan1.ToList();
            var plan2 = Stundenplan.FromList(list);
            
            Assert(plan2.Matrix[0, 1] != null && plan2.Matrix[0, 1].Fach == "Mathe" && plan2.Matrix[0, 1].Lehrperson == "Schmidt" &&
                   plan2.Matrix[2, 3] != null && plan2.Matrix[2, 3].Fach == "Deutsch" && plan2.Matrix[2, 3].Lehrperson == "Meyer", 
                   "Stundenplan_ToListAndBack_PreservesData");
        }

        public static void Stundenplan_Constants_AreCorrect()
        {
            Assert(Stundenplan.TAGE == 5 && Stundenplan.STUNDEN == 8, 
                   "Stundenplan_Constants_AreCorrect");
        }

        public static void Stundenplan_DefaultWeights_AreSet()
        {
            var plan = new Stundenplan();
            Assert(plan.GewichtRandstunden == 1.0 && 
                   plan.GewichtZwischenstunden == 1.0 && 
                   plan.GewichtRessourcen == 0.2, 
                   "Stundenplan_DefaultWeights_AreSet");
        }

        public static void Stundenplan_Weights_CanBeModified()
        {
            var plan = new Stundenplan();
            plan.GewichtRandstunden = 2.0;
            plan.GewichtZwischenstunden = 1.5;
            plan.GewichtRessourcen = 0.5;
            Assert(plan.GewichtRandstunden == 2.0 && 
                   plan.GewichtZwischenstunden == 1.5 && 
                   plan.GewichtRessourcen == 0.5, 
                   "Stundenplan_Weights_CanBeModified");
        }
    }
}
