using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.Validators;

namespace WebAPI.Data.DTO
{
    public class StudentWithDeptInfoDTO
    {
        public int SSN { get; set; }

        public string Name { get; set; } = null!;

        public int Age { get; set; }

        public string? Address { get; set; }

        public string? ImageURL { get; set; }

        public string Email { get; set; }

        public string DepartmentName { get; set; }

        public string DeptManagerName { get; set; }

        public List<string> Skills { get; set; }

        public string Message { get; set; }
    }
}
