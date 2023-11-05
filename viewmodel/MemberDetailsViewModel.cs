using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using postArticle.Models;

namespace postArticle.viewmodel
{
    public class MemberDetailsViewModel
    {

        public MemberDetailsViewModel()
        {
            SubmitUQ = new UserQuestion();
        }

        public IEnumerable<UserQuestion> UserQuestions {get; set;}
        public IEnumerable<ExpertAnswer> ExpertAS { get; set; }

        public PagedList.IPagedList<UserManage> UserManages { get; set; }

        public PagedList.IPagedList<Article> MemberArticles { get; set; }

        public int UserID { get; set; }

        public UserQuestion SubmitUQ { get; set; }

        public UserManage UserManagesDetail { get; set; }

        public string Questions { get; set; }

        public string NewQ { get; set; }
        public string Date { get; set; }

        public string nowmood { get; set; }
    }
}