using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
namespace OA.WebApi.Mappings
{
    public class AspNetUserMapping : Profile
    {
        public AspNetUserMapping()
        {
            //Insert
            CreateMap<UserCreateVModel, AspNetUser>();
            //Update
            CreateMap<UserUpdateVModel, AspNetUser>();
            //Get All
            CreateMap<AspNetUser, UserGetAllVModel>();
            //Get By Id
            CreateMap<AspNetUser, UserGetByIdVModel>();
            CreateMap<AspNetUser, GetMeVModel>();
        }
    }
}
