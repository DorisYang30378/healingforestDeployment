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
        public UserManage UserManagesDetail { get; set; }

        public IEnumerable<UserQuestion> UserQuestions {get; set;}

        public UserQuestion SubmitUQ { get; set; }


        public IEnumerable<ExpertAnswer> ExpertAS { get; set; }
        public string Questions { get; set; }

        public string nowmood { get; set; }
    }
}