using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Data.DTO
{
    public class DepartmentWithAllStudentsNamesDTO
    {
        public string Name { get; set; }

        public string ManagerName { get; set; }

        public string Location { get; set; }

        public DateTime OpenDate { get; set; }

        public List<string> StudentNames { get; set; }

        public int TotalStudents { get; set; }
    }
}
