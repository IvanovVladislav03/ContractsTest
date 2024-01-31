using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test_1.Models
{
    public class Stages
    {
        [Key]
        public int ContractStageId { get; set; }
        public string StageName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Внешний ключ
        public int ContractId { get; set; }

        // Навигационное свойство
        public Contract Contract { get; set; }
    }
}
