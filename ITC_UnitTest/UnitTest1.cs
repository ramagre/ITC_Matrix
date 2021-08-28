using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ITC_UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckCountValueTest()
        {
            int count = 400;
            HomeController controller = new HomeController();
            ViewResult result = controller.CheckCountValue(count) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
