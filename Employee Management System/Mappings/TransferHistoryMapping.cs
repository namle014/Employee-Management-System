using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class TransferHistoryMapping : Profile
    {
        public TransferHistoryMapping()
        {
            //Insert
            CreateMap<TransferHistoryCreateVModel, TransferHistory>();
            // Update
            CreateMap<TransferHistoryUpdateVModel, TransferHistory>();
            //Get All 
            CreateMap<TransferHistory, TransferHistoryGetAllVModel>();
            //Get By Id
            CreateMap<TransferHistory, TransferHistoryGetByIdVModel>();
            //Export
            CreateMap<TransferHistory, TransferHistoryExportVModel>();
        }
    }
}
