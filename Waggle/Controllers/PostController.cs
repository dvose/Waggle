using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Filters;
using Waggle.Models;
using Waggle.ViewModels;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class PostController : Controller
    {
        private PostContext db = new PostContext();
        public static TopicPost tp  = new TopicPost();

         [InitializeSimpleMembershipAttribute]
        public ActionResult Index(int TopicId)
        {
            ViewData["currentUserId"] = WebSecurity.CurrentUserId;
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList list = new PostList();

            Topic topic = null; //The topic whose posts need to be displayed.
            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.TopicId == TopicId)
                    {
                        topic = t;
                        break;
                    }
                }
            }
            if (topic != null)
            {
                list.TopicId = TopicId;
                list.TopicName = topic.Name;
                //list.ForumName = Forum.ForumList[topic.ForumId].Name; //The forum that the topic is in
                using (ForumContext dbForums = new ForumContext())
                {
                    foreach (Forum f in dbForums.Forums)
                    {
                        if (f.ForumId == topic.ForumId)
                        {
                            list.ForumName = f.Name;
                            tp.forumId = f.ForumId;
                        }
                    }
                }
                list.ForumId = topic.ForumId;
                List<Post> posts = new List<Post>();
                using (PostContext dbPosts = new PostContext())
                {
                    foreach (Post p in dbPosts.Posts)
                    {
                        if (p.TopicId == TopicId)
                        {
                            if (p.AttachmentId != 0)
                                p.findAttachment();
                            posts.Add(p);
                        }
                    }
                }
                list.Posts = posts;

                return View(list);
            }
            return View("ERROR"); //Show an error or something. The topicId given as a parameter to this method doesn't exist.
        }

        [Authorize]
        public ActionResult CreatePost(int topicId)
        {
            Topic topic = null;
            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.TopicId == topicId)
                    {
                        topic = t;
                        break;
                    }
                }
            }
            tp = new TopicPost() { PostTopic = topic, NewPostUserId = WebSecurity.CurrentUserId };
            return View(tp);
        }

        [HttpPost]
        public ActionResult PostCreated(string PostBody, HttpPostedFileBase uploadFile)
        {
            int attachmentId = 0;
             if (uploadFile != null) { 
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

                //save file information

                //define context
                using (FileEntitiesContext db = new FileEntitiesContext())
                {
                    int forumId = -1;
                    //create new model
                    Waggle.Models.File newFile = new Waggle.Models.File();
                    
                    //give model it's attributes - populating columns
                    newFile.fileName = fileName;
                    using (TopicContext tc = new TopicContext()) {
                        foreach (Topic t in tc.Topics) {
                            if (t.TopicId == tp.PostTopic.TopicId) {
                                forumId = t.ForumId;
                            }
                        }
                    }
                    newFile.Forum_Id = forumId;
                    newFile.fileDisplayName = fileDisplayName;
                    newFile.filePath = path;
                    newFile.fileType = extension;
                    newFile.User_Id = WebSecurity.CurrentUserId;
                    
                    //save row
                    db.Files.Add(newFile);
                    db.SaveChanges();
                    attachmentId = newFile.Id;
                }
             }

            Post p = new Post();
            p.UserId = tp.NewPostUserId;
            p.TopicId = tp.PostTopic.TopicId;
            p.Body = PostBody;
            p.IsDeleted = false;
            p.AttachmentId = attachmentId;
            // DateTime d = DateTime.Now;
            // string time = d.DayOfWeek + " " + d.Month + " " + d.day;
            p.PostTime = DateTime.Now.ToString();
            p.ReplyTo = -1;
            Debug.WriteLine(p.PostTime);
            if (ModelState.IsValid)
            {
                db.Posts.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = p.TopicId });
            }
            return View();
        }
        [Authorize]
        public ActionResult Reply(int TopicId, int PostId)
        {
            Topic topic = null;
            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.TopicId == TopicId)
                    {
                        topic = t;
                        break;
                    }
                }
            }
            tp = new TopicPost() { PostTopic = topic, NewPostUserId = WebSecurity.CurrentUserId, replyToPost = PostId };
            return View(tp);
        }

        public ActionResult ReplyCreated(string PostBody, HttpPostedFileBase uploadFile)
        {
            int attachmentId = 0;
            if (uploadFile != null)
            {
                var fileName = Path.GetFileName(uploadFile.FileName);
                var fileDisplayName = fileName;
                string extension = Path.GetExtension(uploadFile.FileName);
                string strPath = "~\\Uploads\\" + WebSecurity.CurrentUserId;
                var path = Path.Combine(Server.MapPath(strPath), fileName);

                //handles duplicate file names
                if (System.IO.File.Exists(path))
                {
                    int i = 1;
                    var originalName = fileName;
                    while (System.IO.File.Exists(path))
                    {
                        fileName = i + originalName;
                        path = Path.Combine(Server.MapPath(strPath), fileName);
                        i++;
                    }
                }
                uploadFile.SaveAs(path);

                //save file information

                //define context
                using (FileEntitiesContext db = new FileEntitiesContext())
                {
                    int forumId = -1;
                    //create new model
                    Waggle.Models.File newFile = new Waggle.Models.File();

                    //give model it's attributes - populating columns
                    newFile.fileName = fileName;
                    using (TopicContext tc = new TopicContext())
                    {
                        foreach (Topic t in tc.Topics)
                        {
                            if (t.TopicId == tp.PostTopic.TopicId)
                            {
                                forumId = t.ForumId;
                            }
                        }
                    }
                    newFile.Forum_Id = forumId;
                    newFile.fileDisplayName = fileDisplayName;
                    newFile.filePath = path;
                    newFile.fileType = extension;
                    newFile.User_Id = WebSecurity.CurrentUserId;

                    //save row
                    db.Files.Add(newFile);
                    db.SaveChanges();
                    attachmentId = newFile.Id;
                }
            }
            Post p = new Post();
            p.UserId = tp.NewPostUserId;
            p.TopicId = tp.PostTopic.TopicId;
            p.Body = PostBody;
            p.IsDeleted = false;
            p.AttachmentId = attachmentId;
            // DateTime d = DateTime.Now;
            // string time = d.DayOfWeek + " " + d.Month + " " + d.day;
            p.PostTime = DateTime.Now.ToString();
            p.ReplyTo = tp.replyToPost; //This is a top-level post, not a reply
            if (ModelState.IsValid)
            {
                db.Posts.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = p.TopicId });
            }
            return View();
        }

        [Authorize]
        public ActionResult Edit(int PostId)
        {
            PostContext dbPosts = new PostContext();
            Post p = dbPosts.Posts.Find(PostId);
            tp = new TopicPost { EditPost = p, EditPostId = p.PostId };
            if (p.AttachmentId != 0)
                p.findAttachment();
            return View(p);
        }

        public ActionResult Edited(string PostBody, HttpPostedFileBase uploadFile)
       {
           Post edited = db.Posts.Find(tp.EditPostId);
            int attachmentId = 0;
            if (edited != null)
            {
                if (uploadFile != null)
                {
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    var fileDisplayName = fileName;
                    string extension = Path.GetExtension(uploadFile.FileName);
                    string strPath = "~\\Uploads\\" + WebSecurity.CurrentUserId;
                    var path = Path.Combine(Server.MapPath(strPath), fileName);

                    //handles duplicate file names
                    if (System.IO.File.Exists(path))
                    {
                        int i = 1;
                        var originalName = fileName;
                        while (System.IO.File.Exists(path))
                        {
                            fileName = i + originalName;
                            path = Path.Combine(Server.MapPath(strPath), fileName);
                            i++;
                        }
                    }
                    uploadFile.SaveAs(path);

                    //save file information

                    //define context
                    using (FileEntitiesContext filedb = new FileEntitiesContext())
                    {
                        int forumId = -1;
                        //create new model
                        Waggle.Models.File newFile = new Waggle.Models.File();

                        //give model it's attributes - populating columns
                        newFile.fileName = fileName;
                        using (TopicContext tc = new TopicContext())
                        {
                            foreach (Topic t in tc.Topics)
                            {
                                if (t.TopicId == edited.TopicId)
                                {
                                    forumId = t.ForumId;
                                }
                            }
                        }
                        newFile.Forum_Id = forumId;
                        newFile.fileDisplayName = fileDisplayName;
                        newFile.filePath = path;
                        newFile.fileType = extension;
                        newFile.User_Id = WebSecurity.CurrentUserId;

                        //save row
                        filedb.Files.Add(newFile);
                        filedb.SaveChanges();
                        attachmentId = newFile.Id;
                    }
                }

                Post afterEdit = new Post
                {
                    PostId = edited.PostId,
                    UserId = edited.UserId,
                    User = edited.User,
                    TopicId = edited.TopicId,
                    Topic = edited.Topic,
                    Body = PostBody,
                    IsDeleted = edited.IsDeleted,
                    PostTime = edited.PostTime,
                    ReplyTo = edited.ReplyTo,
                    AttachmentId = attachmentId
                };
                db.Entry(edited).CurrentValues.SetValues(afterEdit);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = edited.TopicId });
            }
            return RedirectToAction("Index", new { TopicId = edited.TopicId });
        }

        [Authorize]
        public ActionResult Delete(int PostId)
        {
            Post toDelete = db.Posts.Find(PostId);
            Post afterDelete = new Post
            {
                PostId = toDelete.PostId,
                UserId = toDelete.UserId,
                User = toDelete.User,
                TopicId = toDelete.TopicId,
                Topic = toDelete.Topic,
                Body = toDelete.Body,
                IsDeleted = true,
                PostTime = toDelete.PostTime,
                ReplyTo = toDelete.ReplyTo
            };
            db.Entry(toDelete).CurrentValues.SetValues(afterDelete);
            db.SaveChanges();
            return RedirectToAction("Index", new { TopicId = toDelete.TopicId }); 
        }

        /*
        [Authorize]
        public ActionResult DeletePost(int postId)
        {
            if (User.IsInRole("admin"))
            {
                using (PostContext Pdb = new PostContext())
                {
                    foreach (Post p in Pdb.Posts)
                    {
                        if (p.PostId == postId)
                        {
                            p.IsDeleted = true;
                        }
                    }
                    Pdb.SaveChanges();

                return View("Index");
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }
        */
        [Authorize]
        public ActionResult UndeletePost(int postId)
        {
            if (User.IsInRole("admin"))
            {
                using (PostContext Pdb = new PostContext())
                {
                    foreach (Post p in Pdb.Posts)
                    {
                        if (p.PostId == postId)
                        {
                            p.IsDeleted = false;
                        }
                    }
                    Pdb.SaveChanges();

                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }
        
    }
}