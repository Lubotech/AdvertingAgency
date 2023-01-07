using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Account;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AdvertisingAgency.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Проверка на валидность
            if (!ModelState.IsValid)
                return View("CreateAccount", model);

            //Проверка соответствия пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Invalid password!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                //Проверка имени на уникальность
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Логин {model.Username} занят");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                //Создание экземпляра класса UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                //Добавление всех даанных в модель
                db.Users.Add(userDTO);

                //Сохранение данных
                db.SaveChanges();

                //Добавление роли пользователя
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            //Запись сообщения в TempData
            TempData["M"] = "Now you are registered and can log in to your account";

            //Переадресация пользователя
            return RedirectToAction("Login");
        }

        // GET: account/Login
        [HttpGet]
        public ActionResult Login()
        {
            //Подтверждение авторизации пользователя
            string userName = User.Identity.Name;

            //Если пользователь авторизован, то переадресация на его профиль
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");

            //Возврат представления
            return View();
        }

        // POST: account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
                return View(model);

            //Проверка пользователя на валидность
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                    isValid = true;

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                else
                {
                    //Распределение пользователей по ролям при входе
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    UserDTO dto = db.Users.FirstOrDefault(x => x.Username == model.Username);
                    UserRoleDTO rdto = db.UserRoles.FirstOrDefault(x => x.UserId == dto.Id);
                    HttpCookie cookie = new HttpCookie("Role", rdto.RoleId.ToString());
                    Response.Cookies.Add(cookie);
                    if (rdto.RoleId == 1) return Redirect("~/Admin/Dashboard/Index");
                    else return Redirect("~/Cart/Index");
                }
            }
        }

        //GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserNavPartial()
        {
            //Получение имени пользователя
            string userName = User.Identity.Name;

            //Объявление модели
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                //Получение пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                //Заполненеи модели данными из контекста (DTO)
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            //Возврат частичного представления с моделью
            return PartialView(model);
        }

        //GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //Получение имени пользователя
            string userName = User.Identity.Name;

            //Объявление модели
            UserProfileVM model;

            using (Db db = new Db())
            {
                //Получение пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                //Инициализируем модель данными
                model = new UserProfileVM(dto);
            }
            //Возврат модели в представление
            return View("UserProfile", model);
        }

        //POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //Проверкаа модели на валидность
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            //Проверка пароля, если пользователь его меняет
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords don't match");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                //Получение имени пользователя
                string userName = User.Identity.Name;

                //Проверка имени на уникальность
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"User {model.Username} already exists.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                //Изменение модели контекста данных
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                //Сохранение изменений
                db.SaveChanges();

                //Установка сообщения в Tempdata
                TempData["M"] = "You have changed your profile!";

                //Возврат представления с моделью
                return View("UserProfile", model);
            }
        }
    }
}