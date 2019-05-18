using App.Models;
using Model.Context;
using Services;
using Services.Service.EmailService;
using System.Web.Mvc;

namespace App.Controllers.Oferta
{
    public class ContactController : Controller
    {
        public ModelContext modelContext;
        public ContactEmailService contactEmailService;

        public ContactController()
        {
            this.modelContext = new ModelContext();
            this.contactEmailService = new ContactEmailService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Contact/Form.cshtml", new ContactViewModel());
        }

        [HttpPost]
        public ActionResult Send(ContactViewModel contactViewModel)
        {
            var contactDTO = new ContactDTO();
            contactDTO.FirstName = contactViewModel.FirstName;
            contactDTO.LastName = contactViewModel.LastName;
            contactDTO.Company = contactViewModel.Company;
            contactDTO.Email = contactViewModel.Email;
            contactDTO.Reason = contactViewModel.Reason;
            contactDTO.Message = contactViewModel.Message;

            this.contactEmailService.Send(contactDTO);

            return RedirectToAction("Thanks", "../Contact");
        }

        [HttpGet]
        public ActionResult Thanks()
        {
            return View("~/Views/Contact/Thanks.cshtml");
        }
    }
}