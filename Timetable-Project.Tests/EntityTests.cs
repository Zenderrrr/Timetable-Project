using System;
using System.Collections.Generic;
using Xunit;
using Timetable_Project;

namespace Timetable_Project.Tests
{
    /// <summary>
    /// Tests for basic entity classes: Fach, Lehrperson, Raum, Schueler_in, Stunde
    /// </summary>
    public class EntityTests
    {
        #region Fach Tests

        [Fact]
        public void Fach_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var fach = new Fach(1, "Mathematik", 5);

            // Assert
            Assert.Equal(1, fach.Id);
            Assert.Equal("Mathematik", fach.Title);
            Assert.Equal(5, fach.Wochenstunden);
        }

        [Fact]
        public void Fach_Properties_CanBeModified()
        {
            // Arrange
            var fach = new Fach(1, "Mathematik", 5);

            // Act
            fach.Title = "Physik";
            fach.Wochenstunden = 3;

            // Assert
            Assert.Equal("Physik", fach.Title);
            Assert.Equal(3, fach.Wochenstunden);
        }

        #endregion

        #region Lehrperson Tests

        [Fact]
        public void Lehrperson_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var lp = new Lehrperson(1, "Dr. Schmidt");

            // Assert
            Assert.Equal(1, lp.Id);
            Assert.Equal("Dr. Schmidt", lp.Name);
            Assert.NotNull(lp.Faecher);
            Assert.Empty(lp.Faecher);
        }

        [Fact]
        public void Lehrperson_CanAddFaecher()
        {
            // Arrange
            var lp = new Lehrperson(1, "Dr. Schmidt");

            // Act
            lp.Faecher.Add("Mathematik");
            lp.Faecher.Add("Physik");

            // Assert
            Assert.Equal(2, lp.Faecher.Count);
            Assert.Contains("Mathematik", lp.Faecher);
            Assert.Contains("Physik", lp.Faecher);
        }

        [Fact]
        public void Lehrperson_IstVerfuegbar_ReturnsTrue_WhenNoRestrictions()
        {
            // Arrange
            var lp = new Lehrperson(1, "Dr. Schmidt");

            // Act & Assert
            Assert.True(lp.IstVerfuegbar("Montag"));
            Assert.True(lp.IstVerfuegbar("Dienstag"));
        }

        [Fact]
        public void Lehrperson_IstVerfuegbar_RespectsVerfuegbarkeit()
        {
            // Arrange
            var lp = new Lehrperson(1, "Dr. Schmidt");
            lp.Verfuegbarkeit["Montag"] = false;
            lp.Verfuegbarkeit["Dienstag"] = true;

            // Act & Assert
            Assert.False(lp.IstVerfuegbar("Montag"));
            Assert.True(lp.IstVerfuegbar("Dienstag"));
        }

        [Fact]
        public void Lehrperson_IstVerfuegbar_ReturnsTrue_ForEmptyTag()
        {
            // Arrange
            var lp = new Lehrperson(1, "Dr. Schmidt");

            // Act & Assert
            Assert.True(lp.IstVerfuegbar(""));
            Assert.True(lp.IstVerfuegbar(null));
        }

        #endregion

        #region Raum Tests

        [Fact]
        public void Raum_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var raum = new Raum(1, "A101", 30);

            // Assert
            Assert.Equal(1, raum.Id);
            Assert.Equal("A101", raum.Bezeichnung);
            Assert.Equal(30, raum.Kapazitaet);
            Assert.True(raum.Verfuegbar);
        }

        [Fact]
        public void Raum_VerfuegbarDefaultsToTrue()
        {
            // Arrange & Act
            var raum = new Raum(5, "B202", 25);

            // Assert
            Assert.True(raum.Verfuegbar);
        }

        [Fact]
        public void Raum_VerfuegbarCanBeChanged()
        {
            // Arrange
            var raum = new Raum(1, "A101", 30);

            // Act
            raum.Verfuegbar = false;

            // Assert
            Assert.False(raum.Verfuegbar);
        }

        #endregion

        #region Schueler_in Tests

        [Fact]
        public void Schueler_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var schueler = new Schueler_in(1, "Max Mustermann", 16, "10A");

            // Assert
            Assert.Equal(1, schueler.Id);
            Assert.Equal("Max Mustermann", schueler.Name);
            Assert.Equal(16, schueler.Alter);
            Assert.Equal("10A", schueler.Klasse);
            Assert.NotNull(schueler.Faecher);
            Assert.Empty(schueler.Faecher);
        }

        [Fact]
        public void Schueler_CanAddFaecher()
        {
            // Arrange
            var schueler = new Schueler_in(1, "Max Mustermann", 16, "10A");

            // Act
            schueler.Faecher.Add("Deutsch");
            schueler.Faecher.Add("Englisch");
            schueler.Faecher.Add("Mathematik");

            // Assert
            Assert.Equal(3, schueler.Faecher.Count);
            Assert.Contains("Deutsch", schueler.Faecher);
            Assert.Contains("Englisch", schueler.Faecher);
            Assert.Contains("Mathematik", schueler.Faecher);
        }

        #endregion

        #region Stunde Tests

        [Fact]
        public void Stunde_CanBeCreated()
        {
            // Arrange & Act
            var stunde = new Stunde
            {
                Fach = "Mathematik",
                Lehrperson = "Dr. Schmidt",
                Raum = "A101",
                Klasse = "10A",
                Tag = "Montag",
                StundeNummer = 1
            };

            // Assert
            Assert.Equal("Mathematik", stunde.Fach);
            Assert.Equal("Dr. Schmidt", stunde.Lehrperson);
            Assert.Equal("A101", stunde.Raum);
            Assert.Equal("10A", stunde.Klasse);
            Assert.Equal("Montag", stunde.Tag);
            Assert.Equal(1, stunde.StundeNummer);
        }

        [Fact]
        public void Stunde_PropertiesCanBeSet()
        {
            // Arrange
            var stunde = new Stunde();

            // Act
            stunde.Fach = "Physik";
            stunde.Lehrperson = "Prof. Meyer";
            stunde.Raum = "B202";
            stunde.Klasse = "11B";
            stunde.Tag = "Dienstag";
            stunde.StundeNummer = 3;

            // Assert
            Assert.Equal("Physik", stunde.Fach);
            Assert.Equal("Prof. Meyer", stunde.Lehrperson);
            Assert.Equal("B202", stunde.Raum);
            Assert.Equal("11B", stunde.Klasse);
            Assert.Equal("Dienstag", stunde.Tag);
            Assert.Equal(3, stunde.StundeNummer);
        }

        #endregion
    }
}
