using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            //Insert
            CreateMap<MessageCreateVModel, Message>();
            // Update
            CreateMap<MessageUpdateVModel, Message>();
            //Get All 
            CreateMap<Message, MessageGetAllVModel>();
            //Get By Id
            CreateMap<Message, MessageGetByIdVModel>();
            //Export
            CreateMap<Message, MessageExportVModel>();
        }
    }
}
