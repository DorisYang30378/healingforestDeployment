using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Net;
using postArticle.viewmodel;
using System.Web;
using System.IO;
using System.Web.UI;
using PagedList;


namespace postArticle.Controllers
{
    public class ArticleManageController : Controller
    {
        //---------------基礎屬性-----------------------------------------
        #region 基礎屬性
        private healingForestEntities db = new healingForestEntities();

        public BasicData basicData = new BasicData();
        public bool CheckLoggedIn() => Session["UserID"] != null;

        public int GetUserID() => Convert.ToInt32(Session["UserID"]);

       

        #endregion
        // -----------------------------------------===============================

        #region ===是否自己的文章====
        private bool IsUserArticle(int id)
        {
            int userID = GetUserID();
            var queryArticleList = from Articledb in db.Articles
                                   where Articledb.UserID == userID && Articledb.ArticleID == id
                                   select Articledb;

            return queryArticleList.Any();
        }
        #endregion
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

        public  bool iStatus(int id)
        {
            int userID = GetUserID();
            var queryArticleList = from Articledb in db.Articles
                                   where Articledb.ArticleID == id
                                   select Articledb.Status;

            if (queryArticleList.FirstOrDefault() == 1)
            {
                return false;
            }
            return true;

        }


        //---------------文章內容與留言-----------------------------------------

        public ActionResult ArticleDetails(int? id, int? page)
        {
            //-----分頁-----
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            //
            bool isUser;
            bool isCreatedByUser;
            bool IStatus;
            List<Message> queryMessList = new List<Message>();
            string Display = "none";
            //---------------判斷文章是否存在-----------------------------------------

            #region 判斷文章是否存在
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            // -----------------------------------------===============================

            #region ===是否有登入===
            isUser = false;
            if (CheckLoggedIn())
            {
                isUser = true;
            }
            #endregion

            //---------------該文章底下的留言-----------------------------------------
            queryMessList = (from Messagedb in db.Messages.Include(m => m.Article).Include(m => m.UserManage)
                             where Messagedb.ArticleID == id
                             select Messagedb).ToList();
            //-----------------------------------------===============================

            //判斷是不是自己的文章----------------------
            isCreatedByUser = IsUserArticle((int)id);
            //-----------------------------------------===============================

            //判斷是否封鎖----------------------
            IStatus = iStatus((int)id);
            //-----------------------------------------===============================

            if (!string.IsNullOrEmpty(TempData["Display"] as string))
            {
                Display = TempData["Display"] as string;
            }

            //---------------建立viewModel-----------------------------------------
            ArticleDetailsViewModel articleDetailsViewModel = new ArticleDetailsViewModel
            {
                article = article,
                messages = queryMessList.ToPagedList(pageNumber, pageSize),
                Page = pageNumber,
                isUser = isUser,
                isCreatedByUser = isCreatedByUser,
                Display = Display,
                iStatus = IStatus

            };
            //-----------------------------------------===============================

            return View(articleDetailsViewModel);
        }






