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
        /// 應用程式的主要進入點。
        /// </summary>
        [ApplicationMain()]
        public static void Main()
        {
            Framework.Program.Initial();
            //學校組態基本上就是儲存在 App.Configuration 之中。
            
            Class.Instance.SyncAllBackground();
            Class.Instance.WaitLoadingComplete();
            Student.Instance.SyncAllBackground();
            Student.Instance.WaitLoadingComplete();
            Teacher.Instance.SyncAllBackground();
            Teacher.Instance.WaitLoadingComplete();
            Course.Instance.SyncAllBackground();
            Course.Instance.WaitLoadingComplete();

            Student.Instance.SetupPresentation();
            Class.Instance.SetupPresentation();
            Teacher.Instance.SetupPresentation();
            Course.Instance.SetupPresentation(); 

            //設定 ASPOSE 元件的 License。
            System.IO.Stream stream = new System.IO.MemoryStream(Properties.Resources.Aspose_Total);

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Cells.License().SetLicense(stream);
            

            FISCA.LogAgent.ApplicationLog.Log("[特殊歷程]", "登入", string.Format("使用者{0}已登入系統", FISCA.Authentication.DSAServices.UserAccount));

            // 變更使用者密碼
            FISCA.Presentation.MotherForm.StartMenu["安全性"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["安全性"].Image = Properties.Resources.foreign_key_lock_64;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["變更密碼"].Enable = User.Acl["StartButton0004"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["變更密碼"].Click += delegate
            {
                UserInfoManager uim = new UserInfoManager();
                uim.ShowDialog();
            };

            // 管理學校基本資料
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Image = Properties.Resources.school_fav_64;
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Enable = User.Acl["StartButton0003"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Click += delegate
            {
                SchoolInfoMangement sim = new SchoolInfoMangement();
                sim.ShowDialog();
            };

            Framework.Security.RoleAclSource.Instance["系統"].Add(new Framework.Security.RibbonFeature("StartButton0003", "管理學校基本資料"));
            Framework.Security.RoleAclSource.Instance["系統"].Add(new Framework.Security.RibbonFeature("StartButton0004", "變更密碼"));

            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Image = Properties.Resources.world_upload_64;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Click += new EventHandler(Restart_Click);

            //設定畫面選取Count
            SelectedListChanged();
        }

        private static void Restart_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// 設定畫面選取Count
        /// </summary>
        private static void SelectedListChanged()
        {
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Student.SelectedSource.Count + "名學生");
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Class.SelectedSource.Count + "個班級");
            };

            K12.Presentation.NLDPanels.Teacher.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Teacher.SelectedSource.Count + "名教師");
            };

            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Course.SelectedSource.Count + "個課程");
            };

        }

    }
}
