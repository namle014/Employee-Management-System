using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class RewardMapping : Profile
    {
        public RewardMapping() 
        {
            //Insert
            CreateMap<RewardCreateVModel, Reward>();
            // Update
            CreateMap<RewardUpdateVModel, Reward>();
            //Get All 
            CreateMap<Reward, RewardGetAllVModel>();
            //Get By Id
            CreateMap<Reward, RewardGetByIdVModel>();
            //Get list
            CreateMap<Reward, RewardExportVModel>();
        }
    }
    }

