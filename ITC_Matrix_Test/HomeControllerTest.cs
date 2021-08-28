<<<<<<< HEAD
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
=======
﻿using ITC_Matrix.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
>>>>>>> e93c95c (added unittest project)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using System.Web.Mvc;
>>>>>>> e93c95c (added unittest project)

namespace ITC_Matrix_Test
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void CheckCountValueTest()
        {
<<<<<<< HEAD
            int count = 400;
            LoginController controller = new HomeController();
=======
            int count = 15;
            LoginController controller = new LoginController();
>>>>>>> e93c95c (added unittest project)
            ViewResult result = controller.CheckCountValue(count) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
