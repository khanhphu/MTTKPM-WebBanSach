using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebBanSach.Areas.Admin.Models;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Data
{
    //khi log cac thay doi sach cua Admin
    // AREAS/ADMIN KO ANH XA ADMIN O MODELS.DATA.ADMIN -> ADMINLOG O DAY
    public class AdminLog
    {
        public int Id {  get; set; }   
        public int AdminId {  get; set; } 
        public string Action {  get; set; }    
        public DateTime TimeStamp{ get; set; }
        //khoa ngoai
        [ForeignKey("AdminId")]
        public  Admin Admin { get; set; }

    }
}