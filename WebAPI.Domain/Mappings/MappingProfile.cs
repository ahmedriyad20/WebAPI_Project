using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.DTO;
using WebAPI.Data.Models;

namespace WebAPI.Domain.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentWithAllStudentsNamesDTO>()
                .ForMember(dest => dest.StudentNames, src => src.MapFrom(src => src.Students.Select(s => s.Name).ToList()))
                .ForMember(dest => dest.TotalStudents, src => src.MapFrom(src => src.Students.Count()));


            CreateMap<Student, StudentWithDeptInfoDTO>()
                .ForMember(dest => dest.DepartmentName, src => src.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.DeptManagerName, src => src.MapFrom(src => src.Department.ManagerName))
                .ForMember(dest => dest.Message, src => src.MapFrom
                (src => (src.Age > 18 && src.Age < 22)? "You Are PreGraduate": "You Are PostGraduate"))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                    new List<string> { "Football", "Programming", "Singing" }));
        }
    }
}
