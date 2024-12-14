using OA.Core.Models;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface ISysFileService : IBaseService<SysFile, SysFileCreateVModel, SysFileUpdateVModel, SysFileGetByIdVModel, SysFileGetAllVModel>
    {
        Task<ResponseResult> CreateFile(SysFileCreateVModel model);
        Task CreateBase64(SysFileCreateBase64VModel model);
        Task<ResponseResult> FileChunks(FileChunk fileChunk);
        Task UploadAvatar(FileChunk fileChunk);
        Task<ResponseResult> GetAll(FilterSysFileVModel model);
        Task RemoveByUrl(string url);
    }
}
