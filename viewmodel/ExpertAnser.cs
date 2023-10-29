using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using postArticle.Models;
using PagedList.Mvc;
using PagedList;

namespace postArticle.viewmodel
{
    public class ExpertAnser
    {

        public IEnumerable<ExpertAnswer> ExpertAnswers{get; set;}

        public PagedList.IPagedList<UserQuestion> UserQuestions { get; set; }


    }
}