using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    public class Planer
    {
        private readonly List<Schueler_in> schueler;
        private readonly List<Lehrperson> lehrpersonen;
        private readonly List<Raum> raeume;
        private readonly Random random = new();

        public Planer(List<Schueler_in> schueler, List<Lehrperson> lehrpersonen, List<Raum> raeume)
        {
            this.schueler = schueler ?? new List<Schueler_in>();
            this.lehrpersonen = lehrpersonen ?? new List<Lehrperson>();
            this.raeume = raeume ?? new List<Raum>();
        }

        // Algorithmus zur Stundenplanerstellung
        // Einige Logik in dieser Methode wurde von ChatGPT erstellt
        public Stundenplan ErstellePlan()
        {
            var plan = new Stundenplan();
            string[] tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };
            int maxVersuche = 300;

            for (int i = 0; i < schueler.Count; i++)
            {
                var sch = schueler[i];
                foreach (var fach in sch.Faecher)
                {
                    var lp = lehrpersonen.FirstOrDefault(l => l.Faecher.Contains(fach));
                    if (lp == null) continue;

                    bool platziert = false;
                    for (int versuch = 0; versuch < maxVersuche && !platziert; versuch++)
                    {
                        int tagIdx = random.Next(Stundenplan.TAGE);
                        int stundeIdx = random.Next(Stundenplan.STUNDEN);
                        var raum = raeume[random.Next(Math.Max(1, raeume.Count))];

                        if (!plan.IstFrei(tagIdx, stundeIdx)) continue;
                        if (!lp.IstVerfuegbar(tage[tagIdx])) continue;

                        bool lpInSlot = false;
                        for (int t = 0; t < Stundenplan.TAGE && !lpInSlot; t++)
                        {
                            var existing = plan.Matrix[tagIdx, stundeIdx];
                            if (existing != null && existing.Lehrperson == lp.Name)
                            {
                                lpInSlot = true;
                            }
                        }
                        if (lpInSlot) continue;

                        var neue = new Stunde
                        {
                            Fach = fach,
                            Lehrperson = lp.Name,
                            Raum = raum.Bezeichnung,
                            Klasse = sch.Klasse,
                            Tag = tage[tagIdx],
                            StundeNummer = stundeIdx + 1
                        };

                        plan.Eintragen(tagIdx, stundeIdx, neue);
                        platziert = true;
                    }
                }
            }

            return plan;
        }
    }
}
