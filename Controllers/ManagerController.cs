using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace postArticle.Controllers
{
    public class ManagerController : Controller
    {

        //初始化資料庫
        private healingForestEntities db = new healingForestEntities();

        int ArticleID = 0;



        // GET: Manager
        public ActionResult UserMange(String search)
        {

            if (search != null)
            {
                var searchinput = from p in db.UserManages where p.UserName == search select p;
                return View("UserMange", searchinput);
            }

            else
            {
                var Name = db.UserManages.ToList().OrderBy(m=>m.UserName);
                return View(Name);
            }

        }

        public ActionResult Delete(int? id)
        {
                var ID = id;
                var p = db.UserManages.Find(ID);
                p.Status = 1;
                db.SaveChanges();
                var Name = db.UserManages.ToList();
                return View("UserMange", Name);
       
        }


        [HttpGet]
        public ActionResult Resize(int? id)
        {
            /*
            String ID = Convert.ToString(id);
            TempData["UserID"] = ID;
            ViewBag.UserID = TempData["UserID"];
            */

            UserManage emp = db.UserManages.Find(id);
            
            return View(emp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resize([Bind(Include ="UserID, UserName, Account, Password, Email, UserType, Status, Birthday")]UserManage emp)
        {

            if (ModelState.IsValid)
            {
                db.Entry(emp).State = EntityState.Modified;
                try { 
                    db.SaveChanges(); 
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors;
                    Response.Write(errorMessages);
                    throw ex;
                }
                

                return RedirectToAction("UserMange");
            }
            return View(emp);
        }




        public ActionResult Report(String sort) {


            //ArticleID 由大至小
            while (sort == "ID由小至大")
            {
                var RAsort= db.Reports.ToList().OrderBy(m=>m.ArticleID);
                return View(RAsort);
            }

            var ReportArticle = db.Reports.ToList();

            return View(ReportArticle);

        }

        public ActionResult ArticleReview(int? id)
        {

            ArticleID = (int)id;
            Article emp = db.Articles.Find(ArticleID);
            return View(emp);

           
        }
       

    }
}