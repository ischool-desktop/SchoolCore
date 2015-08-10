﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SchoolCore.InternalExtendControls.Tagging;
using K12.Data;

namespace SchoolCore.TeacherExtendControls.Ribbon
{
    internal partial class TeacherTagForm : TagForm
    {
        public TeacherTagForm()
        {
            InitializeComponent();
        }

        protected override K12.Data.TagCategory Category
        {
            get
            {
                return K12.Data.TagCategory.Teacher;
            }
        }

        protected override void DoDelete(TagConfigRecord record)
        {
            int use_count = 0;

            foreach (K12.Data.TeacherTagRecord eachTeacher in K12.Data.TeacherTag.SelectAll())
            {                
                if (eachTeacher.RefTagID == record.ID)
                    use_count++;
            }




            string msg;
            if (use_count > 0)
                msg = string.Format("目前有「{0}」個教師使用此類別，您確定要刪除此類別嗎？", use_count);
            else
                msg = "您確定要刪除此類別嗎？";

            if (FISCA.Presentation.Controls.MsgBox.Show(msg, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PermRecLogProcess prlp = new PermRecLogProcess();
                prlp.SaveLog("學籍.類別管理", "類別管理刪除類別", "刪除 " + record.Category + " 類別,名稱:" + record.FullName);

                TagConfig.Delete(record);
            }
        }
    }
}
