using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_1.Models
{
    public class Contract
    {
        public int ContractId { get; set; }
        public string ContractCode { get; set; }
        public string ContractName { get; set; }
        public string Customer { get; set; }

        // Навигационное свойство для связи "один ко многим" с этапами (Stages)
        public ICollection<Stages> Stages { get; set; }
    }
}
