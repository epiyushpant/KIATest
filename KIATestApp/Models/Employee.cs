using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KIATestApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        //[DataType(DataType.Date)]
        public string DOB { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public Decimal Salary { get; set; }

        [Required]
       // [StringLength(100)]
        public string Designation { get; set; }

        public DateTime ImportedDate { get; set; }

        public string ImportedBy { get; set; }

        public string File { get; set; }

        public string FileSource { get; set; }

        public byte[] Photo { get; set; }

    }
}
