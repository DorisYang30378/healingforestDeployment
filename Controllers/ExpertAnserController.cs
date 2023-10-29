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


        // GET: ExpertAnser
        public ActionResult ExpertAnser(int ?page)
        {

            int PageSize = 10;
            int PageNumber = (page ?? 1);
            ExpertAnser EA = new ExpertAnser();

            var UserQuestions = from s in db.UserQuestions select s ;
            var UserQuestioms_toPageList = UserQuestions.ToList().ToPagedList(PageNumber, PageSize);
            EA.UserQuestions = UserQuestioms_toPageList;
            EA.ExpertAnswers = db.ExpertAnswers;
            return View(EA);
        }
    }
}