using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class WorkShiftMapping : Profile
    {
        public WorkShiftMapping()
        {
            //Insert
            CreateMap<WorkShiftCreateVModel, WorkShifts>();
            // Update
            CreateMap<WorkShiftUpdateVModel, WorkShifts>();
            //Get All 
            CreateMap<WorkShifts, WorkShiftGetAllVModel>();
            //Get By Id
            CreateMap<WorkShifts, WorkShiftGetByIdVModel>();
            //Get list
            CreateMap<WorkShifts, WorkShiftExportVModel>();
        }
    }
}
