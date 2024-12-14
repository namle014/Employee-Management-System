using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class NotificationsMapping : Profile
    {
        public NotificationsMapping()
        {
            //Insert
            CreateMap<NotificationsCreateVModel, Notifications>();
            // Update
            CreateMap<NotificationsUpdateVModel, Notifications>();
            //Get All 
            CreateMap<Notifications, NotificationsGetAllVModel>();

            CreateMap<Notifications, NotificationsGetByIdVModel>();
        }
    }
}
