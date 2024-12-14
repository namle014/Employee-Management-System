using AutoMapper;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebAPI.Mappings
{
    public class DisciplineMapping : Profile
    {
        public DisciplineMapping() 
        {
            //Insert
            CreateMap<DisciplineCreateVModel, Discipline>();
            // Update
            CreateMap<DisciplineUpdateVModel, Discipline>();
            //Get All 
            CreateMap<Discipline, DisciplineGetAllVModel>();
            //Get By Id
            CreateMap<Discipline, DisciplineGetByIdVModel>();
            //Get list
            CreateMap<Discipline, DisciplineExportVModel>();
        }
    }
    }

