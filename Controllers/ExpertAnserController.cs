using postArticle.viewmodel;
using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace postArticle.Controllers
{
    public class ExpertAnserController : Controller
    {

        //初始化
        healingForestEntities db = new healingForestEntities();

        ExpertAnser EA = new ExpertAnser();
        public int GetUserID() => Convert.ToInt32(Session["UserID"]);


        // GET: ExpertAnser
        public ActionResult ExpertAnser(int ?page)
        {

            if (Session["UserID"] != null)
            {
                int PageSize = 5;
                int PageNumber = (page ?? 1);
                var UserQuestions = from s in db.UserQuestions select s;
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
            string AnserContext = Request.Form["AnserContext"];
            int QuestionID = int.Parse(Request.Form["QuestionID"]);

            if(Session["UserID"] != null)
            {
                ExpertAnswer Reponse = new ExpertAnswer { QuestionID = QuestionID, UserID = UserID, AnswerContent = AnserContext, AnswerTime = DateTime.Now };
                db.ExpertAnswers.Add(Reponse);
                db.SaveChanges();

                /*
                EA.ExpertAnswers = db.ExpertAnswers.Where(m=>m.QuestionID== QuestionID);
                return Json(EA.ExpertAnswers);*/

                try
                {
                    EA.ExpertAnswers = db.ExpertAnswers.Where(m => m.QuestionID == QuestionID);
                    return Json(db.ExpertAnswers);
                }
                catch (Exception)
                {
                    return Json(EA.ExpertAnswers);
                }
               



            }
            else
            {
                return RedirectToAction("Login", "UserManages");
            }


        }


    }
}