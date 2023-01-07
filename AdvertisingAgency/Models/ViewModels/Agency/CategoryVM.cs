using AdvertisingAgency.Models.Date;
using System.ComponentModel.DataAnnotations;

namespace AdvertisingAgency.Models.ViewModels.Agency
{
    public class CategoryVM
    {
        public CategoryVM()   //default constructor(конструктор по умолчанию)
        {
        }

        public CategoryVM(CategoryDTO row)   //constructor with parameters(конструктор с параметрами)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}