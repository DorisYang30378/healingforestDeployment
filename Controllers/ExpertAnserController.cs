using postArticle.viewmodel;
using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using _Platform.Service;

namespace postArticle.Controllers
{
    public class ExpertAnserController : Controller
    {

        //初始化

        //model
        healingForestEntities db = new healingForestEntities();


        //view model
        ExpertAnser EA = new ExpertAnser();
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);


        // GET: ExpertAnser
        public ActionResult ExpertAnser(int ?page)
        {

            if (Session["UserID"] != null)
            {
                int PageSize = 5;
                int PageNumber = (page ?? 1);
                var UserQuestions = db.UserQuestions.OrderByDescending(m=>m.QuestionTime);
                var UserQuestioms_toPageList = UserQuestions.ToList().ToPagedList(PageNumber, PageSize);
                EA.UserQuestions = UserQuestioms_toPageList;
                EA.ExpertAnswers = db.ExpertAnswers;
                return View(EA);
            }
            else
            {
                return RedirectToAction("Login", "UserManages");
            }

        }


        public ActionResult SendAnser() {

            int UserID = GetUserID();
            string UserName = db.UserManages.Find(UserID).UserName;
            string AnserContext = Request.Form["AnserContext"];
            int QuestionID = int.Parse(Request.Form["QuestionID"]);
            string UQEmail = Request.Form["UQEmail"];
            string UQcontext = Request.Form["UQcontext"];



            if (Session["UserID"] != null)
            {
                ExpertAnswer Reponse = new ExpertAnswer { QuestionID = QuestionID, UserID = UserID, AnswerContent = AnserContext, AnswerTime = DateTime.Now };
                db.ExpertAnswers.Add(Reponse);
                db.SaveChanges();
                ResponesEmail(UserName, UQEmail, UQcontext);

                ExpertAnser send = new ExpertAnser();
                send.Name = db.UserManages.Find(UserID).UserName;
                send.Anser = AnserContext;
                send.Time = DateTime.Now.ToString("d");

                return Json(new { success= true, send});

            }
            else
            {
                return RedirectToAction("Login", "UserManages");
            }


        }

        public void ResponesEmail(string UserName, string UQEmail, string UQcontext)
        {
            MaillService ms = new MaillService();
            string subject = "問題回答";
            string body = @"<p>專家:{UserName}<br>
                            回答您的問題:{UQcontext}<br>
                            ==================================<br />
                            此為系統自動回覆之信件，請勿直接回信<br />
                            ==================================<br />
                             </p>";
            body = body.Replace("{UserName}", " "+UserName);
            body = body.Replace("{UQcontext}", " "+UQcontext);
            ms.SendMail(UQEmail, subject, body);
        }

    }
}