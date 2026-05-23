using ExerciseEFcore.Repository;

namespace ExerciseEFcore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StudentsRepo studentsRepo = new StudentsRepo(new Enities.SchoolDbContext());

            List<Enities.Student> students = studentsRepo.GetAll();

            foreach (Enities.Student student in students) {
                Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, Email: {student.Email}");
            }

            studentsRepo.Add(new Enities.Student { Name = "John Doe", Age = 20, Email = "ngocanh@gmail.com" });
            
            studentsRepo.Remove(1);

            studentsRepo.Update(new Enities.Student { Id = 2, Name = "Ngoc Anh", Age = 22, Email = "ngocanh@gmail.com" });

            Console.WriteLine();
             students = studentsRepo.GetAll();
        }
    }
}
