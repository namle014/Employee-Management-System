using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class ErrorReportMapping : Profile
    {
        public ErrorReportMapping()
        {
            //Insert
            CreateMap<ErrorReportCreateVModel,ErrorReport>();
            // Update
            CreateMap<ErrorReportUpdateVModel,ErrorReport>();
            //Get All 
            CreateMap<ErrorReport, ErrorReportUpdateVModel>();
            //Get By Id
            CreateMap<ErrorReport, ErrorReportUpdateVModel>();
            //Export
            CreateMap<ErrorReport,ErrorReportExportVModel>();
        }
        
    }
}
