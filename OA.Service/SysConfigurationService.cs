using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
namespace OA.Service
{
    public class SysConfigurationService : BaseService<SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel, SysConfigurationExportVModel>, ISysConfigurationService
    {
        private readonly IBaseRepository<SysConfiguration> _configRepo;
        private readonly IMapper _mapper;
        public SysConfigurationService(IBaseRepository<SysConfiguration> configRepo, IMapper mapper) : base(configRepo, mapper)
        {
            _configRepo = configRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetByConfigTypeKey(string type, string key)
        {
            var result = new ResponseResult();
            try
            {
                var entity = (await _configRepo.Where(x => x.Type == type && x.Key == key)).FirstOrDefault();
                //if (entity != null && entity.IsActive.HasValue)
                if (entity != null)
                {
                    result.Data = _mapper.Map<SysConfiguration, SysConfigurationGetByIdVModel>(entity);
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                };
            }
            catch (Exception ex)
            {
                var message = Utilities.MakeExceptionMessage(ex);
                result.Success = false;
            }
            return result;
        }
    }
}
