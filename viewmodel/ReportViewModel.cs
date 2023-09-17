using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using postArticle.Models;

namespace postArticle.viewmodel
{
    public class ReportViewModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Report_ID{ get; set; }
    }
}