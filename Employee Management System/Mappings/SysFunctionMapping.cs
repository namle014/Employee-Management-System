using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class SysFunctionMapping : Profile
    {
        public SysFunctionMapping()
        {
            //Insert
            CreateMap<SysFunctionCreateVModel, SysFunction>();
            // Update
            CreateMap<SysFunctionUpdateVModel, SysFunction>();
            //Get All 
            CreateMap<SysFunction, SysFunctionGetAllVModel>();
            CreateMap<SysFunction, SysFunctionGetAllVModel>()
                .ForPath(des => des.Function.IsAllowAll, src => src.MapFrom(x => false))
                .ForPath(des => des.Function.IsAllowView, src => src.MapFrom(x => false))
                .ForPath(des => des.Function.IsAllowCreate, src => src.MapFrom(x => false))
                .ForPath(des => des.Function.IsAllowEdit, src => src.MapFrom(x => false))
                .ForPath(des => des.Function.IsAllowPrint, src => src.MapFrom(x => false))
                .ForPath(des => des.Function.IsAllowDelete, src => src.MapFrom(x => false));
            CreateMap<SysFunctionGetAllVModel, SysFunctionGetAllVModel>()
                .ForPath(des => des.Function.IsAllowAll, src => src.MapFrom(x => x.Function.IsAllowAll))
                .ForPath(des => des.Function.IsAllowView, src => src.MapFrom(x => x.Function.IsAllowView))
                .ForPath(des => des.Function.IsAllowCreate, src => src.MapFrom(x => x.Function.IsAllowCreate))
                .ForPath(des => des.Function.IsAllowEdit, src => src.MapFrom(x => x.Function.IsAllowEdit))
                .ForPath(des => des.Function.IsAllowPrint, src => src.MapFrom(x => x.Function.IsAllowPrint))
                .ForPath(des => des.Function.IsAllowDelete, src => src.MapFrom(x => x.Function.IsAllowDelete));
            CreateMap<SysFunction, SysFunctionGetAllAsTreesVModel>();
            //Get By Id
            CreateMap<SysFunction, SysFunctionGetByIdVModel>();
        }
    }
}