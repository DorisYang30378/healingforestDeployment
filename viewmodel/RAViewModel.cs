using postArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class RAViewModel
    {
        public Report Report
        {
            get; set;
        }
        public Article Article
        {
            get; set;
        }


        public string Title { get; set; }
        public string Content { get; set; }
         
        public string ImageURL { get; set; }

        public int ReportID { get; set; }
        public string ReportContent { get; set; }


        //檢舉人ID(Report_User_ID)
        public int RU_ID { get; set; } 

        //檢舉文章ID(Report_Article)
        public int RA { get; set; } 

        //被檢舉人ID(Reported_User)
        public int RdU { get; set; }
    }
}