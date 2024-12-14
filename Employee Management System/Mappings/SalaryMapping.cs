using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class SalaryMapping : Profile
    {
        public SalaryMapping()
        {
            //Insert
            CreateMap<SalaryCreateVModel, Salary>();
            // Update
            CreateMap<SalaryUpdateVModel, Salary>();
            //Get All 
            CreateMap<Salary, SalaryGetAllVModel>();
            //Get By Id
            CreateMap<Salary, SalaryGetByIdVModel>();
            //Export
            CreateMap<Salary, SalaryExportVModel>();
        }
    }
}
