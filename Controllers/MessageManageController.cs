using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace postArticle.Controllers
{
    public class MessageManageController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities db = new healingForestEntities();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);

        #endregion
        // -----------------------------------------===============================

        private Article CheckArticleExists(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Article article = db.Articles.Find(id);

            if (article == null)
            {
                return null;
            }

            return article;
        }

        public ActionResult CreateMessage(ArticleDetailsViewModel articleDetailsViewModel, int? id)
        {
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion

            articleDetailsViewModel.Display = "block";

            if (CheckLoggedIn())
            {
                var UserID = GetUserID();
                Message message = new Message();
                requestmsviewmodel ms = new requestmsviewmodel();

                if (!string.IsNullOrEmpty(articleDetailsViewModel.Content))
                {
                    if (ModelState.IsValid)
                    {

                        string username = db.UserManages.Find(UserID).UserName;
                        string context = articleDetailsViewModel.Content;
                        string date = DateTime.Now.ToString("G");

                        ms.UserName = username;
                        ms.Context = context;
                        ms.Date = date;



                        message.ArticleID = (int)id;
                        message.Content = articleDetailsViewModel.Content;
                        message.Time = DateTime.Now;
                        message.UserID = UserID;
                        

                        articleDetailsViewModel.Display = "none";

                        db.Messages.Add(message);

                        if (db.SaveChanges() > 0)
                        {
                            return Json(new { success = true, ms });
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }
                }
                TempData["Display"] = articleDetailsViewModel.Display;
            }

            return RedirectToAction("Login","UserManages");

            
        }
    }
}