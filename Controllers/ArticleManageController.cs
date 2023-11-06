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
using PagedList;
using HtmlAgilityPack;

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

        private bool IsCollect(int id)
        {
            int userID = GetUserID();
            var queryArticleList = from Articledb in db.Collects
                                   where Articledb.UserID == userID && Articledb.ArticleID == id
                                   select Articledb;

            return queryArticleList.Any();
        }

        #region ===收藏管理===
        //取消收藏文章
        public ActionResult CancelCollect(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article article = db.Articles.Find(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            else if (!CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            else
            {

                var UserID = GetUserID();
                var queryCollectSQL = from Collectdb in db.Collects.Include(c => c.Article).Include(c => c.UserManage)
                                      where Collectdb.UserID == UserID && Collectdb.ArticleID == article.ArticleID
                                      select Collectdb;

                if (queryCollectSQL.Any())
                {
                    article.Likes--;
                    db.Entry(article).CurrentValues.SetValues(article);

                    db.Collects.RemoveRange(queryCollectSQL);
                    db.SaveChanges();
                }


                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString, new { page });

            }
        }


        //進行收藏文章
        public ActionResult ArticleCollect(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article article = db.Articles.Find(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            else if (!CheckLoggedIn())
            {
                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString);
            }
            else
            {
                #region ===存資料到collect===
                var UserID = GetUserID();
                var queryCollectSQL = from Collectdb in db.Collects.Include(c => c.Article).Include(c => c.UserManage)
                                      where Collectdb.UserID == UserID && Collectdb.ArticleID == article.ArticleID
                                      select Collectdb;

                if (!queryCollectSQL.Any())
                {
                    Collect collect = new Collect();
                    collect.UserID = UserID;
                    collect.ArticleID = article.ArticleID;
                    collect.Time = DateTime.Now;

                    article.Likes++;
                    db.Entry(article).CurrentValues.SetValues(article);
                    db.Collects.Add(collect);
                    db.SaveChanges();
                }
                #endregion



                return RedirectToAction(basicData.HomeViewString, basicData.HomeControllerString, new { page });
            }

        }
        #endregion



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

            //
            bool isUser;
            bool isCreatedByUser;
            bool IStatus;
            List<Message> queryMessList = new List<Message>();
            string Display = "none";
            int UserID = GetUserID();
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
                messages = queryMessList,
                isUser = isUser,
                isCreatedByUser = isCreatedByUser,
                Display = Display,
                IsCollect = IsCollect(article.ArticleID),
                iStatus = IStatus,
                //
                RM = db.Report_Message.Where(i => i.User_ID == UserID)

            };
            //-----------------------------------------===============================

            return View(articleDetailsViewModel);
        }





        //---------------文章編輯-----------------------------------------
        #region ===文章編輯(GET)====
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
        #endregion


        //---------------文章編輯-----------------------------------------
        #region ===文章編輯(POST)====
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
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
                        string processedContent = ProcessHtmlContent(articleManageViewModel.article.Content);
                        articleManageViewModel.article.Content = processedContent;

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

        public string ProcessHtmlContent(string htmlContent)
        {
            // 創建 HtmlDocument 實例
            HtmlDocument htmlDocument = new HtmlDocument();

            // 載入 HTML 內容
            htmlDocument.LoadHtml(htmlContent);

            // 選擇要處理的 HTML 標籤，例如 <p>
            var paragraphs = htmlDocument.DocumentNode.Descendants("p");

            // 遍歷每個 <p> 標籤
            foreach (var paragraph in paragraphs)
            {
                // 在這裡可以進行適當的處理，例如對內容進行分段
                // 這裡只是一個示例，你可以根據具體需求進行處理

                // 在每個 <p> 標籤後面插入分段符號 <br>
                paragraph.InnerHtml += "<br/>";
            }

            // 獲取處理後的 HTML 內容
            string processedContent = htmlDocument.DocumentNode.OuterHtml;

            // 返回處理後的 HTML 內容
            return processedContent;
        }



        //---------------文章刪除-----------------------------------------
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
        [ValidateInput(true)]
        public ActionResult ArticlePost(ArticleManageViewModel articlePost, HttpPostedFileBase file)
        {
            var UserID = Convert.ToInt32(Session["UserID"]);
            if (CheckLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    #region ===搜尋userID===
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

                    if (db.SaveChanges() > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("發布成功");
                        var experience= db.UserManages.Find(UserID);
                        experience.Experience += 10;
                        db.SaveChanges();
                    }

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






        //檢舉文章
        public void  ReportArticle()
        {
            //ReportViewModel rvm = new ReportViewModel();
            //var rid = (int)rvm.Report_ID;
            var rid = from  a in db.Reports select a.Report_ID;
            var ric = rid.Count()+1;
            
            string name = Request.Form["Context"];
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
                System.Diagnostics.Debug.WriteLine("你重複檢舉了");
            }
        } 


        //檢舉留言
        public void ReportMessage()
        {

            int MSID = int.Parse(Request.Form["MSID"]);
            int USERID = int.Parse(Request.Form["USERID"]);
            string CONTEXT = Request.Form["CONTEXT"];

            //將留言檢舉內容寫入資料庫
            Report_Message Rm = new Report_Message {Message_ID = MSID, User_ID= USERID, Content=CONTEXT , Status= 0 };
            db.Report_Message.Add(Rm);
            db.SaveChanges();
        }

   
    
    
    
    
    
    
    }
}