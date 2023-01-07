using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{Page}
        public ActionResult Index(string page = "")
        {
            //Получение/установка краткого заголовка Slug
            if (page == "")
                page = "home";

            //Объявление модели и класса DTO
            PageVM model;
            PagesDTO dto;

            //Проверка доступа страницы
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });
            }

            //Получение DTO страницы
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //Установка заголовка страницы
            ViewBag.PageTitle = dto.Title;

            //Заполнение модели данными
            model = new PageVM(dto);

            //Возврат представления с моделью
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            //Инициализация list PageVM
            List<PageVM> pageVMList;

            //Получение все страниц кроме home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Возврат частичного представления с листом данных
            return PartialView("_PagesMenuPartial", pageVMList);
        }

    }
}