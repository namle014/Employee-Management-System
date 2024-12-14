using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class DepartmentMapping : Profile
    {
        public DepartmentMapping() 
        {
            //Insert
            CreateMap<DepartmentCreateVModel, Department>();
            // Update
            CreateMap<DepartmentUpdateVModel, Department>();
            //Get All 
            CreateMap<Department, DepartmentGetAllVModel>();
            //Get By Id
            CreateMap<Department, DepartmentGetByIdVModel>();
            //Get list
            CreateMap<Department, DepartmentExportVModel>();
        }
    }
    }

