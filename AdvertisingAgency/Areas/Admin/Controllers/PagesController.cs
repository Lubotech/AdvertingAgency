using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Account;
using AdvertisingAgency.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvertisingAgency.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Объявляем список сущностей для представления (PageVM)
            List<PageVM> PageList;

            //Инициализировать список (Db)
            using (Db db = new Db())
            {
                //Присваиваем объявленному списоку объекты из БД (все сортируем в массиве)
                PageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Возвращаем список в представление
            //Returning the list to the view
            return View(PageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Объявляем переменную для краткого описания (slug)
                string slug;

                //Инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                //Присваиваем заголовок модели (c большой буквы)
                dto.Title = model.Title.ToUpper();

                //Проверяем есть ли краткое описание, если нет - присваиваем описанию название с мал.буквы
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Прверяем, что заголовок и краткое описание уникальны
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "This header already exists.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "This description already exists.");
                    return View(model);
                }

                //Присваиваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;

                dto.Sorting = 100;

                //Сохраняем модель в БД
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Передам сообщение через TenyData (Временные данные)
            TempData["M"] = "You have added a new page:";

            //Переадресовываем пользователя на страницу Index
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель PageVM (Page View Model)
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Проверяем доступность страницы
                if (dto == null)
                {
                    return Content("This page is unavailable.");
                }
                //Присваиваем в модель данные, которые получили
                model = new PageVM(dto);
            }

            //Возвращаем модель в представление
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Получаем id странницы
                int id = model.Id;

                //Объявляем переменную краткого заголовка
                string slug = "home";

                //Проверяем страницу по id
                PagesDTO dto = db.Pages.Find(id);

                //Присваиваем название из полученной модели в DTO
                dto.Title = model.Title;

                //Проверяем краткий заголовок и присваиваем его, если это необходимо
                if (model.Slug != "home")
                {
                    if (String.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Проверяем Slug и Title на уникальность
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Such a title already exists");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Such a short description already exists");
                    return View(model);
                }

                //Записываем остальные значения в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;


                //Сохраняем изменения в БД
                db.SaveChanges();
            }

            //Устанавливаем сообщение в TempData
            TempData["M"] = "You have edited this page.";

            //Переадресация пользователя
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Присваиваем модели информацию из базы
                model = new PageVM(dto);
            }

            //Возвращаем модель в представление
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            //Подключение к бд
            using (Db db = new Db())
            {
                //Получение страницы
                PagesDTO dto = db.Pages.Find(id);

                //Удаление страницы
                db.Pages.Remove(dto);

                //Сохранение изменений в бд
                db.SaveChanges();
            }
            //Оповещение об удалении
            TempData["M"] = "You have successfully deleted the page";

            //Переадресовка пользователя
            return RedirectToAction("Index");
        }

        //Сортировка
        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int [] id)
        {
            //Подключение к бд
            using (Db db = new Db())
            {
                //Реализуем начальный счетчик
                int count = 1;

                //Инициализируем модель данных
                PagesDTO dto;
            
                //Устанавливаем сортировку для каждой страницы
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        //Создание метода списка клиентов
        // GET: Admin/Pages/Clients
        [HttpGet]
        public ActionResult Clients()
        {
            //Объявление модели UserVM типа List
            List<UserVM> listOfUsersVM = new List<UserVM>();

            //Подключение к бд
            using (Db db = new Db())
            {
                List<UserRoleDTO> rdto = db.UserRoles.Where(x => x.RoleId == 2).ToList();
                List<UserDTO> udto = db.Users.ToList();

                var query = from user in udto join role in rdto on user.Id equals role.UserId select user;

                //Инициализирование list и заполнение данными
                foreach (var user in query)
                {
                    listOfUsersVM.Add(new UserVM(user));
                }
            }

            //Возврат представления с данными
            return View(listOfUsersVM);
        }
    }
}