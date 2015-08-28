using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using Framework;
using FISCA.Presentation;


namespace SchoolCore
{
    public static class Program
    {
        /// <summary>
        /// 应用程序的主要进入点。
        /// </summary>
        [ApplicationMain()]
        public static void Main()
        {
            Framework.Program.Initial();
            //学校组态基本上就是储存在 App.Configuration 之中。

            Class.Instance.SyncAllBackground();
            Class.Instance.WaitLoadingComplete();
            Student.Instance.SyncAllBackground();
            Student.Instance.WaitLoadingComplete();
            Teacher.Instance.SyncAllBackground();
            Teacher.Instance.WaitLoadingComplete();
            //Course.Instance.SyncAllBackground();
            //Course.Instance.WaitLoadingComplete();

            Student.Instance.SetupPresentation();
            Class.Instance.SetupPresentation();
            Teacher.Instance.SetupPresentation();
            //Course.Instance.SetupPresentation(); 

            //设定 ASPOSE 组件的 License。
            System.IO.Stream stream = new System.IO.MemoryStream(Properties.Resources.Aspose_Total);
            
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Words.License().SetLicense(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Cells.License().SetLicense(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.BarCode.License().SetLicense(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Pdf.License().SetLicense(stream);

            FISCA.LogAgent.ApplicationLog.Log("[特殊历程]", "登入", string.Format("用户{0}已登入系统", FISCA.Authentication.DSAServices.UserAccount));

            // 变更用户密码
            FISCA.Presentation.MotherForm.StartMenu["安全性"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["安全性"].Image = Properties.Resources.foreign_key_lock_64;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["变更密码"].Enable = User.Acl["StartButton0004"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["变更密码"].Click += delegate
            {
                UserInfoManager uim = new UserInfoManager();
                uim.ShowDialog();
            };

            // 管理学校基本数据
            FISCA.Presentation.MotherForm.StartMenu["管理学校基本数据"].Image = Properties.Resources.school_fav_64;
            FISCA.Presentation.MotherForm.StartMenu["管理学校基本数据"].Enable = User.Acl["StartButton0003"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["管理学校基本数据"].Click += delegate
            {
                SchoolInfoMangement sim = new SchoolInfoMangement();
                sim.ShowDialog();
            };

            Framework.Security.RoleAclSource.Instance["系统"].Add(new Framework.Security.RibbonFeature("StartButton0003", "管理学校基本数据"));
            Framework.Security.RoleAclSource.Instance["系统"].Add(new Framework.Security.RibbonFeature("StartButton0004", "变更密码"));

            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Image = Properties.Resources.world_upload_64;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Click += new EventHandler(Restart_Click);

            //设定画面选取Count
            SelectedListChanged();
        }

        private static void Restart_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// 设定画面选取Count
        /// </summary>
        private static void SelectedListChanged()
        {
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已选取" + K12.Presentation.NLDPanels.Student.SelectedSource.Count + "名学生");
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已选取" + K12.Presentation.NLDPanels.Class.SelectedSource.Count + "个班级");
            };

            K12.Presentation.NLDPanels.Teacher.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已选取" + K12.Presentation.NLDPanels.Teacher.SelectedSource.Count + "名教师");
            };

            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已选取" + K12.Presentation.NLDPanels.Course.SelectedSource.Count + "个课程");
            };

        }

    }
}

