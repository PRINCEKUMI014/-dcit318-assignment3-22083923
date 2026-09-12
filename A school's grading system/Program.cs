using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Student class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // Processor class
    public static class StudentResultProcessor
    {
        public static List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue; // ignore empty lines

                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        throw new MissingFieldException($"Line {lineNumber}: expected 3 fields but found {parts.Length}.");

                    var idPart = parts[0].Trim();
                    var namePart = parts[1].Trim();
                    var scorePart = parts[2].Trim();

                    if (string.IsNullOrEmpty(idPart) || string.IsNullOrEmpty(namePart) || string.IsNullOrEmpty(scorePart))
                        throw new MissingFieldException($"Line {lineNumber}: one or more fields are empty.");

                    if (!int.TryParse(idPart, out var id))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: ID '{idPart}' is not a valid integer.");

                    if (!int.TryParse(scorePart, out var score))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{scorePart}' is not a valid integer.");

                    students.Add(new Student { Id = id, FullName = namePart, Score = score });
                }
            }

            return students;
        }

        public static void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var s in students)
                {
                    var line = $"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}";
                    writer.WriteLine(line);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GitHub Copilot");

            string inputPath;
            string outputPath;

            if (args.Length >= 2)
            {
                inputPath = args[0];
                outputPath = args[1];
            }
            else
            {
                Console.Write("Enter input file path: ");
                inputPath = Console.ReadLine() ?? string.Empty;
                Console.Write("Enter output file path: ");
                outputPath = Console.ReadLine() ?? string.Empty;
            }

            try
            {
                var students = StudentResultProcessor.ReadStudentsFromFile(inputPath);
                StudentResultProcessor.WriteReportToFile(students, outputPath);
                Console.WriteLine($"Report written to: {outputPath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score format: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
