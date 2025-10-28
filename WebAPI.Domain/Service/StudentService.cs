using WebAPI.Domain.IService;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Data.DTO;
using AutoMapper;

namespace WebAPI.Domain.Service
{
    public class StudentService : IStudentService
    {
        private readonly UniversityContext _Context;
        private readonly IMapper _Mapper;

        public StudentService(UniversityContext context, IMapper mapper)
        {
            _Context = context;
            _Mapper = mapper;
        }

        public List<Student> GetAllStudents() =>
            _Context.Students.ToList();

        public Student? GetStudentById(int SSN) => _Context.Students.Include(s => s.Department).FirstOrDefault(s => s.SSN == SSN);

        public int AddStudent(Student student)
        {
            _Context.Students.Add(student);
            if( _Context.SaveChanges() > 0)
            {
                return student.SSN;
            }
            else
                return -1;
        }

        public bool UpdateStudent(Student student)
        {

            _Context.Students.Update(student);
            if (_Context.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }

        public bool DeleteStudent(Student student)
        {
            _Context.Students.Remove(student);
            return _Context.SaveChanges() > 0;
        }

        ////////////////////////////////////////// Student DTO Section //////////////////////////////////////////
        

        public StudentWithDeptInfoDTO GetStudentWithDeptInfoById(int SSN)
        {
            var student = _Context.Students.Include(s => s.Department).FirstOrDefault(s => s.SSN == SSN);

            //StudentWithDeptInfoDTO studentDept = new StudentWithDeptInfoDTO();

            //studentDept.SSN = student.SSN;
            //studentDept.Name = student.Name;
            //studentDept.Age = student.Age;
            //studentDept.Address = student.Address;
            //studentDept.ImageURL = student.ImageURL;
            //studentDept.Email = student.Email;
            //studentDept.DepartmentName = student.Department.Name;
            //studentDept.DeptManagerName = student.Department.ManagerName;
            //studentDept.Skills = new List<string>() { "Football", "Programming", "Singing" };
            //studentDept.Message = (student.Age > 18 && student.Age < 23) ? "You Are PreGraduate" : "You Are PostGraduate";

            var studentDept = _Mapper.Map<StudentWithDeptInfoDTO>(student);

            return studentDept;
        }
    }
}
