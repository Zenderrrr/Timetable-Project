# Timetable-Project Tests

This directory contains comprehensive unit tests for the Timetable-Project application.

## Test Structure

- **EntityTests.cs** - Tests for all entity classes (Fach, Lehrperson, Raum, Schueler_in, Stunde)
- **StundenplanTests.cs** - Tests for the timetable matrix and scoring algorithm
- **PlanerTests.cs** - Tests for the timetable generation algorithm
- **TestRunner.cs** - Main test runner program

## Running Tests

The tests have been rewritten to use plain C# without xUnit dependencies.

### Build the tests
```bash
dotnet build
```

### Run the compiled tests
```bash
dotnet bin/Debug/net9.0/Timetable-Project.Tests.dll
```

## Test Results

All 44 tests across 3 test suites are passing:
- EntityTests: 14 tests
- StundenplanTests: 17 tests
- PlanerTests: 13 tests
