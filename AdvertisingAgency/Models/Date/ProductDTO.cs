using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingAgency.Models.Date
{
    [Table("TableProducts")]
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        //Назначаем внешний ключ
        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }
    }
}