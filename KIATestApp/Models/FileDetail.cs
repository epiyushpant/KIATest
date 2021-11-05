using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KIATestApp.Models
{
    public class FileDetail
    {

        public int EmpId { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }
    }
}
