using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using SchoolCore.Editor;

namespace SchoolCore.StudentExtendControls.Ribbon
{
    public partial class AddStudent : BaseForm
    {
        public AddStudent()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;
            K12.Data.StudentRecord studRec = new K12.Data.StudentRecord();
            studRec.Name = txtName.Text;
            string StudentID = K12.Data.Student.Insert(studRec);
            PermRecLogProcess prlp = new PermRecLogProcess();
            if (chkInputData.Checked == true)
            {
                if (StudentID != "")
                {
                    Student.Instance.PopupDetailPane(StudentID);
                    Student.Instance.SyncDataBackground(StudentID);
                }
            }
            Student.Instance.SyncDataBackground(StudentID);

            prlp.SaveLog("學籍.學生", "新增學生", "新增學生姓名:" + txtName.Text);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
