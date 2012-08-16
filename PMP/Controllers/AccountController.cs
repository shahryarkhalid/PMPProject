using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using PMP.Models;
using System.Net.Mail;

namespace PMP.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            ViewBag.eula = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis lorem arcu, pulvinar non elementum eu, commodo vitae ipsum. Phasellus sodales, enim sed laoreet laoreet, mauris augue ultricies mauris, et dapibus lacus nisl nec libero. Sed semper, turpis sed pretium tempor, felis orci consequat turpis, ut dictum orci sapien non enim. Phasellus ultrices luctus faucibus. Morbi mattis nibh et nunc malesuada sit amet porta sem euismod. Etiam turpis risus, mattis eget ornare a, pulvinar sed ipsum. Nulla feugiat posuere molestie. Praesent sit amet urna neque. Quisque blandit iaculis sagittis.";
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.eula = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis lorem arcu, pulvinar non elementum eu, commodo vitae ipsum. Phasellus sodales, enim sed laoreet laoreet, mauris augue ultricies mauris, et dapibus lacus nisl nec libero. Sed semper, turpis sed pretium tempor, felis orci consequat turpis, ut dictum orci sapien non enim. Phasellus ultrices luctus faucibus. Morbi mattis nibh et nunc malesuada sit amet porta sem euismod. Etiam turpis risus, mattis eget ornare a, pulvinar sed ipsum. Nulla feugiat posuere molestie. Praesent sit amet urna neque. Quisque blandit iaculis sagittis.";
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        //
        // GET: /Account/ForgotPassword

        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("");

                    string user = Membership.GetUserNameByEmail(model.Email);
                    MembershipUser currentUser = Membership.GetUser(user);
                    string password = currentUser.ResetPassword();

                    mail.From = new MailAddress("");
                    mail.To.Add(new MailAddress(model.Email));
                    mail.Subject = "Password Reset";
                    mail.Body = "Your password has been reset to: " + password;

                    SmtpServer.Send(mail);
                }
                catch (ArgumentNullException e)
                {
                    ModelState.AddModelError("", "The email was not found");
                    return View(model);
                }
                catch (Exception e)
                {

                }
               
            }            
            return RedirectToAction("ResetPasswordSuccess");
        }

        //
        // GET: /Account/ResetPasswordSuccess

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
