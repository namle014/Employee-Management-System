using AutoMapper;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class SysFileMapping : Profile
    {
        public SysFileMapping() 
        {
            //Insert
            CreateMap<SysFileCreateVModel, SysFile>();
            // Update
            CreateMap<SysFileUpdateVModel, SysFile>();
            //Get All 
            CreateMap<SysFile, SysFileGetAllVModel>();
            //Get By Id
            CreateMap<SysFile, SysFileGetByIdVModel>();
            //Get list
            CreateMap<SysFile, SysFileExportVModel>();
        }
    }
    }

