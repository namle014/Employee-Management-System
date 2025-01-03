using OA.Core.Models;
using OA.Core.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface ITransferHistoryService
    {
        Task Create(TransferHistoryCreateVModel model);
        Task<ResponseResult> GetAll();
    }
}
