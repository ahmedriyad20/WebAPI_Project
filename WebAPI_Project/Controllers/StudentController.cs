using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Models;
using WebAPI.Domain.Service;

namespace WebAPI_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private StudentService _studentService;
        private DepartmentService _departmentService;

        public StudentController(StudentService studentService, DepartmentService departmentService)
        {
            _studentService = studentService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = _studentService.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("{Id:int}")]
        public IActionResult GetStudentById(int Id)
        {
            //var student = _studentService.GetStudentById(Id);

            var student = _studentService.GetStudentWithDeptInfoById(Id);

            if (student != null)
            {
                return Ok(student);
            }

            return NotFound();
        }


        // Example of applying CORS policy to a specific action
        //[DisableCors] // will override any global CORS Middleware or controller-level CORS policies and disable CORS for this action.
        [EnableCors("ProductionPolicy")]
        [HttpGet("Production/{Id}")]
        public IActionResult GetStudentByIdForProduction(int Id)
        {
            //var student = _studentService.GetStudentById(Id);

            var student = _studentService.GetStudentWithDeptInfoById(Id);

            if (student != null)
            {
                return Ok(student);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            if(ModelState.IsValid)
            {
                if (_studentService.AddStudent(student) != -1)
                {
                    return CreatedAtAction(nameof(GetStudentById), new { Id = student.SSN }, student);
                }
            }

            return BadRequest();
        }

        [HttpPut]
        public IActionResult UpdateStudent(Student student)
        {
            var existingStudent = _studentService.GetStudentById(student.SSN);
            if (existingStudent == null)
            {
                return NotFound();
            }
           
            if (_studentService.UpdateStudent(existingStudent))
            {
               return Ok($"Student with SSN {existingStudent.SSN} updated successfully.");
            }
            
            
            return BadRequest();
        }

        [HttpDelete]
        public IActionResult DeleteStudent(int Id)
        {
            var existingStudent = _studentService.GetStudentById(Id);
            if(existingStudent == null)
            {
                return NotFound();
            }

            if (_studentService.DeleteStudent(existingStudent))
            {
                return Ok($"Student with SSN {Id} deleted successfully.");
            }

            return BadRequest();
        }
    }
}
