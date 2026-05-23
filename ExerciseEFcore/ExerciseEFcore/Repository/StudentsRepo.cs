using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExerciseEFcore.Enities;

namespace ExerciseEFcore.Repository
{
    internal class StudentsRepo
    {
        SchoolDbContext context;
        public StudentsRepo(SchoolDbContext context) { 
            this.context = context;
        }

        public void Add(Student student) {
            context.Students.Add(student);
            context.SaveChanges();
        }


        public void Remove(int id)
        {
            var existingStudent = context.Students.FirstOrDefault(s => s.Id == id);
            if (existingStudent != null)
            {
                context.Students.Remove(existingStudent);
                context.SaveChanges();
            }
        }

        public List<Student> GetAll() {
            return context.Students.ToList();
        }

         public Student GetById(int id) {
            return context.Students.FirstOrDefault(s => s.Id == id);
         }
         
         public Student GetByName(string name) { return context.Students.FirstOrDefault(s => s.Name == name); }
         
         public void Update(Student student) {
                var existingStudent = context.Students.FirstOrDefault(s => s.Id == student.Id);
                if (existingStudent != null) {
                    existingStudent.Name = student.Name;
                    existingStudent.Age = student.Age;
                    existingStudent.Email = student.Email;
                    context.SaveChanges();
                }
        }





    }
}
