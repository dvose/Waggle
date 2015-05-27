using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Waggle.Models;
using Waggle.Filters;
using Waggle.ViewModels;
using System.IO;
using System.Linq;

namespace Waggle.Controllers
{
    [Authorize]
    [InitializeSimpleMembershipAttribute]
    public class UserController : Controller
    {
        //
        // GET: /User/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /User/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /User/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /User/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /User/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.Email, model.Password, propertyValues: new { Name= model.Name}

                    );
                    Roles.AddUserToRole(model.Email, "normal");
                    
                    //create user profile
                    using (UserEntitiesContext db = new UserEntitiesContext()) {
                        var query = from a in db.Users
                                    where a.Email.Contains(model.Email)
                                    select a;
                        User newUser = query.FirstOrDefault();
                        newUser.AvatarPath = "/Content/UserAvatars/default.png";
                        UserProfile up = new UserProfile();
                        up.User_Id = newUser.Id;
                        up.User = newUser;
                        db.UserProfiles.Add(up);
                        db.SaveChanges();

                        //create user upload directory
                        var strPath = Server.MapPath("~/Uploads/" + newUser.Id);
                        Directory.CreateDirectory(strPath);
                    }
                
                    WebSecurity.Login(model.Email, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /User/Show/{Id}
        [AllowAnonymous]
        public ActionResult Show(int id) {
            UserShowModel model = new UserShowModel();
            model.isCurrentUser = (WebSecurity.CurrentUserId == id);
            using (UserEntitiesContext db = new UserEntitiesContext()) {
                model.user = db.Users.Find(id);
                model.userprofile = db.UserProfiles.Find(id);
            }
            if (model.user != null && model.userprofile != null)
            {
                using (FileEntitiesContext dbFiles = new FileEntitiesContext())
                {
                    model.files = new List<Waggle.Models.File>();
                    var query = from f in dbFiles.Files
                                where f.User_Id == model.user.Id
                                select f;
                    foreach (var file in query)
                    {
                        model.files.Add(file);
                    }
                }
                return View(model);
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /User/Manage/
        public ActionResult Manage()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Show", new { id = WebSecurity.CurrentUserId });
            }
            else { 
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /User/Edit/
        public ActionResult Edit(int id)
        {
            if (Request.IsAuthenticated && WebSecurity.CurrentUserId == id)
            {
                using (UserEntitiesContext db = new UserEntitiesContext()) {
                    UserProfile up = db.UserProfiles.Find(id);
                    up.User = db.Users.Find(id);
                    return View(up);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /User/Edit/
        [HttpPost]
        public ActionResult Edit(UserProfile up)
        {
            if (Request.IsAuthenticated)
            {
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                    UserProfile upDb = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                    upDb.Description = up.Description;

                    db.SaveChanges();
                    return RedirectToAction("Manage", "User");
                }
            }
            else
            {
                return View();
            }
        }

        //POST: /User/Avatar
        [Authorize]
        [HttpPost]
        public ActionResult Avatar(HttpPostedFileBase uploadAvatar) {
            if (uploadAvatar == null || uploadAvatar.ContentLength == 0)
            {
                ViewBag.uploadMessage = "Select a file to upload";
                return View("Edit", "User", new {Id=WebSecurity.CurrentUserId});
            }
            if (ModelState.IsValid)
            {
                string[] picturesExtensions = { ".png", ".gif", ".jpeg", ".jpg"};
                string extension = Path.GetExtension(uploadAvatar.FileName);
                if (!picturesExtensions.Contains(extension))
                {
                    ViewBag.uploadMessage = "Please upload an image";
                    return View("Edit", "User", new { Id = WebSecurity.CurrentUserId });
                }
                string strPath = "~\\Content\\UserAvatars\\";
                var avatarFileName = WebSecurity.CurrentUserId + extension;
                var path = Path.Combine(Server.MapPath(strPath), avatarFileName);
                uploadAvatar.SaveAs(path);
                using (UserEntitiesContext db = new UserEntitiesContext()) {
                    User currentUser = db.Users.Find(WebSecurity.CurrentUserId);
                    currentUser.AvatarPath = "/Content/UserAvatars/" + avatarFileName;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", "User", new { Id = WebSecurity.CurrentUserId });
        }

        [Authorize]
        public ActionResult AvatarDelete(int Id)
        {
            if (WebSecurity.CurrentUserId == Id)
            {
                string oldAvatarPath;
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                   User user = db.Users.Find(Id);
                   oldAvatarPath = user.AvatarPath;
                   user.AvatarPath = "/Content/UserAvatars/default.png";
                   db.SaveChanges();
                }
                System.IO.File.Delete(Server.MapPath("~/"+oldAvatarPath));
            }
            return RedirectToAction("Edit", "User", new { Id = WebSecurity.CurrentUserId });
        }
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Email address already exists. Please enter a different email address.";

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
                    return "The email provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
