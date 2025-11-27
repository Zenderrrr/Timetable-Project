using Xunit;
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
        [Fact]
        public void Stundenplan_Constructor_InitializesEmptyMatrix()
        {
            // Arrange & Act
            var plan = new Stundenplan();

            // Assert
            Assert.NotNull(plan.Matrix);
            Assert.Equal(Stundenplan.TAGE, plan.Matrix.GetLength(0));
            Assert.Equal(Stundenplan.STUNDEN, plan.Matrix.GetLength(1));
            
            // Verify all cells are null
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    Assert.Null(plan.Matrix[t, s]);
                }
            }
        }

        [Fact]
        public void Stundenplan_IstFrei_ReturnsTrueForEmptySlot()
        {
            // Arrange
            var plan = new Stundenplan();

            // Act & Assert
            Assert.True(plan.IstFrei(0, 0));
            Assert.True(plan.IstFrei(2, 3));
            Assert.True(plan.IstFrei(4, 7));
        }

        [Fact]
        public void Stundenplan_IstFrei_ReturnsFalseForOccupiedSlot()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            plan.Eintragen(1, 2, stunde);

            // Act & Assert
            Assert.False(plan.IstFrei(1, 2));
        }

        [Fact]
        public void Stundenplan_Eintragen_SuccessfullyAddsStunde()
        {
            // Arrange
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

            // Act
            bool result = plan.Eintragen(0, 0, stunde);

            // Assert
            Assert.True(result);
            Assert.Equal(stunde, plan.Matrix[0, 0]);
        }

        [Fact]
        public void Stundenplan_Eintragen_FailsForOccupiedSlot()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            plan.Eintragen(1, 2, stunde1);

            // Act
            bool result = plan.Eintragen(1, 2, stunde2);

            // Assert
            Assert.False(result);
            Assert.Equal(stunde1, plan.Matrix[1, 2]);
        }

        [Fact]
        public void Stundenplan_BewertePlan_ReturnsMaxScoreForEmptyPlan()
        {
            // Arrange
            var plan = new Stundenplan();

            // Act
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);

            // Assert
            Assert.Equal(100.0, score);
            Assert.Equal(0.0, rand);
            Assert.Equal(0.0, zwischen);
            Assert.Equal(0.0, res);
        }

        [Fact]
        public void Stundenplan_BewertePlan_PenalizesRandstunden()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "A101", Klasse = "10A" };
            
            // First and last hour
            plan.Eintragen(0, 0, stunde1);
            plan.Eintragen(0, 7, stunde2);

            // Act
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);

            // Assert
            Assert.True(rand > 0); // Should have penalty
            Assert.Equal(2.0, rand); // Two edge hours
        }

        [Fact]
        public void Stundenplan_BewertePlan_PenalizesZwischenstunden()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "A101", Klasse = "10A" };
            
            // Create a gap: hour 1 and hour 3 occupied, hour 2 free
            plan.Eintragen(0, 1, stunde1);
            plan.Eintragen(0, 3, stunde2);

            // Act
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);

            // Assert
            Assert.True(zwischen > 0); // Should have penalty for gap
        }

        [Fact]
        public void Stundenplan_BewertePlan_PenalizesMultipleRooms()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            var stunde3 = new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A" };
            
            plan.Eintragen(0, 2, stunde1);
            plan.Eintragen(1, 2, stunde2);
            plan.Eintragen(2, 2, stunde3);

            // Act
            double score = plan.BewertePlan(out var rand, out var zwischen, out var res);

            // Assert
            Assert.True(res > 0); // Should have penalty for multiple rooms
        }

        [Fact]
        public void Stundenplan_ToList_ConvertsMatrixToList()
        {
            // Arrange
            var plan = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A" };
            var stunde3 = new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A" };
            
            plan.Eintragen(0, 1, stunde1); // Monday, 2nd hour
            plan.Eintragen(2, 3, stunde2); // Wednesday, 4th hour
            plan.Eintragen(4, 5, stunde3); // Friday, 6th hour

            // Act
            var list = plan.ToList();

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Contains(list, s => s.Fach == "Mathe" && s.Tag == "Montag" && s.StundeNummer == 2);
            Assert.Contains(list, s => s.Fach == "Deutsch" && s.Tag == "Mittwoch" && s.StundeNummer == 4);
            Assert.Contains(list, s => s.Fach == "Englisch" && s.Tag == "Freitag" && s.StundeNummer == 6);
        }

        [Fact]
        public void Stundenplan_FromList_CreatesMatrixFromList()
        {
            // Arrange
            var list = new List<Stunde>
            {
                new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A", Tag = "Montag", StundeNummer = 1 },
                new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A", Tag = "Mittwoch", StundeNummer = 3 },
                new Stunde { Fach = "Englisch", Lehrperson = "Jones", Raum = "C303", Klasse = "10A", Tag = "Freitag", StundeNummer = 5 }
            };

            // Act
            var plan = Stundenplan.FromList(list);

            // Assert
            Assert.NotNull(plan.Matrix[0, 0]); // Monday, 1st hour
            Assert.Equal("Mathe", plan.Matrix[0, 0].Fach);
            
            Assert.NotNull(plan.Matrix[2, 2]); // Wednesday, 3rd hour
            Assert.Equal("Deutsch", plan.Matrix[2, 2].Fach);
            
            Assert.NotNull(plan.Matrix[4, 4]); // Friday, 5th hour
            Assert.Equal("Englisch", plan.Matrix[4, 4].Fach);
        }

        [Fact]
        public void Stundenplan_FromList_HandlesNullList()
        {
            // Act
            var plan = Stundenplan.FromList(null);

            // Assert
            Assert.NotNull(plan);
            Assert.NotNull(plan.Matrix);
            
            // Verify all cells are null
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    Assert.Null(plan.Matrix[t, s]);
                }
            }
        }

        [Fact]
        public void Stundenplan_FromList_IgnoresInvalidTags()
        {
            // Arrange
            var list = new List<Stunde>
            {
                new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A", Tag = "InvalidDay", StundeNummer = 1 },
                new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10A", Tag = "", StundeNummer = 2 }
            };

            // Act
            var plan = Stundenplan.FromList(list);

            // Assert
            // Verify all cells are null since tags were invalid
            for (int t = 0; t < Stundenplan.TAGE; t++)
            {
                for (int s = 0; s < Stundenplan.STUNDEN; s++)
                {
                    Assert.Null(plan.Matrix[t, s]);
                }
            }
        }

        [Fact]
        public void Stundenplan_ToListAndBack_PreservesData()
        {
            // Arrange
            var plan1 = new Stundenplan();
            var stunde1 = new Stunde { Fach = "Mathe", Lehrperson = "Schmidt", Raum = "A101", Klasse = "10A" };
            var stunde2 = new Stunde { Fach = "Deutsch", Lehrperson = "Meyer", Raum = "B202", Klasse = "10B" };
            
            plan1.Eintragen(0, 1, stunde1);
            plan1.Eintragen(2, 3, stunde2);

            // Act
            var list = plan1.ToList();
            var plan2 = Stundenplan.FromList(list);

            // Assert
            Assert.NotNull(plan2.Matrix[0, 1]);
            Assert.Equal("Mathe", plan2.Matrix[0, 1].Fach);
            Assert.Equal("Schmidt", plan2.Matrix[0, 1].Lehrperson);
            
            Assert.NotNull(plan2.Matrix[2, 3]);
            Assert.Equal("Deutsch", plan2.Matrix[2, 3].Fach);
            Assert.Equal("Meyer", plan2.Matrix[2, 3].Lehrperson);
        }

        [Fact]
        public void Stundenplan_Constants_AreCorrect()
        {
            // Assert
            Assert.Equal(5, Stundenplan.TAGE);
            Assert.Equal(8, Stundenplan.STUNDEN);
        }

        [Fact]
        public void Stundenplan_DefaultWeights_AreSet()
        {
            // Arrange & Act
            var plan = new Stundenplan();

            // Assert
            Assert.Equal(1.0, plan.GewichtRandstunden);
            Assert.Equal(1.0, plan.GewichtZwischenstunden);
            Assert.Equal(0.2, plan.GewichtRessourcen);
        }

        [Fact]
        public void Stundenplan_Weights_CanBeModified()
        {
            // Arrange
            var plan = new Stundenplan();

            // Act
            plan.GewichtRandstunden = 2.0;
            plan.GewichtZwischenstunden = 1.5;
            plan.GewichtRessourcen = 0.5;

            // Assert
            Assert.Equal(2.0, plan.GewichtRandstunden);
            Assert.Equal(1.5, plan.GewichtZwischenstunden);
            Assert.Equal(0.5, plan.GewichtRessourcen);
        }
    }
}
