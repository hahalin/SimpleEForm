using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.Mvc;
namespace eform.Models
{
    public class ForumItem
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        public string prjId { get; set; }
        public string workNo { get; set; }
        public DateTime billDate { get; set; }
        public int seq { get; set; } = 1;
        public string subject { get; set; }
        public string subTitle { get; set; }
        public string smemo { get; set; }
        public string othersA1 { get; set; }
        public string othersA2 { get; set; }
        public string othersA3 { get; set; }
        public string othersA4 { get; set; }
        public string othersA5 { get; set; }
        public string othersA6 { get; set; }
        public string othersA7 { get; set; }
        public string othersA8 { get; set; }
        public string othersA9 { get; set; }
        public string othersA10 { get; set; }
        public string othersB1 { get; set; }
        public string othersB2 { get; set; }
        public string othersB3 { get; set; }
        public string othersB4 { get; set; }
        public string othersB5 { get; set; }
        public string othersB6 { get; set; }
        public string othersB7 { get; set; }
        public string othersB8 { get; set; }
        public string othersB9 { get; set; }
        public string othersB10 { get; set; }
        public string sfile { get; set; }
        public string sfile2 { get; set; }
        public string sfile3 { get; set; }
        public string sfile4 { get; set; }
        public string sfile5 { get; set; }
        public Boolean isPrivate { get; set; } = false;
    }

    public class vwForumItem
    {
        ApplicationDbContext ctx;
        public vwForumItem()
        {
            ctx = new ApplicationDbContext();
        }
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "原文標題")]
        public string pTitle { get; set; }
        public string prjId { get; set; }
        [Display(Name = "作者")]
        public string workNo { get; set; }
        [Display(Name = "作者")]
        public vwEmployee creator
        {
            get
            {
                return ctx.getUserByWorkNo(workNo);
            }
        }

        [Display(Name = "建立日期")]
        public DateTime billDate { get; set; }
        [Display(Name = "序號")]
        public int seq { get; set; } = 1;
        [Display(Name = "標題")]
        public string subject { get; set; }
        [Display(Name = "回覆標題")]
        public string subTitle { get; set; }
        [Display(Name = "內容")]
        public string smemo { get; set; }
        public string othersA1 { get; set; }
        public string othersA2 { get; set; }
        public string othersA3 { get; set; }
        public string othersA4 { get; set; }
        public string othersA5 { get; set; }
        public string othersA6 { get; set; }
        public string othersA7 { get; set; }
        public string othersA8 { get; set; }
        public string othersA9 { get; set; }
        public string othersA10 { get; set; }
        public string othersB1 { get; set; }
        public string othersB2 { get; set; }
        public string othersB3 { get; set; }
        public string othersB4 { get; set; }
        public string othersB5 { get; set; }
        public string othersB6 { get; set; }
        public string othersB7 { get; set; }
        public string othersB8 { get; set; }
        public string othersB9 { get; set; }
        public string othersB10 { get; set; }
        [Display(Name = "正本")]
        public string othersA
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                List<string> lista = new List<string>();
                if (!string.IsNullOrEmpty(othersA1))
                {
                    list.Add(ctx.getUserByWorkNo(othersA1));
                }
                if (!string.IsNullOrEmpty(othersA2))
                {
                    list.Add(ctx.getUserByWorkNo(othersA2));
                }
                if (!string.IsNullOrEmpty(othersA3))
                {
                    list.Add(ctx.getUserByWorkNo(othersA3));
                }
                if (!string.IsNullOrEmpty(othersA4))
                {
                    list.Add(ctx.getUserByWorkNo(othersA4));
                }
                if (!string.IsNullOrEmpty(othersA5))
                {
                    list.Add(ctx.getUserByWorkNo(othersA5));
                }
                if (!string.IsNullOrEmpty(othersA6))
                {
                    list.Add(ctx.getUserByWorkNo(othersA6));
                }
                if (!string.IsNullOrEmpty(othersA7))
                {
                    list.Add(ctx.getUserByWorkNo(othersA7));
                }
                if (!string.IsNullOrEmpty(othersA8))
                {
                    list.Add(ctx.getUserByWorkNo(othersA8));
                }
                if (!string.IsNullOrEmpty(othersA9))
                {
                    list.Add(ctx.getUserByWorkNo(othersA9));
                }
                if (!string.IsNullOrEmpty(othersA10))
                {
                    list.Add(ctx.getUserByWorkNo(othersA10));
                }

                foreach (var u in list)
                {
                    lista.Add(u.UserEName);
                }
                return (string.Join(",", lista.ToArray()));
            }
        }
        [Display(Name = "副本")]
        public string othersB
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                List<string> lista = new List<string>();
                if (!string.IsNullOrEmpty(othersB1))
                {
                    list.Add(ctx.getUserByWorkNo(othersB1));
                }
                if (!string.IsNullOrEmpty(othersB2))
                {
                    list.Add(ctx.getUserByWorkNo(othersB2));
                }
                if (!string.IsNullOrEmpty(othersB3))
                {
                    list.Add(ctx.getUserByWorkNo(othersB3));
                }
                if (!string.IsNullOrEmpty(othersB4))
                {
                    list.Add(ctx.getUserByWorkNo(othersB4));
                }
                if (!string.IsNullOrEmpty(othersB5))
                {
                    list.Add(ctx.getUserByWorkNo(othersB5));
                }
                if (!string.IsNullOrEmpty(othersB6))
                {
                    list.Add(ctx.getUserByWorkNo(othersB6));
                }
                if (!string.IsNullOrEmpty(othersB7))
                {
                    list.Add(ctx.getUserByWorkNo(othersB7));
                }
                if (!string.IsNullOrEmpty(othersB8))
                {
                    list.Add(ctx.getUserByWorkNo(othersB8));
                }
                if (!string.IsNullOrEmpty(othersB9))
                {
                    list.Add(ctx.getUserByWorkNo(othersB9));
                }
                if (!string.IsNullOrEmpty(othersB10))
                {
                    list.Add(ctx.getUserByWorkNo(othersB10));
                }

                foreach (var u in list)
                {
                    lista.Add(u.UserEName);
                }
                return (string.Join(",", lista.ToArray()));
            }
        }
        public bool mobile { get; set; } = false;

        public List<string> mailNmAList
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                List<string> lista = new List<string>();
                if (!string.IsNullOrEmpty(othersA1))
                {
                    list.Add(ctx.getUserByWorkNo(othersA1));
                }
                if (!string.IsNullOrEmpty(othersA2))
                {
                    list.Add(ctx.getUserByWorkNo(othersA2));
                }
                if (!string.IsNullOrEmpty(othersA3))
                {
                    list.Add(ctx.getUserByWorkNo(othersA3));
                }
                if (!string.IsNullOrEmpty(othersA4))
                {
                    list.Add(ctx.getUserByWorkNo(othersA4));
                }
                if (!string.IsNullOrEmpty(othersA5))
                {
                    list.Add(ctx.getUserByWorkNo(othersA5));
                }
                if (!string.IsNullOrEmpty(othersA6))
                {
                    list.Add(ctx.getUserByWorkNo(othersA6));
                }
                if (!string.IsNullOrEmpty(othersA7))
                {
                    list.Add(ctx.getUserByWorkNo(othersA7));
                }
                if (!string.IsNullOrEmpty(othersA8))
                {
                    list.Add(ctx.getUserByWorkNo(othersA8));
                }
                if (!string.IsNullOrEmpty(othersA9))
                {
                    list.Add(ctx.getUserByWorkNo(othersA9));
                }
                if (!string.IsNullOrEmpty(othersA10))
                {
                    list.Add(ctx.getUserByWorkNo(othersA10));
                }

                foreach (var u in list)
                {
                    if (!string.IsNullOrEmpty(u.UserEName))
                        lista.Add(u.UserEName);
                }
                return lista;
            }
        }
        public List<string> mailNmBList
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                List<string> lista = new List<string>();
                if (!string.IsNullOrEmpty(othersB1))
                {
                    list.Add(ctx.getUserByWorkNo(othersB1));
                }
                if (!string.IsNullOrEmpty(othersB2))
                {
                    list.Add(ctx.getUserByWorkNo(othersB2));
                }
                if (!string.IsNullOrEmpty(othersB3))
                {
                    list.Add(ctx.getUserByWorkNo(othersB3));
                }
                if (!string.IsNullOrEmpty(othersB4))
                {
                    list.Add(ctx.getUserByWorkNo(othersB4));
                }
                if (!string.IsNullOrEmpty(othersB5))
                {
                    list.Add(ctx.getUserByWorkNo(othersB5));
                }
                if (!string.IsNullOrEmpty(othersB6))
                {
                    list.Add(ctx.getUserByWorkNo(othersB6));
                }
                if (!string.IsNullOrEmpty(othersB7))
                {
                    list.Add(ctx.getUserByWorkNo(othersB7));
                }
                if (!string.IsNullOrEmpty(othersB8))
                {
                    list.Add(ctx.getUserByWorkNo(othersB8));
                }
                if (!string.IsNullOrEmpty(othersB9))
                {
                    list.Add(ctx.getUserByWorkNo(othersB9));
                }
                if (!string.IsNullOrEmpty(othersB10))
                {
                    list.Add(ctx.getUserByWorkNo(othersB10));
                }

                foreach (var u in list)
                {
                    if (!string.IsNullOrEmpty(u.UserEName))
                        lista.Add(u.UserEName);
                }
                return lista;
            }
        }

        public List<string> mailList
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                List<string> lista = new List<string>();
                if (!string.IsNullOrEmpty(othersA1))
                {
                    list.Add(ctx.getUserByWorkNo(othersA1));
                }
                if (!string.IsNullOrEmpty(othersA2))
                {
                    list.Add(ctx.getUserByWorkNo(othersA2));
                }
                if (!string.IsNullOrEmpty(othersA3))
                {
                    list.Add(ctx.getUserByWorkNo(othersA3));
                }
                if (!string.IsNullOrEmpty(othersA4))
                {
                    list.Add(ctx.getUserByWorkNo(othersA4));
                }
                if (!string.IsNullOrEmpty(othersA5))
                {
                    list.Add(ctx.getUserByWorkNo(othersA5));
                }
                if (!string.IsNullOrEmpty(othersA6))
                {
                    list.Add(ctx.getUserByWorkNo(othersA6));
                }
                if (!string.IsNullOrEmpty(othersA7))
                {
                    list.Add(ctx.getUserByWorkNo(othersA7));
                }
                if (!string.IsNullOrEmpty(othersA8))
                {
                    list.Add(ctx.getUserByWorkNo(othersA8));
                }
                if (!string.IsNullOrEmpty(othersA9))
                {
                    list.Add(ctx.getUserByWorkNo(othersA9));
                }
                if (!string.IsNullOrEmpty(othersA10))
                {
                    list.Add(ctx.getUserByWorkNo(othersA10));
                }

                if (!string.IsNullOrEmpty(othersB1))
                {
                    list.Add(ctx.getUserByWorkNo(othersB1));
                }
                if (!string.IsNullOrEmpty(othersB2))
                {
                    list.Add(ctx.getUserByWorkNo(othersB2));
                }
                if (!string.IsNullOrEmpty(othersB3))
                {
                    list.Add(ctx.getUserByWorkNo(othersB3));
                }
                if (!string.IsNullOrEmpty(othersB4))
                {
                    list.Add(ctx.getUserByWorkNo(othersB4));
                }
                if (!string.IsNullOrEmpty(othersB5))
                {
                    list.Add(ctx.getUserByWorkNo(othersB5));
                }
                if (!string.IsNullOrEmpty(othersB6))
                {
                    list.Add(ctx.getUserByWorkNo(othersB6));
                }
                if (!string.IsNullOrEmpty(othersB7))
                {
                    list.Add(ctx.getUserByWorkNo(othersB7));
                }
                if (!string.IsNullOrEmpty(othersB8))
                {
                    list.Add(ctx.getUserByWorkNo(othersB8));
                }
                if (!string.IsNullOrEmpty(othersB9))
                {
                    list.Add(ctx.getUserByWorkNo(othersB9));
                }
                if (!string.IsNullOrEmpty(othersB10))
                {
                    list.Add(ctx.getUserByWorkNo(othersB10));
                }

                foreach (var u in list)
                {
                    if(!string.IsNullOrEmpty(u.Email))
                    lista.Add(u.Email);
                }
                return lista;
            }
        }

        [Display(Name = "附件")]
        public string sfile { get; set; }
        public string sfile2 { get; set; }
        public string sfile3 { get; set; }
        public string sfile4 { get; set; }
        public string sfile5 { get; set; }
        public vwEmployee vWorkNo
        {
            get
            {
                return ctx.getUserByWorkNo(workNo);
            }
        }
        public string floor
        {
            get
            {
                if (seq >= 1)
                {
                    return (seq)  + "F";
                }
                return "";
            }
        }
        public int replyCnt
        {
            get
            {
                int r = 0;
                r = ctx.prjForumItems.Where(x => x.pid == id).Count();
                return r;
            }
        }
        public string lastReplyDate
        {
            get
            {
                var item = ctx.prjForumItems.Where(x => x.pid == id).OrderByDescending(x => x.billDate).FirstOrDefault();
                if(item==null)
                {
                    return "";
                }
                else
                {
                    return item.billDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

        }
        public Boolean isPrivate { get; set; } = false;
        public string page { get; set; }
    }

    public class vwUpateForumItem
    {
        ApplicationDbContext ctx;
        public vwUpateForumItem()
        {
            ctx = new ApplicationDbContext();
        }
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "原文標題")]
        public string pTitle { get; set; }
        public string prjId { get; set; }
        [Display(Name = "作者")]
        public string workNo { get; set; }
        [Display(Name = "作者")]
        public vwEmployee creator
        {
            get
            {
                return ctx.getUserByWorkNo(workNo);
            }
        }

        [Display(Name = "建立日期")]
        public DateTime billDate { get; set; }
        public int seq { get; set; } = 1;
        [Display(Name = "標題")]
        public string subject { get; set; }
        [Display(Name = "回覆標題")]
        public string subTitle { get; set; }
        [Display(Name = "內容")]
        public string smemo { get; set; }
        public vwEmployee vWorkNo
        {
            get
            {
                return ctx.getUserByWorkNo(workNo);
            }
        }
        public string page { get; set; }
        public bool mobile { get; set; } = false;
    }

    public class vwForumDetail
    {
        ApplicationDbContext ctx;
        public vwForumDetail(string id, string code)
        {
            this.code = code;
            ctx = new ApplicationDbContext();
            ForumItem fitem = ctx.prjForumItems.Where(x => x.id == id).FirstOrDefault();
            Mitem = new vwForumItem
            {
                id = fitem.id,
                seq = 1,
                billDate = fitem.billDate,
                pid = fitem.pid,
                subject = fitem.subject,
                smemo = fitem.smemo,
                workNo = fitem.workNo,
                othersA1 = fitem.othersA1,
                othersA2 = fitem.othersA2,
                othersA3 = fitem.othersA3,
                othersA4 = fitem.othersA4,
                othersA5 = fitem.othersA5,
                othersA6 = fitem.othersA6,
                othersA7 = fitem.othersA7,
                othersA8 = fitem.othersA8,
                othersA9 = fitem.othersA9,
                othersA10 = fitem.othersA10,
                othersB1 = fitem.othersB1,
                othersB2 = fitem.othersB2,
                othersB3 = fitem.othersB3,
                othersB4 = fitem.othersB4,
                othersB5 = fitem.othersB5,
                othersB6 = fitem.othersB6,
                othersB7 = fitem.othersB7,
                othersB8 = fitem.othersB8,
                othersB9 = fitem.othersB9,
                othersB10 = fitem.othersB10,
                prjId = fitem.prjId,
                sfile = fitem.sfile,
                sfile2 = fitem.sfile2,
                sfile3 = fitem.sfile3,
                sfile4 = fitem.sfile4,
                isPrivate = fitem.isPrivate
            };
            SubItemList = new List<vwForumItem>();

            var orgList = ctx.prjForumItems.Where(x => x.pid == Mitem.id).OrderBy(x => x.billDate).ToList<ForumItem>();

            foreach (var f in orgList)
            {
                vwForumItem fItem = new vwForumItem
                {
                    id = f.id,
                    prjId = f.prjId,
                    workNo = f.workNo,
                    billDate = f.billDate,
                    subject = f.subject,
                    subTitle = f.subTitle,
                    smemo = f.smemo,
                    othersA1 = f.othersA1,
                    othersA2 = f.othersA2,
                    othersA3 = f.othersA3,
                    othersA4 = f.othersA4,
                    othersA5 = f.othersA5,
                    othersA6 = f.othersA6,
                    othersA7 = f.othersA7,
                    othersA8 = f.othersA8,
                    othersA9 = f.othersA9,
                    othersA10 = f.othersA10,
                    othersB1 = f.othersB1,
                    othersB2 = f.othersB2,
                    othersB3 = f.othersB3,
                    othersB4 = f.othersB4,
                    othersB5 = f.othersB5,
                    othersB6 = f.othersB6,
                    othersB7 = f.othersB7,
                    othersB8 = f.othersB8,
                    othersB9 = f.othersB9,
                    othersB10 = f.othersB10,
                    seq = f.seq,
                    sfile = f.sfile,
                    sfile2 = f.sfile2,
                    sfile3 = f.sfile3,
                    sfile4 = f.sfile4,
                    isPrivate = f.isPrivate
                    
                };
                SubItemList.Add(fItem);
            }
            //int i = 2;
            //foreach (var item in SubItemList)
            //{
            //    item.seq = i;
            //    i++;
            //}

            this.prjCodeObj= ctx.prjCodeList.Where(x => x.code == code).FirstOrDefault();
        }
        public string currentUser { get; set; }
        public string code { get; set; }
        public string prjId { get; set; }
        public vwForumItem Mitem { get; set; }
        public List<vwForumItem> SubItemList { get; set; }
        public prjCode prjCodeObj { get; set; }
    }
}