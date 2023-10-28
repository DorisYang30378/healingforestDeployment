using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class LoginViewModel
    {
        [Display(Name = "暱稱"), Required(ErrorMessage ="請輸入帳號")]
        public string username { get; set; }

        [Display(Name = "密碼"), Required(ErrorMessage ="請輸入密碼"), DataType(DataType.Password)]
        public string password { get; set; }

        public string ErrMessage { get; set; }

    }

}