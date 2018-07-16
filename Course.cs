using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerProd.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }
        public int Number { get; set; }
        public int GroupNo { get; set; }
        public string Name { get; set; }
        public int Unite { get; set; }
        public int Capacity { get; set; }

    }

}
