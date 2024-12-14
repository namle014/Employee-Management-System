using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class WorkingRulesMapping : Profile
    {
        public WorkingRulesMapping() 
        {
            //Insert
            CreateMap<WorkingRulesCreateVModel, WorkingRules>();
            // Update
            CreateMap<WorkingRulesUpdateVModel, WorkingRules>();
            //Get All 
            CreateMap<WorkingRules, WorkingRulesGetAllVModel>();
            //Get By Id
            CreateMap<WorkingRules, WorkingRulesGetByIdVModel>();
            //Get list
            CreateMap<WorkingRules, WorkingRulesExportVModel>();
        }
    }
    }

