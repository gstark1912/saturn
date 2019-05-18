using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Controllers.Demanda;
using System.Web.Mvc;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var control = new OfertaController();
            control.crearUsuarios();

        }
    }
}
