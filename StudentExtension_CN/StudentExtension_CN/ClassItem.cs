using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;
using SchoolCore;


namespace StudentExtension_CN
{
    [FCode("JHSchool.Student.Detail0090", "班级信息")]
    public partial class ClassItem : FISCA.Presentation.DetailContent
    {
        private ChangeListener DataListener { get; set; }
        private string _DefaultGradeYear;
        private string _DefaultClassName;
        private string _DefaultSeatNo;
        private string _DefaultStudNum;
        private EnhancedErrorProvider Errors { get; set; }
        private Dictionary<string, string> _ClassNameIDDic;
        private K12.Data.StudentRecord objStudent;
        private List<K12.Data.ClassRecord> _AllClassRecs;
        private List<int> _ClassSeatNoList;
        private bool isBwBusy = false;
        private BackgroundWorker BGWork;
        private List<K12.Data.StudentRecord> _AllStudRecList;
        private List<K12.Data.StudentRecord> _studRecList;
        private string tmpClassName = "";
        SchoolCore.PermRecLogProcess prlp;

        public ClassItem()
        {
            InitializeComponent();

            Group = "班级信息";
        }

        void ClassItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Student.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);
        }

        void JHStudent_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHStudent_AfterChange), sender, e);
            }
            else
            {
                if (this.PrimaryKey != "")
                {
                    if (!BGWork.IsBusy)
                        BGWork.RunWorkerAsync();
                }
            }
        }

        void BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBwBusy)
            {
                isBwBusy = false;
                BGWork.RunWorkerAsync();
                return;
            }
            reloadChkdData();
        }

        void BGWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _AllClassRecs.Clear();
            _AllClassRecs = K12.Data.Class.SelectAll();
            _AllStudRecList.Clear();
            _AllStudRecList = K12.Data.Student.SelectAll();
            _studRecList.Clear();

            // 有条件加入一般状态学生与有班级座号学生
            foreach (K12.Data.StudentRecord studRec in _AllStudRecList)
                if (studRec.Class != null)
                    if (studRec.Status == K12.Data.StudentRecord.StudentStatus.一般 && studRec.SeatNo.HasValue)
                        _studRecList.Add(studRec);

            objStudent = K12.Data.Student.SelectByID(PrimaryKey);
        }

        private void ValueManager_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        private void reloadChkdData()
        {
            DataListener.SuspendListen();

            if (objStudent.Class != null)
            {
                cboClass.Text = objStudent.Class.Name;
                cboClass.Items.Add(objStudent.Class.Name);
                this._DefaultClassName = objStudent.Class.Name;
            }
            else
                cboClass.Text = string.Empty;


            cboSeatNo.Text = string.Empty;
            this._DefaultSeatNo = string.Empty;

            // 当有座号
            if (objStudent.SeatNo.HasValue)
                if (objStudent.SeatNo.Value > 0)
                {
                    string strSeatNo = objStudent.SeatNo.Value + "";
                    cboSeatNo.Text = strSeatNo;
                    cboSeatNo.Items.Add(strSeatNo);
                    this._DefaultSeatNo = strSeatNo;
                }


            // 当有学号
            if (string.IsNullOrEmpty(objStudent.StudentNumber))
            {
                this._DefaultStudNum = string.Empty;
                txtStudentNumber.Text = string.Empty;
            }
            else
            {
                txtStudentNumber.Text = objStudent.StudentNumber;
                this._DefaultStudNum = objStudent.StudentNumber;
            }

            prlp.SetBeforeSaveText("班级", cboClass.Text);
            prlp.SetBeforeSaveText("座号", cboSeatNo.Text);
            prlp.SetBeforeSaveText("学号", txtStudentNumber.Text);

            tmpClassName = cboClass.Text;
            setClassName();
            setClassNo();

            DataListener.Reset();
            DataListener.ResumeListen();
            this.Loading = false;
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            cboClass.Items.Clear();
            cboSeatNo.Items.Clear();
            cboClass.Text = "";
            cboSeatNo.Text = "";
            txtStudentNumber.Text = "";
            this.Loading = true;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            if (BGWork.IsBusy)
                isBwBusy = true;
            else
                BGWork.RunWorkerAsync();

        }
        #region IDetailBulider 成员

        public DetailContent GetContent()
        {
            return new ClassItem();
        }

        #endregion


        // 设定班级
        private void setClassName()
        {
            cboClass.Items.Clear();

            cboClass.Items.Add("<空白>");
            _ClassNameIDDic.Clear();
            List<string> ClassNameItems = new List<string>();
            foreach (K12.Data.ClassRecord cr in _AllClassRecs)
            {
                ClassNameItems.Add(cr.Name);
                _ClassNameIDDic.Add(cr.Name, cr.ID);
            }
            ClassNameItems.Sort();
            cboClass.Items.AddRange(ClassNameItems.ToArray());

            cboClass.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboClass.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (cboClass.Items.Count < 2)
                cboClass.Items.Clear();
            Errors.Clear();
        }

        // 设定座号
        private void setClassNo()
        {
            _ClassSeatNoList.Clear();

            foreach (K12.Data.StudentRecord studRec in _studRecList)
                if (cboClass.Text == studRec.Class.Name)
                    _ClassSeatNoList.Add(studRec.SeatNo.Value);

            _ClassSeatNoList.Sort();

            cboSeatNo.Items.Clear();

            if (_ClassSeatNoList.Count > 0)
            {
                int maxSeatNo = _ClassSeatNoList[_ClassSeatNoList.Count - 1];
                for (int i = 1; i <= maxSeatNo; i++)
                {
                    if (!_ClassSeatNoList.Contains(i))
                    {
                        string strSeatNo = i + "";
                        if (!cboSeatNo.Items.Contains(strSeatNo))
                            cboSeatNo.Items.Add(strSeatNo);
                    }
                }

                cboSeatNo.Items.Add((maxSeatNo + 1) + "");
            }

            if (cboSeatNo.Items.Count == 0 && cboClass.Items.Contains(cboClass.Text))
                if (!cboSeatNo.Items.Contains("1"))
                    cboSeatNo.Items.Add("1");
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            Errors.Clear();

            objStudent = K12.Data.Student.SelectByID(PrimaryKey);

            if (chkClassName() == false)
            {
                Errors.SetError(cboClass, "班级名称错误!");
                return;
            }

            int tmpSeatNo = 0;
            int.TryParse(cboSeatNo.Text, out tmpSeatNo);

            if (tmpSeatNo < 1 && cboSeatNo.Text.Trim() != "")
            {
                Errors.SetError(cboSeatNo, "学生座号错误!");
                return;
            }

            // 检查班级座号是否重复
            if (tmpSeatNo > 0)
                if (_ClassSeatNoList.Contains(tmpSeatNo))
                {
                    // 是否是自己原本座号
                    if (this._DefaultSeatNo != cboSeatNo.Text.Trim())
                    {
                        Errors.SetError(cboSeatNo, "座号重复!");
                        return;
                    }
                }

            // 更改学生班级
            if (_ClassNameIDDic.ContainsKey(cboClass.Text))
            {
                objStudent.RefClassID = _ClassNameIDDic[cboClass.Text];
            }

            // 当选空白时
            if ((cboClass.Text == "" && cboSeatNo.Text == "") || cboClass.Text == "<空白>")
            {
                objStudent.RefClassID = null;
            }

            // 检查学号是否重复
            if (txtStudentNumber.Text != this._DefaultStudNum)
            {
                // 判断是否是空
                if (string.IsNullOrEmpty(txtStudentNumber.Text))
                    objStudent.StudentNumber = "";
                else
                {
                    // 取得目前学生状态
                    K12.Data.StudentRecord.StudentStatus studtStatus = K12.Data.Student.SelectByID(PrimaryKey).Status;

                    List<string> checkData = new List<string>();
                    // 同状态下学号不能重复
                    foreach (K12.Data.StudentRecord studRec in K12.Data.Student.SelectAll())
                        if (studRec.Status == studtStatus)
                            checkData.Add(studRec.StudentNumber);

                    if (checkData.Contains(txtStudentNumber.Text))
                    {
                        Errors.SetError(txtStudentNumber, "学号重复!");
                        return;
                    }
                    objStudent.StudentNumber = txtStudentNumber.Text;
                }
            }

            if (tmpSeatNo > 0)
                objStudent.SeatNo = tmpSeatNo;
            else
                objStudent.SeatNo = null;
            K12.Data.Student.Update(objStudent);

            SaveButtonVisible = false;
            CancelButtonVisible = false;

            prlp.SetAfterSaveText("班级", cboClass.Text);
            prlp.SetAfterSaveText("座号", cboSeatNo.Text);
            prlp.SetAfterSaveText("学号", txtStudentNumber.Text);
            prlp.SetActionBy("学籍", "学生班级信息");
            prlp.SetAction("修改学生班级信息");
            prlp.SetDescTitle("学生姓名:" + objStudent.Name + ",学号:" + objStudent.StudentNumber + ",");

            prlp.SaveLog("", "", "student", PrimaryKey);


            this._DefaultClassName = cboClass.Text;
            this._DefaultSeatNo = cboSeatNo.Text;
            this._DefaultStudNum = txtStudentNumber.Text;
            Student.Instance.SyncDataBackground(PrimaryKey);

            reloadChkdData();
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            cboClass.Text = this._DefaultClassName;
            cboSeatNo.Text = this._DefaultSeatNo;
            txtStudentNumber.Text = this._DefaultStudNum;
            setClassName();
            setClassNo();
            SaveButtonVisible = false;
            CancelButtonVisible = false;

        }

        private bool chkClassName()
        {
            bool chkHasName = false;
            if (_ClassNameIDDic.ContainsKey(cboClass.Text) || cboClass.Text == "<空白>")
                chkHasName = true;

            if (cboClass.Text == "" && cboSeatNo.Text == "")
                chkHasName = true;

            return chkHasName;
        }

        private void cboClass_TextChanged(object sender, EventArgs e)
        {
            if (cboClass.Text == "<空白>")
            {
                cboClass.Text = "";
                cboClass.SelectedText = "";
                cboSeatNo.Items.Clear();
            }
            cboSeatNo.Text = "";
            setClassNo();
            Errors.Clear();
        }

        private void cboSeatNo_TextChanged(object sender, EventArgs e)
        {
            Errors.Clear();
        }

        private void ClassItem_Load(object sender, EventArgs e)
        {
            Errors = new EnhancedErrorProvider();
            _ClassNameIDDic = new Dictionary<string, string>();
            _ClassSeatNoList = new List<int>();

            K12.Data.Student.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);

            objStudent = K12.Data.Student.SelectByID(PrimaryKey);
            _AllClassRecs = K12.Data.Class.SelectAll();
            _AllStudRecList = new List<K12.Data.StudentRecord>();
            _studRecList = new List<K12.Data.StudentRecord>();
            BGWork = new BackgroundWorker();
            BGWork.DoWork += new DoWorkEventHandler(BGWork_DoWork);
            BGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWork_RunWorkerCompleted);

            DataListener = new ChangeListener();
            DataListener.Add(new TextBoxSource(txtStudentNumber));
            DataListener.Add(new ComboBoxSource(cboClass, ComboBoxSource.ListenAttribute.Text));
            DataListener.Add(new ComboBoxSource(cboSeatNo, ComboBoxSource.ListenAttribute.Text));
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(ValueManager_StatusChanged);
            prlp = new PermRecLogProcess();

            if (!string.IsNullOrEmpty(PrimaryKey))
                BGWork.RunWorkerAsync();

            Disposed += new EventHandler(ClassItem_Disposed);
        }

    }
}

