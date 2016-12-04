using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConfigurationSample.Models
{
    public class Detail
    {
        public int Id { get; set; }
        public string Comment { get; set; }

        public Master Master { get; set; }
        public int MasterId { get; set; }
    }
}
