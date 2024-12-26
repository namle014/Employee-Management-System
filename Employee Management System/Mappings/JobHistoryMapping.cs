using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class JobHistoryMapping : Profile
    {
        public JobHistoryMapping()
        {
            //Insert
            CreateMap<JobHistoryCreateVModel,JobHistory>();
            // Update
            CreateMap<JobHistoryUpdateVModel,JobHistory>();
            //Get All 
            CreateMap<JobHistory, JobHistoryUpdateVModel>();
            //Get By Id
            CreateMap<JobHistory, JobHistoryUpdateVModel>();
            //Export
            CreateMap<JobHistory,JobHistoryExportVModel>();
        }
        
    }
}
