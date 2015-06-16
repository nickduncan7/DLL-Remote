using System.Data.Linq.Mapping;

namespace DLLRemoteService.Data_Model
{
    [Table(Name = "RegisteredDLLs")]
    public class RegisteredDLL
    {
        [Column]
        public string DLLPath { get; set; }
    }
}