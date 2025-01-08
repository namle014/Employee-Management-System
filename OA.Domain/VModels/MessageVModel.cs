using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{

    public class MessageCreateVModel
    {
        public string Content { get; set; } = string.Empty;
        public bool Type { get; set; }
    }
    public class MessageGetAllVModel : MessageCreateVModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }
    public class MessageExportVModel { }
    public class MessageUpdateVModel { }
    public class MessageGetByIdVModel { }
}
