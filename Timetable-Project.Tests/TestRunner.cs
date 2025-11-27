using System;

namespace Timetable_Project.Tests
{
    /// <summary>
    /// Main entry point for running all tests
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         Timetable Project - Test Suite Runner            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                // Run EntityTests
                EntityTests.RunAllTests();
                
                // Run StundenplanTests
                StundenplanTests.RunAllTests();
                
                // Run PlanerTests
                PlanerTests.RunAllTests();
                
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("OVERALL TEST SUMMARY");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine($"Total Test Suites: 3");
                Console.WriteLine($"All tests completed successfully!");
                Console.WriteLine(new string('=', 60));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ FATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}
