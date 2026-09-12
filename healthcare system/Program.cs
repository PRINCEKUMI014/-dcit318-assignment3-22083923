using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // Generic repository for entity management
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }

        public List<T> GetAll()
        {
            // Return a copy to avoid external modification
            return new List<T>(items);
        }

        public T? GetById(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var item = items.FirstOrDefault(predicate);
            if (item == null) return false;
            return items.Remove(item);
        }
    }

    // Patient entity
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Age = age;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    // Prescription entity
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription Id: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
        }
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Alice Johnson", 34, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Smith", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Carol Williams", 28, "Female"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Lisinopril", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Atorvastatin", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-20)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Albuterol", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Omeprazole", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("All patients:");
            foreach (var p in _patientRepo.GetAll())
            {
                Console.WriteLine(p);
            }
            Console.WriteLine();
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var list))
            {
                // return a copy to protect internal state
                return new List<Prescription>(list);
            }
            return new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine($"Patient with Id {patientId} not found.");
                return;
            }

            Console.WriteLine($"Prescriptions for {patient.Name} (Id {patient.Id}):");
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("  (none)");
                return;
            }

            foreach (var rx in prescriptions)
            {
                Console.WriteLine("  " + rx);
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            // Choose a patient id to show prescriptions for (e.g., 2)
            int selectedPatientId = 2;
            app.PrintPrescriptionsForPatient(selectedPatientId);

            // Keep console open when running from IDE
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
