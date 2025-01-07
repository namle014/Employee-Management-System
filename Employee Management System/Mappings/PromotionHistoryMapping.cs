using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class PromotionHistoryMapping : Profile
    {
        public PromotionHistoryMapping()
        {
            //Insert
            CreateMap<CreatePromotionHistory, PromotionHistory>();

            //CreateMap<CreateBenefitUser, BenefitUser>();
            // Update
            CreateMap<UpdatePromotionHistory, PromotionHistory>();
            //Get All 
            CreateMap<PromotionHistory, GetAllPromotionHistory>();
            //CreateMap<BenefitUser, GetAllBenefitUser>();

            //Get By Id
            CreateMap<PromotionHistory, GetByIdPromotionHistory>();
            //Export
            //CreateMap<Benefit, BenefitExportVModel>();
        }
    }
}