        //---------------文章編輯-----------------------------------------
        #region ===文章編輯====
        public ActionResult ArticleEdit(int? id)
        {
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================

            if (CheckLoggedIn())
            {
                if (IsUserArticle((int)id))
                {
                    TempData["ArticleID"] = id;

                    ArticleManageViewModel viewModel = new ArticleManageViewModel();
                    viewModel.article = article;
                    return View(viewModel);
                }
            }

            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(ArticleManageViewModel articleManageViewModel, HttpPostedFileBase file)
        {
            int? id = (int)TempData["ArticleID"];
            TempData["ArticleID"] = id;
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================

            if (CheckLoggedIn() && IsUserArticle((int)id))
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        //刪除文件
                        var path = Path.Combine(Server.MapPath("~/Uploads"), articleManageViewModel.article.ImageURL);
                        if (System.IO.File.Exists(path) && articleManageViewModel.article.ImageURL != "index.png")
                        {

                            System.IO.File.Delete(path);
                        }

                        var fileName = Guid.NewGuid().ToString() + ".png";
                        path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        file.SaveAs(path);

                        articleManageViewModel.article.ImageURL = fileName;
                    }

                    db.Entry(article).CurrentValues.SetValues(articleManageViewModel.article);
                    db.SaveChanges();


                    return RedirectToAction("ArticleDetails", "ArticleManage", new { id });
                }
                else
                {
                    return View(articleManageViewModel);
                }
            }


            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);

        }
        #endregion
        // -----------------------------------------===============================


        // // // 文章刪除 // // //
        public ActionResult ArticleDelete(int? id)
        {
            //---------------判斷文章是否存在-----------------------------------------
            #region ===判斷該頁面是否存在===
            Article article = CheckArticleExists(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            #endregion
            //-----------------------------------------===============================


            if (CheckLoggedIn())
            {
                if (IsUserArticle((int)id))
                {
                    //刪除文件
                    var path = Path.Combine(Server.MapPath("~/Uploads"), article.ImageURL);
                    if (System.IO.File.Exists(path) && article.ImageURL != "index.png")
                    {
                        System.IO.File.Delete(path);
                    }
                    //


                    db.Articles.Remove(article);
                    db.SaveChanges();
                }
            }

            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
        }



        //發布文章get
        public ActionResult ArticlePost()
        {
            if (CheckLoggedIn())
            {
                ArticleManageViewModel viewModel = new ArticleManageViewModel();
                return View(viewModel);

            }
            else
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
        }

        //發布文章post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticlePost(ArticleManageViewModel articlePost, HttpPostedFileBase file)
        {
            if (CheckLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    #region ===搜尋userID===
                    var UserID = Convert.ToInt32(Session["UserID"]);
                    var queryUserSQL = from UserManagedb in db.UserManages.Include(u => u.ThanksfulThing)
                                       where UserManagedb.UserID == UserID
                                       select new
                                       {
                                           UserManagedb.UserType
                                       };

                    var user = queryUserSQL.FirstOrDefault();
                    if (user.UserType.Equals("member"))
                    {
                        articlePost.article.ArticleType = "否";
                    }
                    else
                    {
                        articlePost.article.ArticleType = "是";
                    }
                    #endregion


                    #region ===設置articlePost的資料======
                    //新增文件
                    articlePost.article.ImageURL = "index.png";
                    // 获取当前时间
                    articlePost.article.Time = DateTime.Now;

                    articlePost.article.UserID = UserID;
                    #endregion
                    articlePost.article.Status = 0;

                    #region ===上傳圖片====
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + ".png";
                        var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        file.SaveAs(path);

                        articlePost.article.ImageURL = fileName;
                    }
                    #endregion
                    //
                    db.Articles.Add(articlePost.article);
                    db.SaveChanges();
                }
                else
                {
                    var viewModel = new ArticleManageViewModel
                    {
                        article = articlePost.article
                    };

                    return View(viewModel);
                }
            }

            return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);


        }
    
    
    
    
    
        public void  ReportArticle()
        {
            //ReportViewModel rvm = new ReportViewModel();
            //var rid = (int)rvm.Report_ID;
            var rid = from  a in db.Reports select a.Report_ID;
            var ric = rid.Count()+1;
            
            string name = Request.Form["Name"];
            int id = Int16.Parse(Request.Form["ID"]);
            int ArticleID = Int16.Parse(Request.Form["ArticleID"]);


            var rig = from a in db.Reports
                      where a.UserID == id && a.ArticleID == ArticleID
                      select a;
            

            if (ModelState.IsValid  && !rig.Any())
            {
                Report report_submit = new Report {Report_ID = ric , UserID = id, ArticleID = ArticleID, ReportContent = name, Status = 0 };
                db.Reports.Add(report_submit);
                db.SaveChanges();
            }
            else
            {
                Console.WriteLine("無法重複檢舉");
            }
        } 

   
    
    
    
    
    
    
    }
}