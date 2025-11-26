# Benutzerhandbuch - Timetable-Project

## Installation und Start

### Voraussetzungen
- .NET 9.0 SDK installiert

### Programm starten
```bash
cd Timetable-Project-main
dotnet run
```

Oder das kompilierte Programm ausführen:
```bash
./bin/Debug/net9.0/Timetable-Project
```

## Hauptmenü

Nach dem Start erscheint das Hauptmenü mit folgenden Optionen:

```
=== STUNDENPLAN MANAGER ===
1. Daten anzeigen
2. Fach hinzufügen
3. Lehrperson hinzufügen
4. Schüler hinzufügen
5. Raum hinzufügen
6. Stundenplan generieren
7. Stundenplan anzeigen
0. Beenden
```

## Schritt-für-Schritt Anleitung

### 1. Grunddaten erfassen

#### Fächer hinzufügen (Option 2)
```
Fachname: Mathematik
Wochenstunden: 4
```
Wiederholen Sie dies für alle Fächer (z.B. Deutsch, Englisch, Sport, etc.)

#### Räume hinzufügen (Option 5)
```
Raum: A101
Kapazität: 30
```
Fügen Sie mehrere Räume hinzu.

#### Lehrpersonen hinzufügen (Option 3)
```
Name: Herr Müller
Fächer (komma-getrennt): Mathematik, Physik
```
**Wichtig:** Die Fächer müssen bereits existieren. Trennen Sie mehrere Fächer mit Komma.

#### Schüler hinzufügen (Option 4)
```
Name: Max Mustermann
Alter: 16
Klasse: 10A
Fächer (komma-getrennt): Mathematik, Deutsch, Englisch
```
Die angegebenen Fächer müssen bereits im System existieren.

### 2. Daten überprüfen (Option 1)

Zeigt eine Übersicht aller erfassten:
- Fächer mit Wochenstunden
- Lehrpersonen mit ihren Fächern
- Schüler mit Klassen und Fächern
- Räume mit Kapazität

### 3. Stundenplan generieren (Option 6)

Generiert automatisch einen Stundenplan basierend auf:
- Schülern und ihren Fächern
- Verfügbaren Lehrpersonen
- Vorhandenen Räumen
- Verfügbarkeiten der Lehrpersonen

**Voraussetzung:** Es müssen mindestens Schüler, Lehrpersonen und Räume erfasst sein.

### 4. Stundenplan anzeigen (Option 7)

Zeigt den generierten Stundenplan an. Sie haben zwei Anzeigeoptionen:

**Option 1 - Tabellarische Übersicht:**
```
Stunde | Zeit       | Montag    | Dienstag  | ...
1      | 08:00-08:45| Mathe (10A)| Deutsch (10A)| ...
```

**Option 2 - Detail-Ansicht:**
```
┌────────────────── Montag ──────────────────
│ 1. Stunde (08:00-08:45):
│   Mathematik mit Herr Müller
│   Raum: A101, Klasse: 10A
```

Die Bewertung des Plans wird automatisch angezeigt:
```
Bewertung: 85.40
```

### 5. Programm beenden (Option 0)

Alle Daten werden automatisch in `daten.json` gespeichert und beim nächsten Start wieder geladen.

## Datenspeicherung

- Alle Daten werden in der Datei `daten.json` gespeichert
- Beim Start werden die Daten automatisch geladen
- Bei jedem Hinzufügen/Ändern werden die Daten gespeichert
- Die JSON-Datei kann auch manuell bearbeitet werden

## Bewertungskriterien

Der Stundenplan wird nach folgenden Kriterien bewertet:

1. **Randstunden** - Stunden am Tagesanfang (8:00) oder -ende (15:00) werden schlechter bewertet
2. **Zwischenstunden** - Lücken im Stundenplan werden negativ bewertet
3. **Ressourcennutzung** - Weniger verschiedene Räume sind besser

Höhere Bewertung = besserer Stundenplan (Maximum: 100 Punkte)

## Tipps

- Fügen Sie zuerst alle Fächer hinzu, bevor Sie Lehrpersonen und Schüler erfassen
- Stellen Sie sicher, dass für jedes Fach mindestens eine Lehrperson zuständig ist
- Fügen Sie genügend Räume hinzu, um Überschneidungen zu vermeiden
- Bei unbefriedigenden Ergebnissen: Stundenplan erneut generieren (Option 6)
- Der Algorithmus ist randomisiert - mehrmaliges Generieren kann bessere Ergebnisse liefern

## Fehlerbehebung

**"Nicht genug Daten" beim Generieren:**
- Überprüfen Sie, ob Schüler, Lehrpersonen und Räume erfasst sind

**Fach wird nicht zugeordnet:**
- Stellen Sie sicher, dass eine Lehrperson das Fach unterrichten kann
- Prüfen Sie die Verfügbarkeit der Lehrperson

**Daten gehen verloren:**
- Die Datei `daten.json` sollte nicht gelöscht werden
- Bei Problemen: Sicherungskopie der JSON-Datei erstellen

## Beispiel-Workflow

```
1. Programm starten: dotnet run
2. Fächer hinzufügen: Mathematik, Deutsch, Englisch
3. Räume hinzufügen: A101, A102, B201
4. Lehrperson hinzufügen: Herr Müller (Mathematik)
5. Lehrperson hinzufügen: Frau Schmidt (Deutsch, Englisch)
6. Schüler hinzufügen: Max (Klasse 10A, Fächer: Mathematik, Deutsch)
7. Daten anzeigen (Option 1) zur Kontrolle
8. Stundenplan generieren (Option 6)
9. Stundenplan anzeigen (Option 7)
10. Programm beenden (Option 0)
```
