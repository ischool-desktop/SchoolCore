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

namespace SchoolCore.ClassExtendControls.Ribbon
{
    public partial class AddClass : BaseForm
    {
        public AddClass()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool chkHasClassName = false;
            if (txtName.Text.Trim() == "")
                return;

            List<K12.Data.ClassRecord> AllClassRecs = K12.Data.Class.SelectAll();
            foreach (K12.Data.ClassRecord cr in AllClassRecs)
                if (cr.Name == txtName.Text)
                {
                    MessageBox.Show("班级名称重复");
                    return;
                }

            PermRecLogProcess prlp = new PermRecLogProcess();
            K12.Data.ClassRecord classRec = new K12.Data.ClassRecord();
            classRec.Name = txtName.Text;
            string ClassID = K12.Data.Class.Insert(classRec);

            Class.Instance.SyncDataBackground(ClassID);

            if (chkInputData.Checked == true)
            {
                Class.Instance.PopupDetailPane(ClassID);
                Class.Instance.SyncDataBackground(ClassID);
            }

            prlp.SaveLog("学籍.班级", "新增班级", "新增班级,名称:" + txtName.Text);
            this.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

