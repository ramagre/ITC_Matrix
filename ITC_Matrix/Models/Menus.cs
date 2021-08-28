using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITC_Matrix.Models
{
    public class Menus
    {
        public int MainMenuID { get; set; }
        public string MainMenuName { get; set; }
        public string MainMenuValue { get; set; }

        public int SubMenuID { get; set; }
        public string SubMenuName { get; set; }
        public string SubMenuValue { get; set; }
    }
}