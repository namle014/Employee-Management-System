using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class BenefitMapping : Profile
    {
        public BenefitMapping()
        {
            //Insert
            CreateMap<BenefitCreateVModel, Benefit>();
            // Update
            CreateMap<BenefitUpdateVModel, Benefit>();
            //Get All 
            CreateMap<Benefit, BenefitGetAllVModel>();
            //Get By Id
            CreateMap<Benefit, BenefitGetByIdVModel>();
            //Export
            CreateMap<Benefit, BenefitExportVModel>();
        }
    }
}
