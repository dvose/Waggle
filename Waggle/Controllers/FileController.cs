﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Waggle.Models;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /File/Upload

        [Authorize]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadFile, int forumId)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                ViewBag.uploadMessage = "Select a file to upload";
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(uploadFile.FileName);
                var fileDisplayName = fileName;
                string extension = Path.GetExtension(uploadFile.FileName);
                string strPath = "~\\Uploads\\" + WebSecurity.CurrentUserId;
                var path = Path.Combine(Server.MapPath(strPath), fileName);

                //handles duplicate file names
                if (System.IO.File.Exists(path)) { 
                    int i = 1;
                    var originalName = fileName;
                    while (System.IO.File.Exists(path)) {
                        fileName = i + originalName;
                        path = Path.Combine(Server.MapPath(strPath), fileName);
                        i++;
                    }
                }
                uploadFile.SaveAs(path);
                ModelState.Clear();

                //save file information

                //define context
                using (FileEntitiesContext db = new FileEntitiesContext())
                {
                    //create new model
                    Waggle.Models.File newFile = new Waggle.Models.File();
                    
                    //give model it's attributes - populating columns
                    newFile.fileName = fileName;
                    newFile.Forum_Id = forumId;
                    newFile.fileDisplayName = fileDisplayName;
                    newFile.filePath = path;
                    newFile.fileType = extension;
                    newFile.User_Id = WebSecurity.CurrentUserId;
                    
                    //save row
                    db.Files.Add(newFile);
                    db.SaveChanges();
                }
                ViewBag.uploadMessage = "Your file was uploaded!";
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            else {
                ViewBag.uploadMessage = "Your file was could not be uploaded";
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
        }

        public FileResult Download(int Id) {
            Waggle.Models.File file;
            using (FileEntitiesContext db = new FileEntitiesContext()) {
                file = db.Files.Find(Id);
            }
            if (file == null) {
                return null;
            }
            return File(file.filePath, System.Net.Mime.MediaTypeNames.Application.Octet, file.fileName);
        }

        [Authorize]
        public ActionResult Delete(int Id)
        {
            Waggle.Models.File file;
            using (FileEntitiesContext db = new FileEntitiesContext())
            {
                file = db.Files.Find(Id);
                if ((file.User_Id == WebSecurity.CurrentUserId || Roles.IsUserInRole("admin")) && file != null)
                {
                    db.Files.Remove(file);
                    db.SaveChanges();
                }
                else
                {
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
            }
            System.IO.File.Delete(file.filePath);
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

    }
}
