using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Agency;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace AdvertisingAgency.Controllers
{
    public class AgencyController : Controller
    {
        // GET: Agency
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: вывод категорий рекламы в меню
        public ActionResult CategoryMenuPartial()
        {
            //Объявление модели типа list CategoryVM
            List<CategoryVM> categoryVMList;

            //Инициализация модели данными
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            //Возврат частичного представления с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Agency/Category/name Вывод рекламы по категориям
        public ActionResult Category(string name)
        {
            //Объявление списка типа list
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                //Получение id категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                //Инициализация списка данными
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                //Получение имени категории
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                //Проверка на null
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.Categoryname = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            //Возврат представления с моделью
            return View(productVMList);
        }

        // GET: Agency/Product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Объявление модели DTO и VM
            ProductDTO dto;
            ProductVM model;

            //Инициализация id продукта
            int id = 0;

            using (Db db = new Db())
            {
                //Проверка доступности продукта
                if (! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Agency");
                }

                //Инициализируем модель DTO данными
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Получение id 
                id = dto.Id;

                //Инициализируем модель VM данными
                model = new ProductVM(dto);
            }

            //Получение изображении из галереи
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Small"))
                .Select(fn => Path.GetFileName(fn));

            //Возврат модели в представление
            return View("ProductDetails", model);
        }
    }
}