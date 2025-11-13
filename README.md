# Timetbale_Project

## Description

This poject is a C# application developed as a school project.  
Its goal is to automatically generate weekly timetables by combining students, teachers, rooms, and subjects without conflicts.  
The program takes into account teacher and room availability, subject hours per week, and class assignments.

---

## Main Features

- Automatic Timetable generation
- JSON Data Storage
- Smart plan evaluation

---

## Class Overview

| Class           | Description                                                             |
| --------------- | ----------------------------------------------------------------------- |
| **Schueler_in** | Represents a student with name, age, class, and assigned subjects       |
| **Lehrperson**  | Represents a teacher with subjects and weekly availability              |
| **Raum**        | Represents a classroom with name, capacity, and availability status     |
| **Fach**        | Represents a subject with weekly hour requirements                      |
| **Stunde**      | Represents a single lesson with subject, teacher, room, time, and class |
| **Stundenplan** | Holds a collection of all scheduled lessons                             |
| **Planer**      | Core logic that combines all data and creates the timetable             |

---

## Example Output

=== Generated Timetable ===

--- Monday ---

1. Lesson: Mathematics (10A) with Herr Meier in R101
2. Lesson: Deutsch (10A) with Frau Schulz in R202

--- Tuesday ---
(no entries)
...

Score: 92.70 (Edge: 3.0, Gaps: 2.0, Rooms: 2.3)

## How To Run

Run the project from the console:

dotnet run

---

## License

This project is for educational use.
