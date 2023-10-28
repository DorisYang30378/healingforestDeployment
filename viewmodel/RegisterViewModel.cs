using postArticle.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace postArticle.viewmodel
{
    public class RegisterViewModel
    {


        [Required(ErrorMessage = "請確認密碼")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "驗證碼"), DataType(DataType.Text)]
        [Required(ErrorMessage ="請輸入驗證碼")]
        public string code { get; set; }

        [Required(ErrorMessage = "請再次確認密碼")]
        public bool rememberPassword { get; set; }

        public UserManage userManage { get; set; }
    }
}