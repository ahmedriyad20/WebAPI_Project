using WebAPI.Domain.IService;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.DTO;
using AutoMapper;

namespace WebAPI.Domain.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly UniversityContext _Context;
        private readonly IMapper _Mapper;

        public DepartmentService(UniversityContext context, IMapper mapper)
        {
            _Context = context;
            _Mapper = mapper;
        }

        public  List<Department> GetAllDepartments()
        {
            List<Department> Departments = _Context.Departments.ToList();

            return Departments;
        }

        public Department GetDepartmentById(int Id)
        {
            return _Context.Departments.FirstOrDefault(d => d.Id == Id);
        }

        public  Department GetDepartmentByName(string Name)
        {
            Department? Department = _Context.Departments.Include(d => d.Students).FirstOrDefault(d => d.Name == Name);

            return Department;
        }

        public  int AddDepartment(Department department)
        {
            _Context.Departments.Add(department);

            if( _Context.SaveChanges() > 0)
            {
                return department.Id;
            }
            else
                return -1;
        }

        public  bool UpdateDepartment(Department department)
        {
            _Context.Departments.Update(department);
            return _Context.SaveChanges() > 0;
        }

        public bool DeleteDepartment(Department department)
        {
            _Context.Departments.Update(department);
            return _Context.SaveChanges() > 0;
        }

        ////////////////////////////////////////// Department DTO Section //////////////////////////////////////////
        ///
        public DepartmentWithAllStudentsNamesDTO GetDepartmentWithAllStudentsNamesById(int Id)
        {
            var department = _Context.Departments.Include(d => d.Students).FirstOrDefault(d => d.Id == Id);

            //DepartmentWithAllStudentsNamesDTO departmentDTO = new DepartmentWithAllStudentsNamesDTO();

            //departmentDTO.Name = department.Name;
            //departmentDTO.ManagerName = department.ManagerName;
            //departmentDTO.Location = department.Location;
            //departmentDTO.OpenDate = department.OpenDate;
            //departmentDTO.StudentNames = department.Students.Select(s => s.Name).ToList();
            //departmentDTO.TotalStudents = department.Students.Count;

            //_Mapper.Map<Department, DepartmentWithAllStudentsNamesDTO>(department, departmentDTO);

            var departmentDTO = _Mapper.Map<DepartmentWithAllStudentsNamesDTO>(department);

            return departmentDTO;
        }
    }
}
