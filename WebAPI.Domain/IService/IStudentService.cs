using WebAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain.IService
{
    public interface IStudentService
    {
        public List<Student> GetAllStudents();

        public Student? GetStudentById(int SSN);

        public int AddStudent(Student student);

        public bool UpdateStudent(Student student);

        public bool DeleteStudent(Student student);

    }
}
