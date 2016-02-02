using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentExtension_CN
{
    public class Program
    {
        [ApplicationMain()]
        static public void Main()
        {            
            // 班級資訊
           SchoolCore.Student.Instance.AddDetailBulider(new DetailBulider<StudentExtension_CN.ClassItem>());

            // 家長資訊
           SchoolCore.Student.Instance.AddDetailBulider(new DetailBulider<StudentExtension_CN.ParentInfoItem>());

            // 權限註冊         
           Catalog catalog1 = RoleAclSource.Instance["学生"]["数据项"];
           catalog1.Add(new DetailItemFeature("JHSchool.Student.Detail0090", "班級信息"));
           catalog1.Add(new DetailItemFeature("StudentExtension_CN_ParentInfoItem", "家长信息"));


           RibbonBarItem rbItem2 = MotherForm.RibbonBarItems["学生", "资料统计"];
           rbItem2["汇出"]["汇出学生基本资料"].Enable = UserAcl.Current["StudentExtension_CN_ExportStudentData"].Executable;
           rbItem2["汇出"]["汇出学生基本资料"].Click += delegate
           {
               if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
               {
                   ExportStudentData esd = new ExportStudentData(K12.Presentation.NLDPanels.Student.SelectedSource);
                   esd.Start();
               }
               else
               {
                   FISCA.Presentation.Controls.MsgBox.Show("请选择学生");
                   return;
               }

           };


           // 学生基本资料
           Catalog catalog1b = RoleAclSource.Instance["学生"]["功能按钮"];
           catalog1b.Add(new RibbonFeature("StudentExtension_CN_ExportStudentData", "汇出学生基本资料"));


        }
    }
}
