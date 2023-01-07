using AdvertisingAgency.Models.Date;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdvertisingAgency.Models.ViewModels.Agency
{
    public class ProductVM
    {
        //Конструктор без параметра по умолчанию
        //Constructor without default parameter
        public ProductVM() { }

        // Constructor with the parameter
        // Конструктор с параметром
        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }
        [Required]   //The field is required (Поле обязательно)
        [DisplayName("Title")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Price")]
        public int Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Picture")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}