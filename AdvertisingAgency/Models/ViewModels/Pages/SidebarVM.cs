using AdvertisingAgency.Models.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvertisingAgency.Models.ViewModels.Pages
{
    public class SidebarVM
    {
        //Конструктор по умолчанию создается если нет других конструкторов, т.к. у нас есть другой, то создаем пустой конструктор без параметров
        public SidebarVM()
        {
        }

        //Конструктор с параметрами, который получает данные из dto модели
        public SidebarVM(SidebarDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }

        public int Id { get; set; }

        public string Body { get; set; }
    }
}