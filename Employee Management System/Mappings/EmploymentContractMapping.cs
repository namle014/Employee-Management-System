using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class EmploymentContractMapping : Profile
    {
        public EmploymentContractMapping()
        {
            //Insert
            CreateMap<EmploymentContractCreateVModel, EmploymentContract>();
            // Update
            CreateMap<EmploymentContractUpdateVModel, EmploymentContract>();
            //Get All 
            CreateMap<EmploymentContract, EmploymentContractGetAllVModel>();
            //Get By Id
            CreateMap<EmploymentContract, EmploymentContractGetByIdVModel>();
            //Export
            CreateMap<EmploymentContract, EmploymentContractExportVModel>();
        }
        
    }
}
