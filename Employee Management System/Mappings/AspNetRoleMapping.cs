using AutoMapper;
using OA.Domain.VModels;
using OA.Domain.VModels.Role;
using OA.Infrastructure.EF.Entities;
namespace OA.WebApi.Mappings
{
    public class AspNetRoleMapping : Profile
    {
        public AspNetRoleMapping()
        {
            //Insert
            CreateMap<AspNetRoleCreateVModel, AspNetRole>();
            //Update
            CreateMap<AspNetRoleUpdateVModel, AspNetRole>();
            //Get All
            CreateMap<AspNetRole, AspNetRoleGetAllVModel>();
            //Get By Id
            CreateMap<AspNetRole, AspNetRoleGetByIdVModel>();
            CreateMap<AspNetRole, GetMeVModel>();
        }
    }
}
