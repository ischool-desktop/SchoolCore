namespace SchoolCore.CourseExtendControls
{
    partial class BasicInfoItem
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                //Teacher.Instance.TeacherDataChanged -= new EventHandler<TeacherDataChangedEventArgs>(Instance_TeacherDataChanged);
                //Teacher.Instance.TeacherInserted -= new EventHandler(Instance_TeacherInserted);
                //Teacher.Instance.TeacherDeleted -= new EventHandler<TeacherDeletedEventArgs>(Instance_TeacherDeleted);
                //ClassEntity.Instance.ClassInserted-= new EventHandler<InsertClassEventArgs>(Instance_ClassInserted);
                //ClassEntity.Instance.ClassUpdated -= new EventHandler<UpdateClassEventArgs>(Instance_ClassUpdated);
                //ClassEntity.Instance.ClassDeleted -= new EventHandler<DeleteClassEventArgs>(Instance_ClassDeleted);
                //CourseEntity.Instance.ForeignTableChanged -= new EventHandler(Instance_ForeignTableChanged);
                //CourseEntity.Instance.CourseChanged -= new EventHandler<CourseChangeEventArgs>(Instance_CourseChanged);
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtCourseName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtSubject = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.cboClass = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.txtPeriodCredit = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnTeachers = new DevComponents.DotNetBar.ButtonX();
            this.btnTeacher1 = new DevComponents.DotNetBar.ButtonItem();
            this.btnTeacher2 = new DevComponents.DotNetBar.ButtonItem();
            this.btnTeacher3 = new DevComponents.DotNetBar.ButtonItem();
            this.cboMultiTeacher = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(36, 20);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(70, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "課程名稱";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // txtCourseName
            // 
            // 
            // 
            // 
            this.txtCourseName.Border.Class = "TextBoxBorder";
            this.txtCourseName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtCourseName.Location = new System.Drawing.Point(113, 19);
            this.txtCourseName.MaxLength = 50;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Size = new System.Drawing.Size(151, 25);
            this.txtCourseName.TabIndex = 1;
            // 
            // txtSubject
            // 
            // 
            // 
            // 
            this.txtSubject.Border.Class = "TextBoxBorder";
            this.txtSubject.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSubject.Location = new System.Drawing.Point(113, 50);
            this.txtSubject.MaxLength = 50;
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(151, 25);
            this.txtSubject.TabIndex = 3;
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(36, 51);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(70, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "科目名稱";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(290, 20);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(70, 23);
            this.labelX4.TabIndex = 12;
            this.labelX4.Text = "所屬班級";
            this.labelX4.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX5
            // 
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(290, 51);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(70, 23);
            this.labelX5.TabIndex = 14;
            this.labelX5.Text = "學 年 度";
            this.labelX5.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX6
            // 
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(290, 82);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(70, 23);
            this.labelX6.TabIndex = 16;
            this.labelX6.Text = "學         期";
            this.labelX6.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX7
            // 
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.Class = "";
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Location = new System.Drawing.Point(36, 82);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(70, 23);
            this.labelX7.TabIndex = 18;
            this.labelX7.Text = "節數/權數";
            this.labelX7.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboClass
            // 
            this.cboClass.DisplayMember = "Text";
            this.cboClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboClass.FormattingEnabled = true;
            this.cboClass.ItemHeight = 19;
            this.cboClass.Location = new System.Drawing.Point(364, 19);
            this.cboClass.MaxDropDownItems = 6;
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(151, 25);
            this.cboClass.TabIndex = 13;
            this.cboClass.Tag = "ForceValidate";
            // 
            // txtPeriodCredit
            // 
            // 
            // 
            // 
            this.txtPeriodCredit.Border.Class = "TextBoxBorder";
            this.txtPeriodCredit.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPeriodCredit.Location = new System.Drawing.Point(113, 81);
            this.txtPeriodCredit.Name = "txtPeriodCredit";
            this.txtPeriodCredit.Size = new System.Drawing.Size(151, 25);
            this.txtPeriodCredit.TabIndex = 19;
            // 
            // btnTeachers
            // 
            this.btnTeachers.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnTeachers.AutoExpandOnClick = true;
            this.btnTeachers.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnTeachers.Location = new System.Drawing.Point(271, 113);
            this.btnTeachers.Margin = new System.Windows.Forms.Padding(4);
            this.btnTeachers.Name = "btnTeachers";
            this.btnTeachers.Size = new System.Drawing.Size(89, 22);
            this.btnTeachers.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnTeacher1,
            this.btnTeacher2,
            this.btnTeacher3});
            this.btnTeachers.TabIndex = 20;
            // 
            // btnTeacher1
            // 
            this.btnTeacher1.GlobalItem = false;
            this.btnTeacher1.Name = "btnTeacher1";
            this.btnTeacher1.Text = "教師一";
            // 
            // btnTeacher2
            // 
            this.btnTeacher2.GlobalItem = false;
            this.btnTeacher2.Name = "btnTeacher2";
            this.btnTeacher2.Text = "教師二";
            // 
            // btnTeacher3
            // 
            this.btnTeacher3.GlobalItem = false;
            this.btnTeacher3.Name = "btnTeacher3";
            this.btnTeacher3.Text = "教師三";
            // 
            // cboMultiTeacher
            // 
            this.cboMultiTeacher.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cboMultiTeacher.DisplayMember = "Text";
            this.cboMultiTeacher.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboMultiTeacher.FormattingEnabled = true;
            this.cboMultiTeacher.ItemHeight = 19;
            this.cboMultiTeacher.Location = new System.Drawing.Point(364, 112);
            this.cboMultiTeacher.MaxDropDownItems = 6;
            this.cboMultiTeacher.Name = "cboMultiTeacher";
            this.cboMultiTeacher.Size = new System.Drawing.Size(151, 25);
            this.cboMultiTeacher.TabIndex = 21;
            this.cboMultiTeacher.Tag = "ForceValidate";
            // 
            // cbxSchoolYear
            // 
            this.cbxSchoolYear.DisplayMember = "Text";
            this.cbxSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSchoolYear.FormattingEnabled = true;
            this.cbxSchoolYear.ItemHeight = 19;
            this.cbxSchoolYear.Location = new System.Drawing.Point(364, 50);
            this.cbxSchoolYear.Name = "cbxSchoolYear";
            this.cbxSchoolYear.Size = new System.Drawing.Size(151, 25);
            this.cbxSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxSchoolYear.TabIndex = 22;
            // 
            // cbxSemester
            // 
            this.cbxSemester.DisplayMember = "Text";
            this.cbxSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSemester.FormattingEnabled = true;
            this.cbxSemester.ItemHeight = 19;
            this.cbxSemester.Location = new System.Drawing.Point(364, 81);
            this.cbxSemester.Name = "cbxSemester";
            this.cbxSemester.Size = new System.Drawing.Size(151, 25);
            this.cbxSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxSemester.TabIndex = 23;
            // 
            // BasicInfoItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cbxSemester);
            this.Controls.Add(this.cbxSchoolYear);
            this.Controls.Add(this.btnTeachers);
            this.Controls.Add(this.cboMultiTeacher);
            this.Controls.Add(this.cboClass);
            this.Controls.Add(this.labelX7);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtCourseName);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtPeriodCredit);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(550, 0);
            this.Name = "BasicInfoItem";
            this.Size = new System.Drawing.Size(550, 155);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtCourseName;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtSubject;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.LabelX labelX7;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtPeriodCredit;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboClass;
        private DevComponents.DotNetBar.ButtonX btnTeachers;
        private DevComponents.DotNetBar.ButtonItem btnTeacher1;
        private DevComponents.DotNetBar.ButtonItem btnTeacher2;
        private DevComponents.DotNetBar.ButtonItem btnTeacher3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboMultiTeacher;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSemester;

    }
}
