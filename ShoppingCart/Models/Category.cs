using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Out of range")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
