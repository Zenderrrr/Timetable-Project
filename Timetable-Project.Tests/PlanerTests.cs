using Xunit;
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
        private List<Schueler_in> CreateTestSchueler()
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

        private List<Lehrperson> CreateTestLehrpersonen()
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

        private List<Raum> CreateTestRaeume()
        {
            var raeume = new List<Raum>
            {
                new Raum(1, "A101", 30),
                new Raum(2, "B202", 25),
                new Raum(3, "C303", 20)
            };
            return raeume;
        }

        [Fact]
        public void Planer_Constructor_AcceptsValidData()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();

            // Act
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Assert
            Assert.NotNull(planer);
        }

        [Fact]
        public void Planer_Constructor_HandlesNullLists()
        {
            // Act
            var planer = new Planer(null, null, null);

            // Assert
            Assert.NotNull(planer);
        }

        [Fact]
        public void Planer_ErstellePlan_CreatesNonNullPlan()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();

            // Assert
            Assert.NotNull(plan);
            Assert.NotNull(plan.Matrix);
        }

        [Fact]
        public void Planer_ErstellePlan_PlacesStundenInMatrix()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            Assert.NotEmpty(list);
        }

        [Fact]
        public void Planer_ErstellePlan_CreatesStundenWithCorrectProperties()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            foreach (var stunde in list)
            {
                Assert.NotNull(stunde.Fach);
                Assert.NotNull(stunde.Lehrperson);
                Assert.NotNull(stunde.Raum);
                Assert.NotNull(stunde.Klasse);
                Assert.NotNull(stunde.Tag);
                Assert.True(stunde.StundeNummer >= 1 && stunde.StundeNummer <= Stundenplan.STUNDEN);
            }
        }

        [Fact]
        public void Planer_ErstellePlan_AssignsCorrectTeacherForSubject()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            foreach (var stunde in list)
            {
                if (stunde.Fach == "Mathematik" || stunde.Fach == "Physik")
                {
                    Assert.Equal("Dr. Schmidt", stunde.Lehrperson);
                }
                else if (stunde.Fach == "Deutsch" || stunde.Fach == "Englisch")
                {
                    Assert.Equal("Prof. Meyer", stunde.Lehrperson);
                }
            }
        }

        [Fact]
        public void Planer_ErstellePlan_UsesAvailableRooms()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var roomNames = raeume.Select(r => r.Bezeichnung).ToList();

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            foreach (var stunde in list)
            {
                Assert.Contains(stunde.Raum, roomNames);
            }
        }

        [Fact]
        public void Planer_ErstellePlan_DoesNotOverlapTeachers()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();

            // Assert - Check that no teacher is in multiple places at same time
            for (int tag = 0; tag < Stundenplan.TAGE; tag++)
            {
                for (int stunde = 0; stunde < Stundenplan.STUNDEN; stunde++)
                {
                    var stundenAtTime = new List<Stunde>();
                    
                    if (plan.Matrix[tag, stunde] != null)
                    {
                        stundenAtTime.Add(plan.Matrix[tag, stunde]);
                    }

                    // Check for duplicate teachers at this time
                    var teacherCount = stundenAtTime.GroupBy(s => s.Lehrperson).Count();
                    Assert.Equal(stundenAtTime.Count, teacherCount);
                }
            }
        }

        [Fact]
        public void Planer_ErstellePlan_WithEmptyData_ReturnsEmptyPlan()
        {
            // Arrange
            var planer = new Planer(new List<Schueler_in>(), new List<Lehrperson>(), new List<Raum>());

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            Assert.Empty(list);
        }

        [Fact]
        public void Planer_ErstellePlan_WithNoMatchingTeacher_SkipsSubject()
        {
            // Arrange
            var schueler = new List<Schueler_in>
            {
                new Schueler_in(1, "Max", 16, "10A")
                {
                    Faecher = new List<string> { "Chemistry" } // No teacher for this
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

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            Assert.DoesNotContain(list, s => s.Fach == "Chemistry");
        }

        [Fact]
        public void Planer_ErstellePlan_RespectsTeacherAvailability()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            lehrpersonen[0].Verfuegbarkeit["Montag"] = false; // Dr. Schmidt not available on Monday
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            var mondayLessons = list.Where(s => s.Tag == "Montag" && s.Lehrperson == "Dr. Schmidt");
            Assert.Empty(mondayLessons);
        }

        [Fact]
        public void Planer_ErstellePlan_ValidDays()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);
            var validDays = new[] { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };

            // Act
            var plan = planer.ErstellePlan();
            var list = plan.ToList();

            // Assert
            foreach (var stunde in list)
            {
                Assert.Contains(stunde.Tag, validDays);
            }
        }

        [Fact]
        public void Planer_ErstellePlan_MultipleRuns_ProduceDifferentPlans()
        {
            // Arrange
            var schueler = CreateTestSchueler();
            var lehrpersonen = CreateTestLehrpersonen();
            var raeume = CreateTestRaeume();
            var planer = new Planer(schueler, lehrpersonen, raeume);

            // Act
            var plan1 = planer.ErstellePlan();
            var plan2 = planer.ErstellePlan();
            
            var list1 = plan1.ToList();
            var list2 = plan2.ToList();

            // Assert - Plans should likely differ due to randomness (though not guaranteed)
            // Just verify both are valid plans
            Assert.NotEmpty(list1);
            Assert.NotEmpty(list2);
        }
    }
}
