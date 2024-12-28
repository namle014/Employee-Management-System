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

            CreateMap<CreateBenefitUser, BenefitUser>();
            // Update
            CreateMap<BenefitUpdateVModel, Benefit>();
            //Get All 
            CreateMap<Benefit, BenefitGetAllVModel>();
            CreateMap<BenefitUser, GetAllBenefitUser>();

            //Get By Id
            CreateMap<Benefit, BenefitGetByIdVModel>();
            //Export
            CreateMap<Benefit, BenefitExportVModel>();
        }
    }
}
