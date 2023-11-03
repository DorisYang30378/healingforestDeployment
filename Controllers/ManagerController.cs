using postArticle.Models;
using postArticle.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Net;
using PagedList;

namespace postArticle.Controllers
{
    public class ManagerController : Controller
    {

        //初始化
        public bool CheckLoggedIn() => Session["UserID"] != null;

        //初始化資料庫
        private healingForestEntities db = new healingForestEntities();
        private RAViewModel emp = new RAViewModel();
        private ReportViewModel RMV = new ReportViewModel();
        private MemberDetailsViewModel user = new MemberDetailsViewModel();


        // GET: Manager
        public ActionResult UserMange(String search, int?page)
        {

            int PageSize = 10;
            int PageNumber = (page ?? 1);
            var pushdb = db.UserManages.Where(m=>m.UserType!="Admin").ToList();
            var Name1 = pushdb.ToList().OrderBy(m => m.Status);


            if (Session["UserType"] != null  && Session["UserType"].ToString() == "Admin") {

                if (search == "")
                {


                    user.UserManages = Name1.ToList().ToPagedList(PageNumber, PageSize);


                    return View(user);
                }


                else if (search != null)
                {

                    var searchuserdb = pushdb.Where(m => m.UserName == search).ToList();
                    user.UserManages = searchuserdb.ToList().ToPagedList(PageNumber, PageSize);


                    return View(user);
                }

                else
                {

                    Name1 = pushdb.ToList().OrderBy(m => m.Status);
                    user.UserManages = Name1.ToList().ToPagedList(PageNumber, PageSize);


                    return View(user);
                }
            }

            else
            {
                return RedirectToAction("Login", "UserManages");
            }

        }

        public ActionResult Lock(int? id, int? page)
        {
            int PageSize = 10;
            int PageNumber = (page ?? 1);
            var ID = id;
            var p = db.UserManages.Find(ID);
            p.Status = 1;
            db.SaveChanges();
            var Name = db.UserManages.OrderBy(m => m.Status).ToList();
            user.UserManages = Name.ToList().ToPagedList(PageNumber, PageSize);
            return View("UserMange", user);

        }

        public ActionResult Unlock(int? id, int? page)
        {
            int PageSize = 10;
            int PageNumber = (page ?? 1);
            var ID = id;
            var p = db.UserManages.Find(ID);
            p.Status = 0;
            db.SaveChanges();
            var Name = db.UserManages.OrderBy(m => m.Status).ToList();
            user.UserManages = Name.ToList().ToPagedList(PageNumber, PageSize);
            return View("UserMange", user);
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







        //////////////////////////////////////////////////////////////
        //                 念之寫的                                 //
        //                                                         //
        /////////////////////////////////////////////////////////////

        public ActionResult Index()
        {
            var expertApply = db.ExpertApplies.Include(u => u.UserManage);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckExpert([Bind(Include = "ExpertApplyID,ExpertField,ExpertInfo,Status,UserID,ExpertImgURL,Remark")] ExpertApply expertApply, string submitButton)
        {

            UserManage user = db.UserManages.FirstOrDefault(u => u.UserID == expertApply.UserID);
            if (ModelState.IsValid && user != null)
            {
                if (submitButton == "審核通過")
                {
                    expertApply.Status = "審核通過";
                    user.UserType = "Expert";
                }
                else
                {
                    expertApply.Status = "審核失敗";

                }


                db.Entry(expertApply).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("ExpertDetail");

            }

            return View(expertApply);
        }


        public ActionResult CheckExpert(int? id)
        {
            ExpertApply model = new ExpertApply();
            UserManage usermodel = new UserManage();

            if (!CheckLoggedIn())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpertApply expertApply = db.ExpertApplies.Find(id);
            List<string> imageNames = GetImageNamesFromDatabase(model, usermodel, id);


            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);
            ViewBag.UserName = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserManage.UserName);
            ViewBag.ImageNames = imageNames;
            if (expertApply == null)
            {
                return HttpNotFound();
            }
            return View(expertApply);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            ExpertApply expertApply = db.ExpertApplies.Find(id);

            return RedirectToAction("Index");
        }

        // GET: ExpertApplies/Delete/5
        public ActionResult DeleteExpert(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            ExpertApply expertApply = db.ExpertApplies.Find(id);

            if (expertApply != null)
            {
                db.ExpertApplies.Remove(expertApply);
                db.SaveChanges();
            }
            return RedirectToAction("ExpertDetail");
        }

        // POST: ExpertApplies/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpertApplyID,ExpertField,ExpertInfo,Status,UserID,ExpertImgURL,Remark")] ExpertApply expertApply)
        {
            if (ModelState.IsValid)
            {

                db.Entry(expertApply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);
            ViewBag.UserName = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);

            return View(expertApply);
        }


        // GET: ExpertApplies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpertApply expertApply = db.ExpertApplies.Find(id);
            if (expertApply == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.UserManages, "UserID", "UserName", expertApply.UserID);

            return View(expertApply);
        }

        // GET: Admin
        public ActionResult ExpertDetail()
        {
            var expertApply = db.ExpertApplies.Include(u => u.UserManage);

            if (Session["UserType"].ToString() == "Admin")
            {
                return View(expertApply.ToList());
            }
            else
            {
                return RedirectToAction("Login", "UserManages");
            }
            
        }

        public List<string> GetImageNamesFromDatabase(ExpertApply model, UserManage usermodel, int? id)
        {
            List<string> imageNames = new List<string>();


            using (healingForestEntities dbContext = new healingForestEntities())
            {
                // 假設您的資料庫中有一個名為 "Images" 的資料表，其中包含一個名為 "FileName" 的欄位存儲檔案名稱
                model.UserID = (int)Session["UserID"];
                // 查詢資料庫獲取檔案名稱

                var query = from expertApply in db.ExpertApplies
                            where expertApply.ExpertApplyID == id
                            select expertApply.ExpertImgURL;

                // 將檔案名稱加入到結果列表中
                imageNames = query.ToList();
            }


            return imageNames;
        }





    }
}