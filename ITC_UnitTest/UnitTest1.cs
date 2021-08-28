using ITC_Matrix.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;

namespace ITC_UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckCountValueTest()
        {
            int count = 10;
            LoginController controller = new LoginController();
            ViewResult result = controller.CheckCountValue(count) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
