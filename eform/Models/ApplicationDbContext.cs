﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using eform.Migrations;

namespace eform.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

            //Database.SetInitializer(new CustomMigration.eFormDBInitializer());

            Database.SetInitializer<ApplicationDbContext>(null);

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<dep> deps { get; set; }
        public DbSet<jobPo> jobPos { get; set; }
        public DbSet<FlowDefMain> FlowDefMainList { get; set; }
        public DbSet<FlowDefSub> FlowDefSubList { get; set; }
        public DbSet<FlowMain> FlowMainList { get; set; }
        public DbSet<FlowSub> FlowSubList { get; set; }
        public DbSet<news> newsList { get; set; }
        public DbSet<newsType> newsTypeList { get; set; }
        public DbSet<ReqOverTime> reqOverTimeList { get; set; }
        public DbSet<permission> permList { get; set; }
        public DbSet<permMod> permModList { get; set; }
        public DbSet<salary> salaryList { get; set; }

        public DbSet<dayOffType> dayOffTypeList { get; set; }
        public DbSet<dayOff> dayOffList { get; set; }
        public DbSet<dayOffSum> dayOffSumList { get; set; }

        public DbSet<publicOut> publicOutList { get; set; }
        public DbSet<guestForm> guestFormList { get; set; }

        public DbSet<ReqInHouse> reqInHouseList { get; set; }

        //stock begin
        public DbSet<stockItemInit> stockItemInitList { get; set; }
        //stock end

        public DbSet<EventSchedule> eventScheduleList { get; set; }
        public DbSet<prjCode> prjCodeList { get; set; }
        public DbSet<prjWork> prjWorkList { get; set; }
        public DbSet<prjWorkItem> prjWorkItemList { get; set; }
        public DbSet<prjPM> prjPMList { get; set; }
        public DbSet<prjForumUser> prjForumUserList { get; set; }
        public DbSet<PrjEvent> prjEventList { get; set; }
        public DbSet<ForumItem> prjForumItems { get; set; }

        //Schedule
        public DbSet<schTemp> schTempList { get; set; }
        public DbSet<schTempItem> schTempItemList { get; set; }

        public DbSet<scheduleItem> scheduleItems{ get; set; }

        public List<vwSignType> signTypeList()
        {
            List<vwSignType> list = new List<vwSignType>();
            list.Add(new vwSignType
            {
                id = 1,
                nm = "會簽"
            });
            list.Add(new vwSignType
            {
                id = 2,
                nm = "會簽-全部同意"
            });
            list.Add(new vwSignType
            {
                id = 3,
                nm = "直接許可"
            });
            list.Add(new vwSignType
            {
                id = 4,
                nm = "串簽"
            });
            list.Add(new vwSignType
            {
                id = 701,
                nm = "人事登錄"
            });
            return list;
        }
        public List<vwFlowStatus> flowStatusList()
        {
            List<vwFlowStatus> list = new List<vwFlowStatus>();

            list.Add(new vwFlowStatus
            {
                id = 1,
                nm = "簽核中"
            });

            list.Add(new vwFlowStatus
            {
                id = 2,
                nm = "核准"
            });

            list.Add(new vwFlowStatus
            {
                id = 3,
                nm = "退回"
            });

            return list;
        }

        public List<vwSignResult> signResultList()
        {
            List<vwSignResult> list = new List<vwSignResult>();
            list.Add(new vwSignResult
            {
                id = 1,
                nm = "同意"
            });
            list.Add(new vwSignResult
            {
                id = 2,
                nm = "不同意"
            });
            return list;
        }

        public DateTime getLocalTiime()
        {
            return DateTime.UtcNow.AddSeconds(28800);
        }

        public string getParentDeps(string depNo, ApplicationDbContext ctx)
        {
            string r = "";
            dep depObj = ctx.deps.Where(x => x.depNo == depNo).First();

            if (depObj != null)
            {
                r = depObj.depNm;
                if (depNo != "001")
                {
                    if (depObj.depLevel > 1)
                    {
                        r = getParentDeps(depObj.parentDepNo, ctx) + "-" + r;
                    }
                    else if (depObj.parentDepNo != "001")
                    {
                        r = depObj.depNm + "-" + r;
                    }
                }
            }

            return r;
        }

        public vwEmployee getCurrentUser(string Name)
        {
            ApplicationUser user = this.Users.Where(x => x.UserName == Name).FirstOrDefault();
            vwEmployee employee = new vwEmployee
            {
                Id = user.Id,
                UserCName = user.cName,
                UserEName = user.eName,
                workNo = user.workNo,
                Email = user.Email,
                beginWorkDate = user.beginWorkDate
            };
            return employee;
        }
        public vwEmployee getUserByWorkNo(string WorkNo)
        {
            ApplicationUser user = this.Users.Where(x => x.workNo == WorkNo).FirstOrDefault();
            vwEmployee employee = new vwEmployee
            {
                Id = user.Id,
                UserCName = user.cName,
                UserEName = user.eName,
                workNo = user.workNo,
                Email = user.Email,
                beginWorkDate = user.beginWorkDate
            };
            return employee;
        }
        public string getUserList()
        {
            var userlist0 = this.Users.Where(x => x.status == 1 && x.UserName.ToLower().Contains("admin") == false).OrderBy(x => x.UserName).ToList();
            List<dynamic> userlist = new List<dynamic>();
            foreach (var user in userlist0)
            {
                dynamic obj = new ExpandoObject();
                obj.id = user.workNo;
                obj.text = user.eName;
                userlist.Add(obj);
            }
            dynamic userSelTitle = new ExpandoObject();
            userSelTitle.id = ""; userSelTitle.text = "請選擇人員";
            userlist.Insert(0, userSelTitle);

            return JsonConvert.SerializeObject(userlist);
        }

        public string getDepList(int level=3)
        {
            var depList0 = this.deps.Where(x => x.depLevel == level).OrderBy(x => x.depNm).ToList();
            List<dynamic> depList = new List<dynamic>();
            foreach (var dep in depList0)
            {
                dynamic obj = new ExpandoObject();
                obj.id = dep.depNo;
                obj.text = dep.depNm;
                depList.Add(obj);
            }
            dynamic depSelTitle = new ExpandoObject();
            depSelTitle.id = ""; depSelTitle.text = "請選擇部門";
            depList.Insert(0, depSelTitle);

            return JsonConvert.SerializeObject(depList);
        }

        public string getDepListP020A1(int level = 3)
        {
            var depList0 = this.deps.Where(x => x.depLevel == level && (x.depNm.Contains("9200-2") || x.depNm.Contains("9200-4"))).OrderBy(x => x.depNm).ToList();
            List<dynamic> depList = new List<dynamic>();
            foreach (var dep in depList0)
            {
                dynamic obj = new ExpandoObject();
                obj.id = dep.depNo;
                obj.text = dep.depNm;
                depList.Add(obj);
            }
            dynamic depSelTitle = new ExpandoObject();
            depSelTitle.id = ""; depSelTitle.text = "請選擇部門";
            depList.Insert(0, depSelTitle);

            return JsonConvert.SerializeObject(depList);
        }

        public string genBillNo(string formType)
        {
            string r = "";
            string code = "";
            if (formType == "OverTime")
            {
                code = "P016A1";
            }
            if (formType == "RealOverTime")
            {
                code = "P017A1";
            }
            if (formType == "DayOff")
            {
                code = "P018A1";
            }
            if (formType == "PublicOut")
            {
                code = "P019A1";
            }
            if (formType == "GuestForm")
            {
                code = "P011A1";
            }
            if (formType == "ReqInHouse")
            {
                code = "P020A1";
            }
            if (formType == "EventSchedule")
            {
                code = "B001A1";
            }
            string preCode = code + "-" + getLocalTiime().ToString("yyMM");
            var list = from fmain in this.FlowMainList.Where(x => x.billNo != null && x.billNo.Contains(preCode))
                       select fmain.billNo;
            list = list.Where(x => string.IsNullOrEmpty(x) == false);

            if (list.Count() == 0)
            {
                r = preCode + "0001";
            }
            else
            {
                List<int> vList = new List<int>();
                foreach (string _v in list)
                {
                    if (_v.Length > 4)
                    {
                        vList.Add(Convert.ToInt32(_v.Substring(_v.Length - 4, 4)));
                    }
                }
                r = preCode + String.Format("{0:0000}", vList.Max() + 1);
            }

            return r;
        }

        public System.Data.Entity.DbSet<eform.Models.vwScheduleItem> vwScheduleItems { get; set; }
    }
}