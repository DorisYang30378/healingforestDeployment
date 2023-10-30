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

    





        /////////////////////////////////////// Report//////////////////////////////////////////////



        public void collectReport()
        {
            RMV.RA = db.Reports.ToList();
            RMV.RM = db.Report_Message.ToList();
        }

        public ActionResult Report() {


            /*
            //ArticleID 由大至小
            while (sort == "ID由小至大")
            {
                var RAsort= db.Reports.ToList().Where(m=>m.Status == 0).OrderBy(m=>m.ArticleID);
                return View(RAsort);
            }

            var ReportArticle = db.Reports.ToList().Where(m => m.Status == 0);
            */

            collectReport();

            return View(RMV);

        }





        ///檢舉和留言檢視

        public ActionResult ArticleReview(int ?id, int ?RID )
        {
            //檢舉文章ID
            var ArticleID = id;
            //檢舉ID
            Report empp = db.Reports.Find(RID);
            Article emps = db.Articles.Find(ArticleID);

            TempData["RRID"] = empp.Report_ID;
            TempData["RAID"] = empp.ArticleID;

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
                ReportContent = empp.ReportContent,
                RAStatus = empp.Status
            };



            return View(emp);

           
        }



        [HttpGet]
        public ActionResult MSReview(int? id)
        {

            if (id != null)
            {
                TempData["rmid"] = db.Report_Message.Find(id).Message_ID;
                TempData["rrmid"] = db.Report_Message.Find(id).MS_ID;
                ViewBag.MS_ID = db.Report_Message.Find(id).MS_ID;
                ViewBag.MessageID = db.Report_Message.Find(id).Message_ID;
                ViewBag.UserID = db.Report_Message.Find(id).User_ID;
                ViewBag.Content = db.Report_Message.Find(id).Content;
                ViewBag.Context = db.Messages.Find(ViewBag.MessageID).Content.ToString();
            }
            return View();
        }






        ///檢舉回復 RRA:return review article///
        public ActionResult RRA(RAViewModel model)
        {

            var time = DateTime.Now.ToString();

            string result = model.result.radioResult;

            if (result == "檢舉通過")
            {

                int RAID = (int)TempData["RAID"];

                var a = db.Articles.Find(RAID);
                a.Status = 1;

                var b = db.Reports.Where(m=>m.ArticleID == RAID).ToList();
                foreach (var i in b)
                {
                    i.Status = 1;
                }



                db.SaveChanges();




            }
            else if (result == "檢舉未通過")
            {
                var b = db.Reports.Find(TempData["RRID"]);
                b.Status = 1;
                db.SaveChanges();
            }
            else
            {

            }









            return RedirectToAction("Report", "Manager");


        }


        ///檢舉回復 RRA:return review Message///

        public ActionResult RRM(ReportResult model)
        {

            string result = model.radioResult;

            if (result == "檢舉通過")
            {

                int rmid = (int)TempData["rmid"];

                var a= db.Messages.Find(rmid);
                    a.Status = 1;


                var c = db.Report_Message.Where(m=>m.Message_ID == rmid).ToList();
                foreach(var d in c)
                {
                    d.Status = 1;
                }
                db.SaveChanges();



            }
            else if (result == "檢舉未通過")
            {
                var b = db.Report_Message.Find(TempData["rrmid"]);
                b.Status = 1;
                db.SaveChanges();
            }
            else
            {

            }



            return RedirectToAction("Report", "Manager");
        }




        ///管理者刪除檢舉
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


    }
}