using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Models;
using WebAPI.Domain.Service;
using WebAPI_Project.Filters;

namespace WebAPI_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private DepartmentService _DepartmentService;


        public DepartmentController(DepartmentService DepartmentService, DepartmentService departmentService)
        {
            _DepartmentService = DepartmentService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            var Departments = _DepartmentService.GetAllDepartments();
            return Ok(Departments);
        }

        [HttpGet("{Id:int}")]
        public IActionResult GetDepartmentById(int Id)
        {
            //var Department = _DepartmentService.GetDepartmentById(Id);

            var Department = _DepartmentService.GetDepartmentWithAllStudentsNamesById(Id);

            if (Department != null)
            {
                return Ok(Department);
            }

            return NotFound();
        }

        [ExceptionHandleFilter]
        [HttpGet("{Name:alpha}")]
        public IActionResult GetDepartmentByName(string Name)
        {
            //throw new Exception("Test exception"); // This will trigger the filter


            var Department = _DepartmentService.GetDepartmentByName(Name);
            if (Department != null)
            {
                return Ok(Department);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult AddDepartment(Department Department)
        {

            if (_DepartmentService.AddDepartment(Department) != -1)
            {
                return CreatedAtAction(nameof(GetDepartmentById), new { Id = Department.Id }, Department);
            }
            

            return BadRequest();
        }

        [HttpPut]
        public IActionResult UpdateDepartment(Department Department)
        {
            if (ModelState.IsValid)
            {
                var existingDepartment = _DepartmentService.GetDepartmentById(Department.Id);
                if (existingDepartment == null)
                {
                    return NotFound();
                }

                if (_DepartmentService.UpdateDepartment(Department))
                {
                    return Ok($"Department with SSN {Department.Id} updated successfully.");
                }
            }

            return BadRequest();
        }

        [HttpDelete]
        public IActionResult DeleteDepartment(int Id)
        {
            var existingDepartment = _DepartmentService.GetDepartmentById(Id);
            if (existingDepartment == null)
            {
                return NotFound();
            }

            if (_DepartmentService.DeleteDepartment(existingDepartment))
            {
                return Ok($"Department with SSN {Id} deleted successfully.");
            }

            return BadRequest();
        }
    }
}
