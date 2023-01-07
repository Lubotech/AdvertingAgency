using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Account;
using AdvertisingAgency.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        //Viewing the shopping cart (Просмотр корзины)
        public ActionResult Index()
        {
            // Cart VM type list declaration (if the session is empty, a new sheet is created)
            // (Объявление list типа CartVM (если сессия пуста создаётся новый лист))
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Checking the bucket = 0 or not (Проверка корзины = 0 или нет)
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your shopping cart is empty please do something about it.";
                return View();
            }

            // The amount and its entry in the ViewBag
            // (Сумма и ее запись во ViewBag)
            int total = 0;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //(Returning a View) Возврат представления
            return View(cart);
        }

        // partial view in the upper right corner with the data about the ads in the shopping cart
        // (частичное представление в верхнем правом углу с данными о рекламе в корзине)
        public ActionResult CartPartial()
        {
            //Объявление модели CartVM
            CartVM model = new CartVM();

            //Объявление переменной количества
            int qty = 0;

            //Объявление переменной цены
            int price = 0;

            //Проверка сессии
            if (Session["cart"] != null)
            {
                //Получение общего количества товаров и цену
                var list = (List<CartVM>) Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //Или цена и кол-во равны 0
                model.Quantity = 0;
                model.Price = 0;
            }
            
            //Возврат частичного представления с моделью
            return PartialView("_CartPartial", model);
        }

        //Добавление рекламы в корзину
        public ActionResult AddToCartPartial(int id)
        {
            //Объявление List типа CartVM   (если сессия пуста создаётся новый лист)
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Объявление модели CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                //(Getting ads) Получение рекламы
                ProductDTO product = db.Products.Find(id);

                // Checking whether an ad is in the shopping cart (Проверка нахождения рекламы в корзине)
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //If not, then adding the product to the cart (Если нет, то добавление товара в корзину)
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }

                // If there is, adding a unit of goods (Если есть, добавление единицы товара)
                else
                {
                    productInCart.Quantity++;
                }
            }

            // If there is, adding a unit of goods (Получение общего кол-ва рекламы, цену и добавление данных в модель)
            int qty = 0;
            int price = 0;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Saving the bucket state to a session (Сохранение состояния корзины в сессию)
            Session["cart"] = cart;

            // (Returning a partial view with a model) Возврат частичного представления с моделью
            return PartialView("_AddToCartPartial", model);
        }

        //GET: /cart/IncrementProduct
        //Increase the number of products (Увеличить кол-во товаров)
        public JsonResult IncrementProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Добавление количества
                model.Quantity++;

                //Сохранение необходимых данных
                var result = new { qty = model.Quantity, price = model.Price };

                //возврат Json ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/DecrementProduct
        //Уменьшить кол-во товаров
        public ActionResult DecrementProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Убавление количества
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //Сохранение необходимых данных
                var result = new { qty = model.Quantity, price = model.Price };

                //возврат Json ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/RemoveProduct
        //Удалить товар из корзины
        public void RemoveProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }

        //GET: /cart/order
        //Оформить заказ
        [HttpGet]
        public ActionResult Order()
        {
            //Получение имени пользователя
            string userName = User.Identity.Name;

            //Объявление модели
            UserProfileVM model;

            if (userName != "")
            {
                using (Db db = new Db())
                {
                    //Получение пользователя
                    UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                    //Инициализируем модель данными
                    model = new UserProfileVM(dto);
                }
            }
            else
            {
                //Оставляем модель пустой
                model = new UserProfileVM();

                //Установка сообщения в Tempdata
                TempData["M"] = "To place an order, log in to your account!";
            }

            //Возврат модели в представление
            return View("Order", model);
        }

        //Отправка заказа на почту
        public bool ProcessOrder(List<CartVM> cart, UserVM UserInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = true;
                smtpClient.Host = "smtp.mail.ru";
                smtpClient.Port = 25;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("luboyerajoel200@gmail.com", "pass1996");


               // smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
               // smtpClient.PickupDirectoryLocation = @"c:\PROmedia_emails";
               // smtpClient.EnableSsl = false;

                StringBuilder body = new StringBuilder()
                    .AppendLine("New order")
                    .AppendLine("---")
                    .AppendLine("Advertisement:");

                var Total = 0;
                foreach (var item in cart)
                {
                    body.AppendFormat("{0} {1}\t{2} * {3} (итого: {4:c}",
                        item.ProductId, item.ProductName, item.Quantity, item.Price, item.Total);
                    Total += item.Total;
                }

                body.AppendFormat("Total cost: {0:c}", Total)
                    .AppendLine("---")
                    .AppendLine("User data:")
                    .AppendLine(UserInfo.FirstName)
                    .AppendLine(UserInfo.LastName)
                    .AppendLine(UserInfo.EmailAdress)
                    .AppendLine(UserInfo.Username);

                MailMessage mailMessage = new MailMessage(
                                       "lubotech@gmail.com",          // От кого 
                                       "luboyerajoel200@gmail.com",       // Кому  пароль 2020pass
                                       "A new order has been sent!",        // Тема
                                       body.ToString());                // Тело письма


                //mailMessage.BodyEncoding = Encoding.UTF8;
                try
                {
                    smtpClient.Send(mailMessage);
                    return true;
                }
                catch (SmtpException e)
                {
                    return false;
                }
            }

        }

        //GET: /cart/order
        //Оформить заказ
        [HttpPost]
        public ActionResult Order(int id)
        {
            //Объявление модели ProductVM
            UserVM model;

            using (Db db = new Db())
            {
                //Получение рекламы
                UserDTO dto = db.Users.Find(id);

                //Инициализация модели данных
                model = new UserVM(dto);
            }

            //Объявление list типа CartVM (если сессия пуста создаётся новый лист)
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if (ProcessOrder(cart, model))
            {
                Session["cart"] = null;
                TempData["I"] = "The order has been sent!";
                return RedirectToAction("Order");
            }
            else
            {
                TempData["M"] = "The order has not been sent!";
                return RedirectToAction("Order");
            }
        }
    }
}
