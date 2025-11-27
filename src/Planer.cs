using System;
using System.Collections.Generic;
using System.Linq;

namespace Timetable_Project
{
    /// <summary>
    /// Generiert und verwaltet Stundenpläne basierend auf verfügbaren Ressourcen
    /// </summary>
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

        /// <summary>
        /// Erstellt einen vollständigen Stundenplan basierend auf den vorhandenen Daten
        /// </summary>
        public Stundenplan ErstellePlan()
        {
            var plan = new Stundenplan();
            
            var klassenGruppen = schueler.GroupBy(s => s.Klasse);
            
            foreach (var klasse in klassenGruppen)
            {
                var klassenName = klasse.Key;
                var schuelerInKlasse = klasse.ToList();
                
                var klassenFaecher = ErmittleKlassenFaecher(schuelerInKlasse);
                
                foreach (var fach in klassenFaecher)
                {
                    PlatziereFachFuerKlasse(plan, klassenName, fach.Key, fach.Value);
                }
            }

            return plan;
        }

        private Dictionary<string, int> ErmittleKlassenFaecher(List<Schueler_in> schuelerInKlasse)
        {
            var faecher = new Dictionary<string, int>();
            
            foreach (var schueler in schuelerInKlasse)
            {
                foreach (var fach in schueler.Faecher)
                {
                    if (!faecher.ContainsKey(fach))
                    {
                        faecher[fach] = 2;
                    }
                }
            }
            return faecher;
        }

        private void PlatziereFachFuerKlasse(Stundenplan plan, string klasse, string fach, int wochenstunden)
        {
            var lehrperson = lehrpersonen.FirstOrDefault(l => l.Faecher.Contains(fach));
            if (lehrperson == null) return;

            var verfuegbareRaeume = raeume.Where(r => r.Verfuegbar).ToList();
            if (!verfuegbareRaeume.Any()) return;

            int platziert = 0;
            int maxVersuche = 1000;

            while (platziert < wochenstunden && maxVersuche-- > 0)
            {
                int tagIdx = random.Next(Stundenplan.TAGE);
                int stundeIdx = random.Next(Stundenplan.STUNDEN);
                var raum = verfuegbareRaeume[random.Next(verfuegbareRaeume.Count)];

                if (!plan.IstFrei(tagIdx, stundeIdx)) continue;
                
                if (!lehrperson.IstVerfuegbar(Stundenplan.TAGE_NAMEN[tagIdx])) continue;
                
                bool lehrpersonBelegt = false;
                for (int t = 0; t < Stundenplan.TAGE && !lehrpersonBelegt; t++)
                {
                    var existing = plan.Matrix[tagIdx, stundeIdx];
                    if (existing != null && existing.Lehrperson == lehrperson.Name)
                    {
                        lehrpersonBelegt = true;
                    }
                }
                if (lehrpersonBelegt) continue;

                bool raumBelegt = false;
                for (int t = 0; t < Stundenplan.TAGE && !raumBelegt; t++)
                {
                    var existing = plan.Matrix[tagIdx, stundeIdx];
                    if (existing != null && existing.Raum == raum.Bezeichnung)
                    {
                        raumBelegt = true;
                    }
                }
                if (raumBelegt) continue;

                var neueStunde = new Stunde 
                { 
                    Fach = fach, 
                    Lehrperson = lehrperson.Name, 
                    Raum = raum.Bezeichnung, 
                    Klasse = klasse,
                    Tag = Stundenplan.TAGE_NAMEN[tagIdx],
                    StundeNummer = stundeIdx + 1
                };
                
                plan.Eintragen(tagIdx, stundeIdx, neueStunde);
                platziert++;
            }
        }
    }
}