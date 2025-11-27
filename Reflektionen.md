# Reflektionen zum Timetable-Projekt

## Teamarbeit und Aufgabenverteilung

Generell hatte die Gruppe einen eher losen Zusammenhalt, wobei Egor den grössten Teil des Codes alleine geschrieben hat und Zoe und Gian eher Dokumentation und Tests übernahmen. Die Arbeitsteilung hat sich organisch entwickelt, wobei jedes Teammitglied seine Stärken einbringen konnte.

## Projektverlauf

Wir waren gut in der Zeit und das Projekt läuft gut. Die Implementierung der Kernfunktionalität verlief reibungslos, und die nachträgliche Erstellung der Tests half dabei, die Codequalität zu sichern und potenzielle Fehler zu identifizieren.

## Erfüllung der Anforderungen

### 1. Grundfunktionen (vollständig erfüllt ✓)
- **Verwaltung von Entitäten**: Vollständig implementiert für Schüler:innen, Lehrpersonen, Räume und Fächer mit CRUD-Operationen über die Konsolen-Applikation
- **Definition von Verfügbarkeiten**: Implementiert durch `Verfuegbarkeit`-Dictionary in der `Lehrperson`-Klasse, ermöglicht tagbasierte Verfügbarkeitsdefinition
- **Zuweisung**: Schüler können Fächer besuchen (List<string> Faecher), Lehrpersonen können bestimmte Fächer unterrichten (List<string> Faecher)
- **Automatische Stundenplanerstellung**: Implementiert durch die `Planer`-Klasse mit randomisiertem Algorithmus, der alle Constraints beachtet
- **Persistente Datenspeicherung**: Vollständig umgesetzt mit JSON-Serialisierung (daten.json), Daten bleiben nach Neustart erhalten

### 2. Qualitätskriterien bei der Planung (vollständig erfüllt ✓)
- **Fächer an Randzeiten**: Bewertet in `BewertePlan()` - erste (Stunde 0) und letzte Stunde (Stunde 7) werden mit `GewichtRandstunden` bestraft
- **Zwischenstunden**: Implementiert - Lücken im Tagesplan nach Beginn des Unterrichts werden mit `GewichtZwischenstunden` negativ bewertet
- **Ressourcenschonender Stundenplan**: Weniger gleichzeitig genutzte Räume werden besser bewertet durch `GewichtRessourcen * Anzahl_verschiedener_Räume`
- **Einstellbare Gewichtung**: Alle drei Gewichte (`GewichtRandstunden`, `GewichtZwischenstunden`, `GewichtRessourcen`) sind als öffentliche Properties in der `Stundenplan`-Klasse definiert und können angepasst werden

### Technische Umsetzung
Die Applikation nutzt ein sauberes Objektmodell mit Klassen für alle Entitäten. Der Scoring-Algorithmus ermöglicht objektive Bewertung der Stundenpläne. Die Datenpersistenz erfolgt über JSON-Serialisierung, was eine einfache und lesbare Speicherung ermöglicht.

## Lerneffekte

Durch das Projekt haben alle Teammitglieder wertvolle Erfahrungen gesammelt

## Verbesserungspotenzial

- Engere Zusammenarbeit und mehr Pair Programming hätten das Verständnis des gesamten Codes verbessert
- Frühere Integration von Tests während der Entwicklung wäre effizienter gewesen
- Regelmässigere Code-Reviews hätten zur Wissensverteilung beigetragen
- UI für die Gewichtungseinstellungen könnte benutzerfreundlicher sein (aktuell nur über Code änderbar)
- Der Planungsalgorithmus könnte deterministischer sein und bessere Ergebnisse liefern

## Fazit

Trotz der losen Zusammenarbeit konnte das Projekt erfolgreich abgeschlossen werden.
Jedes Teammitglied hat in seinem Bereich dazugelernt und zum Gesamterfolg beigetragen.
