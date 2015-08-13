using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace SchoolCore.TeacherExtendControls.Ribbon
{
    public partial class AddTeacher : BaseForm
    {
        public AddTeacher()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;

            // 检查教师名称，验证方式，姓名+昵称 不能重复。            
            List<K12.Data.TeacherRecord> TRecs = K12.Data.Teacher.SelectAll();
            Dictionary<string, K12.Data.TeacherRecord> checkStr = new Dictionary<string, K12.Data.TeacherRecord>();
            foreach (K12.Data.TeacherRecord TRec in TRecs)
                checkStr.Add(TRec.Name + TRec.Nickname, TRec);

            string strName = txtName.Text + txtNickName.Text;

            if (checkStr.ContainsKey(strName))
            {
                if (checkStr[strName].Status == K12.Data.TeacherRecord.TeacherStatus.一般)
                {
                    MsgBox.Show("教师姓名:" + txtName.Text + ",已存在系统内,如果要使用相同姓名请加昵称.");
                    return;
                }

                // 当删除状态，修正删除教师内的昵称 与 TeacherID
                if (checkStr[strName].Status == K12.Data.TeacherRecord.TeacherStatus.刪除)
                {
                    K12.Data.TeacherRecord delRec = checkStr[strName];
                    delRec.Nickname = delRec.ID;
                    K12.Data.Teacher.Update(delRec);
                }
            }

            K12.Data.TeacherRecord teacherRec = new K12.Data.TeacherRecord();
            teacherRec.Name = txtName.Text;
            teacherRec.Nickname = txtNickName.Text;

            string TeacherID = K12.Data.Teacher.Insert(teacherRec);

            Teacher.Instance.SyncDataBackground(TeacherID);

            if (chkInputData.Checked == true)
            {
                if (TeacherID != "")
                {
                    Teacher.Instance.PopupDetailPane(TeacherID);
                    Teacher.Instance.SyncDataBackground(TeacherID);
                }
            }
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("学籍.教师", "新增教师", "新增教师,姓名:" + txtName.Text + ",昵称:" + txtNickName.Text);

            this.Close();
        }
    }
}

