using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerProd.Models
{ 

    public class Register 
    {
        [Key]
        public int ID { get; set; }
        public long stu_id { get; set; }
        public int course_id { get; set; }
        public string date { get; set; }
        public int status { get; set; }
    }
}
