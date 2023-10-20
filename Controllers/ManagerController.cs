using postArticle.Models;
using postArticle.viewmodel;
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
        private RAViewModel emp = new RAViewModel();
        private ReportViewModel RMV = new ReportViewModel();



        // GET: Manager
        public ActionResult UserMange(String search)
        {

            if (search == "")
            {
                var Name1 = db.UserManages.ToList().OrderBy(m => m.UserName);
                return View(Name1);
            }


            if (search != null)
            {
                var searchinput = from p in db.UserManages where p.UserName == search select p;
                return View("UserMange", searchinput);
            }

            else
            {
                var Name = db.UserManages.ToList().OrderBy(m => m.UserName);
                return View(Name);
            }

        }

        public ActionResult Lock(int? id)
        {
            var ID = id;
            var p = db.UserManages.Find(ID);
            p.Status = 1;
            db.SaveChanges();
            var Name = db.UserManages.ToList();
            return View("UserMange", Name);

        }

        public ActionResult Unlock(int? id)
        {
            var ID = id;
            var p = db.UserManages.Find(ID);
            p.Status = 0;
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
        public ActionResult Resize([Bind(Include = "UserID, UserName, Account, Password, Email, UserType, Status, Birthday")] UserManage emp)
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


            /*
            //ArticleID 由大至小
            while (sort == "ID由小至大")
            {
                var RAsort= db.Reports.ToList().Where(m=>m.Status == 0).OrderBy(m=>m.ArticleID);
                return View(RAsort);
            }

            var ReportArticle = db.Reports.ToList().Where(m => m.Status == 0);
            */

            RMV.RA = db.Reports.ToList();
            RMV.RM = db.Report_Message.ToList();



            return View(RMV);

        }



        public ActionResult ArticleReview(int ?id, int ?RID )
        {
            //檢舉文章ID
            var ArticleID = id;
            //檢舉ID
            Report empp = db.Reports.Find(RID);
            Article emps = db.Articles.Find(ArticleID);
           
            ////////////////////////////////////
            RAViewModel emp = new RAViewModel
            {
            ////////////////////////////////////
                Title = emps.Title,
                Content = emps.Content,
                ImageURL = emps.ImageURL,
                ReportID = (int)empp.Report_ID,
                RU_ID = (int)empp.UserID,
                RA = (int)empp.ArticleID,
                RdU = (int)emps.UserID,
                ReportContent = empp.ReportContent
            };



            return View(emp);

           
        }

        ///傳送文章檢舉回復 RRA:return review article///
        ///


        
        public ActionResult RRA(int ?Report_ID)
        {
            int ReportID = (int)Report_ID;
            int RU_ID = Int16.Parse(Request.Form["RU_ID"]);
            int RA = Int16.Parse(Request.Form["RA"]);
            String Report_Content = Request.Form["Report_Content"];
            var content = DateTime.Now.ToString();

            //判斷檢舉通過或未通過(Pass/Failed)

            if (Report_ID != null)
            {

                PF(Report_Content, RA);
                R_Report r_Report = new R_Report { ReportID = ReportID, UserID = RU_ID, Article_ID = RA, Content = "已於" + content + Report_Content, Status = 0 };
                db.R_Report.Add(r_Report);
                //將檢舉設置為已閱
                Read(ReportID);
                db.SaveChanges();

                return Json(new { success = true}, JsonRequestBehavior.AllowGet);

            }

            else {

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }
        }



        public void Read(int ReportID)
        {
            var p = db.Reports.Find(ReportID);
            p.Status = 1;
            db.SaveChanges();
        }


        public void PF(string content, int articleID )
        {

            //尋找被檢舉的文章
            var p = db.Articles.Find(articleID);

            if (content == "檢舉通過")
            {
                p.Status = 1;
            }
            else
            {
                p.Status = 0;
            }
            db.SaveChanges();
        }



        public ActionResult Delete_RA(int ?id)
        {

            var delete = db.Reports.Find(id);

            if (delete != null)
            {
                db.Reports.Remove(delete);
                db.SaveChanges();
            }


            ReportViewModel RMV = new ReportViewModel();
            RMV.RA = db.Reports.OrderBy(p => p.UserID).ToList();
            RMV.RM = db.Report_Message.OrderBy(p => p.Message_ID).ToList();

            //
            return Json(new { success = true}, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Delete_RM(int? id)
        {

            var delete = db.Report_Message.Find(id);

            if (delete != null)
            {
                db.Report_Message.Remove(delete);
                db.SaveChanges();
            }


            ReportViewModel RMV = new ReportViewModel();
            RMV.RA = db.Reports.OrderBy(p => p.UserID).ToList();
            RMV.RM = db.Report_Message.OrderBy(p => p.Message_ID).ToList();

            //
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }





        [HttpGet]
        public ActionResult MSReview(int? id) 
        {

            if (id != null)
            {
                ViewBag.MS_ID = db.Report_Message.Find(id).MS_ID;
                ViewBag.MessageID = db.Report_Message.Find(id).Message_ID;
                ViewBag.UserID = db.Report_Message.Find(id).User_ID;
                ViewBag.Content = db.Report_Message.Find(id).Content;
                ViewBag.Context = db.Messages.Find(ViewBag.MessageID).Content.ToString();

            }
            return View();
        }



    }
}