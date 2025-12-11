namespace TimeTableTests
{
    [TestClass]
    public sealed class TimeTableTest
    {
        [TestMethod]
        public void BewertetRandStundenSchlechter()
        {
            var plan = new Stundenplan();
            plan.Matrix[0, 0] = new Stunde { Fach = "Mathe" };

            var score = plan.BewertePlan(out var rand, out _, out _);

            Assert.IsTrue(rand > 0);
            Assert.IsTrue(score < 100);
        }

        [TestMethod]
        public void Bewertet_Zwischenstunden_Negativ()
        {
            var plan = new Stundenplan();
            plan.Matrix[0, 1] = new Stunde { Fach = "Mathe" };
            plan.Matrix[0, 3] = new Stunde { Fach = "Deutsch" };

            var score = plan.BewertePlan(out _, out var zwischen, out _);

            Assert.IsTrue(zwischen > 0);
            Assert.IsTrue(score < 100);
        }

        [TestMethod]
        public void Erkennt_Lehrperson_Kollision()
        {
            var plan = new Stundenplan();

            plan.Matrix[0, 0] = new Stunde { Lehrperson = "Müller", Tag = "Montag", StundeNummer = 1 };
            plan.Matrix[0, 0] = new Stunde { Lehrperson = "Müller", Tag = "Montag", StundeNummer = 1 };

            Assert.IsTrue(plan.HatLehrpersonKollision());
        }

        [TestMethod]
        public void Lehrperson_Dienstag_Nachmittag_Gesperrt()
        {
            var lp = new Lehrperson(1, "Meier");
            lp.SetzeVerfuegbarkeit("Dienstag", 6, false);

            Assert.IsFalse(lp.IstVerfuegbar("Dienstag", 6));
        }

        [TestMethod]
        public void Gueltiger_Plan_Wird_Akzeptiert()
        {
            var plan = new Stundenplan();
            plan.Matrix[0, 1] = new Stunde { Fach = "Mathe", Klasse = "1A", Lehrperson = "Meier" };

            var score = plan.BewertePlan(out _, out _, out _);

            Assert.IsTrue(score > 0);
        }
    }
}
