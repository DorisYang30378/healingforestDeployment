using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using postArticle.Models;
using postArticle.viewmodel;
using System.Data;
using System.Data.SqlClient;  //搭配MS資料庫

namespace postArticle.Controllers
{
    public class DateViewController : Controller
    { 

        //索取Session[UserID]

        int id;
        public void UserId() {
            id = (int)this.Session["UserId"];
        }

        //透過Enitity連接資料庫
        public void connectSQL()
        {
            var db = new healingForestEntities();
            if (ViewBag.CheckIn=="簽到")
            {
                Checkin p = new Checkin { UserID = id, CheckInDate = DateTime.Now.Date, BoolCheckIn = 1 };
                db.Checkins.Add(p);
                db.SaveChanges();
            }
        }


        public void Day()
        {
            string[] month_olympic = new string[] { "31", "29", "31", "30", "31", "30", "31", "31", "30", "31", "30", "31" };
            int[] month_normal = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            string[] month_name = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };


            //今天日期
            var my_date = DateTime.Now;
            var my_year = my_date.ToString("yyyy");
            var my_month = my_date.Month;
            var my_day = my_date.ToString("dd");
            ViewBag.mg = my_date.ToString("MMMM");


            //該月首天
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            var week = FirstDay.DayOfWeek;
            //該月首日星期幾
            ViewBag.firstweekdate = week.ToString("d");


            //該月總天數
            var month_totalday = monthday(my_month);
            ViewBag.date = month_totalday;

            int monthday(int my_mont)
            {
                return month_normal[my_mont];
            }   

        }


        //(透過資料庫判斷是否簽到和User當月Checkin日期)
        public void Read()
        {
            using (var db =new healingForestEntities())
            {
                var product = from p in db.Checkins where p.UserID==id select p;
                var my_day = DateTime.Now.ToString("dd");
                var my_month = DateTime.Now.Month;
                int count=1;
                int[] array = new int[32];
 
                foreach (var i in product)
                {
                    //將User當月checkin date寫入陣列
                    if (i.CheckInDate.Month == my_month)
                    {
                        array[count] = int.Parse(i.CheckInDate.ToString("dd"));
                        count++;
                    }
            

                    //當日是否簽到
                    if (i.CheckInDate.ToString("dd") == my_day) {
                        ViewBag.CheckIn = "已簽到";
                    };

                    //將checkin傳回View
                    ViewBag.product = array;

                }
            }

        }




        public ActionResult dateView()
        {
            ViewBag.CheckIn = "簽到";
            UserId();
            //Response.Write(id);
            Read();
            Day();
            connectSQL();
            return View();
        }




    }

}