using App.Model.User;
using App.Models;
using App.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Model.Context;
using Model.Model.Enum;
using Services;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public ModelContext modelContext;

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ModelContext())))
        {
            this.modelContext = new ModelContext();
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordReset(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        return View("~/Views/Account/UsuarioEmailNoConfirmado.cshtml");
                    }

                    if (!user.Enabled)
                    {
                        return View("~/Views/Account/UsuarioNoHabilitado.cshtml");
                    }

                    await SignInAsync(user, model.RememberMe);

                    switch (Convert.ToInt32(user.Roles.FirstOrDefault().RoleId))
                    {
                        case (int)ApplicationRoleEnum.Admin:
                            return RedirectToAction("Home", "../Admin");

                        case (int)ApplicationRoleEnum.Oferta:
                            Session.Add("Company", user.CompanyId);
                            Session["usuarioLogueado"] = user;
                            return RedirectToAction("Home", "../Admin");

                        case (int)ApplicationRoleEnum.Demanda:
                            return RedirectToAction("Home", "../Demanda");
                    }

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();

            var context = new ModelContext();

            var roleStore = new RoleStore<ApplicationRole>(context);
            var roleMngr = new RoleManager<ApplicationRole>(roleStore);

            var roles = roleMngr.Roles.ToList().Where(x => x.UserSelection == true);

            foreach (var role in roles)
            {
                model.Roles.Add(new SelectListItem { Text = role.Name, Value = role.Id });
            }

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                user.Roles.Add(new IdentityUserRole { RoleId = model.Role, UserId = user.Id });

                UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            var context = new ModelContext();

            var roleStore = new RoleStore<ApplicationRole>(context);
            var roleMngr = new RoleManager<ApplicationRole>(roleStore);

            var roles = roleMngr.Roles.ToList().Where(x => x.UserSelection == true);

            foreach (var role in roles)
            {
                model.Roles.Add(new SelectListItem { Text = role.Name, Value = role.Id });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "su contraseña fue modificada."
                : message == ManageMessageId.SetPasswordSuccess ? "Su contraseña fue establecida."
                : message == ManageMessageId.RemoveLoginSuccess ? "El login externo ha sido removido."
                : message == ManageMessageId.Error ? "Ha ocurrido un error."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Remove("Company");
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "../Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            // En algunos casos el código se manda mal por queryString, hay que ver como solucionar esto correctamente.
            var parsedCode = string.Join("",code.Split( ' ' ));
            ApplicationUser user = this.modelContext
                .Users
                .FirstOrDefault(x =>  userId == x.Id  );

            if( user == null || (string.Join("",user.confirmToken.Split('+')) != parsedCode && user.confirmToken != code))
            {
                return View("Error");
            }
            else
            {
                user.EmailConfirmed = true;
                this.modelContext.SaveChanges();
                return View("ConfirmEmail");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotViewModel model)
        {
            var user = this.modelContext
                .Users
                .FirstOrDefault(x => x.EmailConfirmed == true && x.Email == model.Email);
            if (user == null || !user.EmailConfirmed)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }
            user.resetPassToken = UniqueKeyUtil.GetUniqueKey(16);
            await EnviarMailRecuperoPassword(user);
            this.modelContext.SaveChanges();
            return View("ForgotPasswordConfirmation");
        }

        private async Task EnviarMailRecuperoPassword(ApplicationUser user)
        {
            var callbackUrl = Url.Action("ResetPassword", "Account",
            new { UserId = user.Id, code = user.resetPassToken }, protocol: Request.Url.Scheme);

            var emailSender = new EmailSender();
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(user.Email);
            msg.IsBodyHtml = true;
            msg.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["SMTPUsername"], "Equipo de EvaluandoSoftware.com");
            msg.Subject = "Recuperar contraseña en EvaluandoSoftware.com";
            msg.Body = "Estimado " + user.FirstName + " " + user.LastName + ":<br><br>Usted ha solicitado recuperar su contraseña en Evaluando Software. <br>Su nombre de usuario para ingresar al sistema es: " + user.UserName + ". <br>Por favor haga click <a href='" + callbackUrl + "'>aquí</a> para continuar con la recuperación. <br><br> Lo saluda atentamente, <br>el equipo de <a href='evaluandosoftware.com' target='_blank'>EvaluandoSoftware.com</a>";
            emailSender.Send(msg);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            Session["tokenResetPassword"] = code;
            Session["usuarioResetPassword"] = userId;

            return View("~/Views/Account/RecoveryPassword.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RecoveryPassword(ResetPassViewModel model)
        {
            String userId = Session["usuarioResetPassword"].ToString();
            String resetToken = Session["tokenResetPassword"].ToString();
            String newPassword = model.NewPassword;

            ApplicationUser user = this.modelContext
                .Users
                .FirstOrDefault(x => x.Id == userId && x.resetPassToken == resetToken);

            if (user != null && resetToken.Length > 0)
            {
                ApplicationUser cUser = UserManager.FindById(userId);
                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>();
                await store.SetPasswordHashAsync(user, hashedNewPassword);
                user.resetPassToken = UniqueKeyUtil.GetUniqueKey(16);
                this.modelContext.SaveChanges();
            }

            return View("~/Views/Account/Login.cshtml");
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}