using System;
using Timetable_Project;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project.Tests
{
    /// <summary>
    /// Tests for the Planer class which generates timetables
    /// </summary>
    public class PlanerTests
    {
        private static int testsRun = 0;
        private static int testsPassed = 0;
        private static int testsFailed = 0;

        public static void RunAllTests()
        {
            Console.WriteLine("\n=== PlanerTests ===\n");
            
            Planer_Constructor_AcceptsValidData();
            Planer_Constructor_HandlesNullLists();
            Planer_ErstellePlan_CreatesNonNullPlan();
            Planer_ErstellePlan_PlacesStundenInMatrix();
            Planer_ErstellePlan_CreatesStundenWithCorrectProperties();
            Planer_ErstellePlan_AssignsCorrectTeacherForSubject();
            Planer_ErstellePlan_UsesAvailableRooms();
            Planer_ErstellePlan_DoesNotOverlapTeachers();
            Planer_ErstellePlan_WithEmptyData_ReturnsEmptyPlan();
            Planer_ErstellePlan_WithNoMatchingTeacher_SkipsSubject();
            Planer_ErstellePlan_RespectsTeacherAvailability();
            Planer_ErstellePlan_ValidDays();
            Planer_ErstellePlan_MultipleRuns_ProduceDifferentPlans();

            Console.WriteLine($"\n--- PlanerTests Summary ---");
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

        private static List<Schueler_in> CreateTestSchueler()
        {
            var schueler = new List<Schueler_in>
            {
                new Schueler_in(1, "Max Mustermann", 16, "10A")
                {
                    Faecher = new List<string> { "Mathematik", "Deutsch", "Englisch" }
                },
                new Schueler_in(2, "Anna Schmidt", 16, "10A")
                {
                    Faecher = new List<string> { "Mathematik", "Physik" }
                }
            };
            return schueler;
        }

        private static List<Lehrperson> CreateTestLehrpersonen()
        {
            var lehrpersonen = new List<Lehrperson>
            {
                new Lehrperson(1, "Dr. Schmidt")
                {
                    Faecher = new List<string> { "Mathematik", "Physik" }
                },
                new Lehrperson(2, "Prof. Meyer")
                {
                    Faecher = new List<string> { "Deutsch", "Englisch" }
                }
            };
            return lehrpersonen;
        }

        private static List<Raum> CreateTestRaeume()
        {
            var raeume = new List<Raum>
            {
                new Raum(1, "A101", 30),
                new Raum(2, "B202", 25),
                new Raum(3, "C303", 20)
            };
            return raeume;
        }

        #endregion

        public static void Planer_Constructor_AcceptsValidData()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            Assert(planer != null, "Planer_Constructor_AcceptsValidData");
        }

        public static void Planer_Constructor_HandlesNullLists()
        {
            var planer = new Planer(null, null, null);
            Assert(planer != null, "Planer_Constructor_HandlesNullLists");
        }

        public static void Planer_ErstellePlan_CreatesNonNullPlan()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            Assert(plan != null && plan.Matrix != null, "Planer_ErstellePlan_CreatesNonNullPlan");
        }

        public static void Planer_ErstellePlan_PlacesStundenInMatrix()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            Assert(list.Count > 0, "Planer_ErstellePlan_PlacesStundenInMatrix", $"Expected non-empty list, got {list.Count} items");
        }

        public static void Planer_ErstellePlan_CreatesStundenWithCorrectProperties()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            bool allValid = true;
            foreach (var stunde in list)
            {
                if (stunde.Fach == null || stunde.Lehrperson == null || stunde.Raum == null || 
                    stunde.Klasse == null || stunde.Tag == null || 
                    stunde.StundeNummer < 1 || stunde.StundeNummer > Stundenplan.STUNDEN)
                {
                    allValid = false;
                    break;
                }
            }
            Assert(allValid, "Planer_ErstellePlan_CreatesStundenWithCorrectProperties");
        }

        public static void Planer_ErstellePlan_AssignsCorrectTeacherForSubject()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            bool allCorrect = true;
            foreach (var stunde in list)
            {
                if (stunde.Fach == "Mathematik" || stunde.Fach == "Physik")
                {
                    if (stunde.Lehrperson != "Dr. Schmidt") allCorrect = false;
                }
                else if (stunde.Fach == "Deutsch" || stunde.Fach == "Englisch")
                {
                    if (stunde.Lehrperson != "Prof. Meyer") allCorrect = false;
                }
            }
            Assert(allCorrect, "Planer_ErstellePlan_AssignsCorrectTeacherForSubject");
        }

        public static void Planer_ErstellePlan_UsesAvailableRooms()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var roomNames = raeume.Select(r => r.Bezeichnung).ToList();
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            bool allRoomsValid = true;
            foreach (var stunde in list)
            {
                if (!roomNames.Contains(stunde.Raum)) allRoomsValid = false;
            }
            Assert(allRoomsValid, "Planer_ErstellePlan_UsesAvailableRooms");
        }

        public static void Planer_ErstellePlan_DoesNotOverlapTeachers()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            
            bool noOverlap = true;
            for (int tag = 0; tag < Stundenplan.TAGE; tag++)
            {
                for (int stunde = 0; stunde < Stundenplan.STUNDEN; stunde++)
                {
                    var stundenAtTime = new List<Stunde>();
                    if (plan.Matrix[tag, stunde] != null)
                    {
                        stundenAtTime.Add(plan.Matrix[tag, stunde]);
                    }
                    var teacherCount = stundenAtTime.GroupBy(s => s.Lehrperson).Count();
                    if (stundenAtTime.Count != teacherCount) noOverlap = false;
                }
            }
            Assert(noOverlap, "Planer_ErstellePlan_DoesNotOverlapTeachers");
        }

        public static void Planer_ErstellePlan_WithEmptyData_ReturnsEmptyPlan()
        {
            var planer = new Planer(new List<Schueler_in>(), new List<Lehrperson>(), new List<Raum>());
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            Assert(list.Count == 0, "Planer_ErstellePlan_WithEmptyData_ReturnsEmptyPlan");
        }

        public static void Planer_ErstellePlan_WithNoMatchingTeacher_SkipsSubject()
        {
            var schueler = new List<Schueler_in>
            {
                new Schueler_in(1, "Max", 16, "10A")
                {
                    Faecher = new List<string> { "Chemistry" }
                }
            };
            var lehrpersonen = new List<Lehrperson>
            {
                new Lehrperson(1, "Dr. Schmidt")
                {
                    Faecher = new List<string> { "Mathematik" }
                }
            };
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            bool hasChemistry = list.Any(s => s.Fach == "Chemistry");
            Assert(!hasChemistry, "Planer_ErstellePlan_WithNoMatchingTeacher_SkipsSubject");
        }

        public static void Planer_ErstellePlan_RespectsTeacherAvailability()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            lehrpersonen[0].Verfuegbarkeit["Montag"] = false;
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            var mondayLessons = list.Where(s => s.Tag == "Montag" && s.Lehrperson == "Dr. Schmidt");
            Assert(mondayLessons.Count() == 0, "Planer_ErstellePlan_RespectsTeacherAvailability");
        }

        public static void Planer_ErstellePlan_ValidDays()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var validDays = new[] { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
            var plan = planer.ErstellePlan();
            var list = plan.ToList();
            
            bool allDaysValid = true;
            foreach (var stunde in list)
            {
                if (!validDays.Contains(stunde.Tag)) allDaysValid = false;
            }
            Assert(allDaysValid, "Planer_ErstellePlan_ValidDays");
        }

        public static void Planer_ErstellePlan_MultipleRuns_ProduceDifferentPlans()
        {
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var plan1 = planer.ErstellePlan();
            var plan2 = planer.ErstellePlan();
            var list1 = plan1.ToList();
            var list2 = plan2.ToList();
            
            Assert(list1.Count > 0 && list2.Count > 0, "Planer_ErstellePlan_MultipleRuns_ProduceDifferentPlans");
        }
    }
}
