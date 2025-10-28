using WebAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain.IService
{
    public interface IDepartmentService
    {
        public List<Department> GetAllDepartments();

        public Department GetDepartmentById(int Id);

        public Department GetDepartmentByName(string Name);

        public int AddDepartment(Department department);

        public bool UpdateDepartment(Department department);

        public bool DeleteDepartment(Department department);
    }
}
