using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace SchoolCore.ClassExtendControls
{
    [FCode("JHSchool.Class.Detail0010", "班级基本数据")]
    internal partial class ClassBaseInfoItem : FISCA.Presentation.DetailContent
    {
        //年级清单
        List<string> _gradeYearList = new List<string>();
        //?
        private ErrorProvider epTeacher = new ErrorProvider();
        private ErrorProvider epDisplayOrder = new ErrorProvider();
        private ErrorProvider epGradeYear = new ErrorProvider();
        private ErrorProvider epClassName = new ErrorProvider();
        private PermRecLogProcess prlp;

        private bool _isBGWorkBusy = false;
        private BackgroundWorker _BGWorker;
        private ChangeListener _DataListener { get; set; }

        private K12.Data.ClassRecord _ClassRecord;
        private List<K12.Data.ClassRecord> _AllClassRecList;
        private Dictionary<string, string> _TeacherNameDic;

        //?
        private string _NamingRule = "";

        //建构子
        public ClassBaseInfoItem()
        {
            InitializeComponent();
            Group = "班级基本数据";
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtClassName));
            _DataListener.Add(new TextBoxSource(txtSortOrder));
            _DataListener.Add(new ComboBoxSource(cboGradeYear, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboTeacher, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);
            _TeacherNameDic = new Dictionary<string, string>();
            prlp = new PermRecLogProcess();
            K12.Data.Class.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(Class_AfterChange);
            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            Disposed += new EventHandler(ClassBaseInfoItem_Disposed);
        }

        void ClassBaseInfoItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Class.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(Class_AfterChange);
        }

        void Class_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(Class_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWorker.IsBusy)
                        _BGWorker.RunWorkerAsync();
                }
            }
        }

        void _DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (!IsValid())
            {
                FISCA.Presentation.Controls.MsgBox.Show("输入数据未通过验证，请修正后再行储存");
                return;
            }


            _ClassRecord.NamingRule = _NamingRule;

            // 年级
            int GrYear;
            if (int.TryParse(cboGradeYear.Text, out GrYear))
                _ClassRecord.GradeYear = GrYear;
            else
                _ClassRecord.GradeYear = null;

            // 班名转型
            if (ValidateNamingRule(_NamingRule))
                _ClassRecord.Name = ParseClassName(_NamingRule, GrYear);
            else
            {
                if (ValidClassName(_ClassRecord.ID, txtClassName.Text))
                    _ClassRecord.Name = txtClassName.Text;
                else
                    return;
            }

            _ClassRecord.RefTeacherID = "";
            // 教师
            foreach (KeyValuePair<string, string> val in _TeacherNameDic)
                if (val.Value == cboTeacher.Text)
                    _ClassRecord.RefTeacherID = val.Key;
            _ClassRecord.DisplayOrder = txtSortOrder.Text;

            SaveButtonVisible = false;
            CancelButtonVisible = false;
            // Log
            prlp.SetAfterSaveText("班级名称", txtClassName.Text);
            prlp.SetAfterSaveText("班级命名规则", _ClassRecord.NamingRule);
            prlp.SetAfterSaveText("年级", cboGradeYear.Text);
            prlp.SetAfterSaveText("班导师", cboTeacher.Text);
            prlp.SetAfterSaveText("排列序号", txtSortOrder.Text);
            prlp.SetActionBy("学籍", "班级基本数据");
            prlp.SetAction("修改班级基本数据");
            prlp.SetDescTitle("班级名称:" + _ClassRecord.Name + ",");
            prlp.SaveLog("", "", "class", PrimaryKey);
            K12.Data.Class.Update(_ClassRecord);
            Class.Instance.SyncDataBackground(PrimaryKey);


        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            _DataListener.SuspendListen();
            LoadDALDefaultDataToForm();
            _DataListener.Reset();
            _DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;

        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGWorkBusy)
            {
                _isBGWorkBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }
            BindDataToForm();
        }

        private void LoadDefaultDataToForm()
        {
            // 年级
            LoadGradeYearToForm();

            // 教师
            LoadTeacherNameToForm();
        }

        private void LoadTeacherNameToForm()
        {
            cboTeacher.Items.Clear();
            List<string> nameList = new List<string>();
            foreach (string name in _TeacherNameDic.Values)
                nameList.Add(name);
            nameList.Sort();

            cboTeacher.Items.AddRange(nameList.ToArray());
        }

        private void LoadGradeYearToForm()
        {
            cboGradeYear.Items.Clear();
            List<string> GradeYearList = new List<string>();
            foreach (K12.Data.ClassRecord classRec in K12.Data.Class.SelectAll())
                if (classRec.GradeYear.HasValue)
                    if (!GradeYearList.Contains(classRec.GradeYear.Value + ""))
                        GradeYearList.Add(classRec.GradeYear.Value + "");
            GradeYearList.Sort(GradeYearSort);
            cboGradeYear.Items.AddRange(GradeYearList.ToArray());
        }

        private int GradeYearSort(string x, string y)
        {
            string xx = x.PadLeft(10, '0');
            string yy = y.PadLeft(10, '0');

            return xx.CompareTo(yy);
        }

        private void BindDataToForm()
        {

            _DataListener.SuspendListen();
            // 默认值
            LoadDefaultDataToForm();
            LoadDALDefaultDataToForm();

            // Before log
            prlp.SetBeforeSaveText("班级名称", txtClassName.Text);
            prlp.SetBeforeSaveText("年级", cboGradeYear.Text);
            prlp.SetBeforeSaveText("班导师", cboTeacher.Text);
            prlp.SetBeforeSaveText("排列序号", txtSortOrder.Text);
            if (_ClassRecord != null)
                prlp.SetBeforeSaveText("班级命名规则", _ClassRecord.NamingRule);
            _DataListener.Reset();
            _DataListener.ResumeListen();
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;

        }


        // 将 DAL 资料放到 Form
        private void LoadDALDefaultDataToForm()
        {
            if (_ClassRecord != null)
            {
                txtSortOrder.Text = _ClassRecord.DisplayOrder;
                if (_ClassRecord.GradeYear.HasValue)
                    cboGradeYear.Text = _ClassRecord.GradeYear.Value + "";
                else
                    cboGradeYear.Text = "";

                if (_TeacherNameDic.ContainsKey(_ClassRecord.RefTeacherID))
                    cboTeacher.Text = _TeacherNameDic[_ClassRecord.RefTeacherID];
                else
                    cboTeacher.Text = "";

                _NamingRule = _ClassRecord.NamingRule;
                txtClassName.Text = _ClassRecord.Name;
            }
        }


        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _ClassRecord = K12.Data.Class.SelectByID(PrimaryKey);
            _AllClassRecList = K12.Data.Class.SelectAll();

            // 教师名称索引
            _TeacherNameDic.Clear();
            foreach (K12.Data.TeacherRecord TRec in K12.Data.Teacher.SelectAll())
            {
                if (TRec.Status == K12.Data.TeacherRecord.TeacherStatus.刪除)
                    continue;

                if (string.IsNullOrEmpty(TRec.Nickname))
                    _TeacherNameDic.Add(TRec.ID, TRec.Name);
                else
                    _TeacherNameDic.Add(TRec.ID, TRec.Name + "(" + TRec.Nickname + ")");
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            epClassName.Clear();
            epDisplayOrder.Clear();
            epGradeYear.Clear();
            epTeacher.Clear();
            _NamingRule = "";
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }




        private void InitializeGradeYearList()
        {
            List<string> gList = new List<string>();
            foreach (XmlNode node in SchoolCore.Feature.Legacy.QueryClass.GetGradeYearList().GetContent().GetElements("GradeYear"))
            {
                string gradeYear = node.SelectSingleNode("GradeYear").InnerText;
                if (!gList.Contains(gradeYear))
                    gList.Add(gradeYear);
            }
            cboGradeYear.Items.AddRange(gList.ToArray());
            _gradeYearList = gList;
        }



        protected void cboGradeYear_TextChanged(object sender, EventArgs e)
        {
            if (ValidateNamingRule(_NamingRule))
            {
                int gradeYear = 0;
                if (int.TryParse(cboGradeYear.Text, out gradeYear))
                {
                    string classname = ParseClassName(_NamingRule, gradeYear);
                    classname = classname.Replace("{", "");
                    classname = classname.Replace("}", "");
                    txtClassName.Text = classname;

                    if (_ClassRecord.GradeYear.HasValue)
                        if (gradeYear != _ClassRecord.GradeYear.Value)
                        {
                            SaveButtonVisible = true;
                            CancelButtonVisible = true;
                        }
                        else
                        {
                            SaveButtonVisible = false;
                            CancelButtonVisible = false;
                        }
                }
                else
                    txtClassName.Text = _NamingRule;
            }
        }

        public bool IsValid()
        {
            // 班级名称验证
            bool valid = true;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Tag != null)
                {
                    if (ctrl.Tag.ToString() == "error")
                        valid = false;
                }
            }
            return valid;
        }

        private void cboTeacher_Validated(object sender, EventArgs e)
        {
            string id = string.Empty;

            foreach (KeyValuePair<string, string> var in _TeacherNameDic)
                if (var.Value == cboTeacher.Text)
                    id = var.Key;



            if (!string.IsNullOrEmpty(cboTeacher.Text) && id == "")
            {
                epTeacher.SetError(cboTeacher, "查无此教师");
                cboTeacher.Tag = "error";
                return;
            }
            else
            {
                epTeacher.Clear();
                cboTeacher.Tag = id;
            }
        }

        private void cboTeacher_Validating(object sender, CancelEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int index = combo.FindStringExact(combo.Text);
            if (index >= 0)
            {
                combo.SelectedItem = combo.Items[index];
            }

        }

        private void txtSortOrder_Validated(object sender, EventArgs e)
        {
            string text = txtSortOrder.Text;
            int i;
            if (!string.IsNullOrEmpty(text) && !int.TryParse(text, out i))
            {
                epDisplayOrder.SetError(txtSortOrder, "请输入整数");
                txtSortOrder.Tag = "error";
                return;
            }
            epDisplayOrder.Clear();
            txtSortOrder.Tag = null;
        }

        private void cboGradeYear_Validated(object sender, EventArgs e)
        {
            string text = cboGradeYear.Text;
            bool hasGradeYear = false;

            if (_gradeYearList.Contains(text))
                hasGradeYear = true;

            int i;
            if (!string.IsNullOrEmpty(text) && !int.TryParse(text, out i))
            {
                epGradeYear.SetError(cboGradeYear, "年级必须为数字");
                cboGradeYear.Tag = "error";
                return;
            }

            if (!string.IsNullOrEmpty(text) && !hasGradeYear)
            {
                epGradeYear.SetError(cboGradeYear, "无此年级");
                cboGradeYear.Tag = null;
            }
            else
            {
                epGradeYear.Clear();
                cboGradeYear.Tag = null;
            }
        }




        private bool ValidateNamingRule(string namingRule)
        {
            return namingRule.IndexOf('{') < namingRule.IndexOf('}');
        }

        // 检查班级命名规则
        private string ParseClassName(string namingRule, int gradeYear)
        {
            // 当年级是7,8,9
            if (gradeYear >= 6)
                gradeYear -= 6;

            gradeYear--;
            if (!ValidateNamingRule(namingRule))
                return namingRule;
            string classlist_firstname = "", classlist_lastname = "";
            if (namingRule.Length == 0) return "{" + (gradeYear + 1) + "}";

            string tmp_convert = namingRule;

            // 找出"{"之前文字 并放入 classlist_firstname , 并除去"{"
            if (tmp_convert.IndexOf('{') > 0)
            {
                classlist_firstname = tmp_convert.Substring(0, tmp_convert.IndexOf('{'));
                tmp_convert = tmp_convert.Substring(tmp_convert.IndexOf('{') + 1, tmp_convert.Length - (tmp_convert.IndexOf('{') + 1));
            }
            else tmp_convert = tmp_convert.TrimStart('{');

            // 找出 } 之后文字 classlist_lastname , 并除去"}"
            if (tmp_convert.IndexOf('}') > 0 && tmp_convert.IndexOf('}') < tmp_convert.Length - 1)
            {
                classlist_lastname = tmp_convert.Substring(tmp_convert.IndexOf('}') + 1, tmp_convert.Length - (tmp_convert.IndexOf('}') + 1));
                tmp_convert = tmp_convert.Substring(0, tmp_convert.IndexOf('}'));
            }
            else tmp_convert = tmp_convert.TrimEnd('}');

            // , 存入 array
            string[] listArray = new string[tmp_convert.Split(',').Length];
            listArray = tmp_convert.Split(',');

            // 检查是否在清单范围
            if (gradeYear >= 0 && gradeYear < listArray.Length)
            {
                tmp_convert = classlist_firstname + listArray[gradeYear] + classlist_lastname;
            }
            else
            {
                tmp_convert = classlist_firstname + "{" + (gradeYear + 1) + "}" + classlist_lastname;
            }
            return tmp_convert;
        }

        // 检查班级名称是否重复
        private bool ValidClassName(string classid, string className)
        {
            if (string.IsNullOrEmpty(className)) return false;
            foreach (K12.Data.ClassRecord classRec in _AllClassRecList)
            {
                if (classRec.ID != classid && classRec.Name == className)
                    return false;
            }
            return true;
        }

        private void txtClassName_TextChanged(object sender, EventArgs e)
        {
            _DataListener.SuspendListen();
            //if (!_StopEvent)
            //{

            if (string.IsNullOrEmpty(txtClassName.Text))
            {
                epClassName.SetError(txtClassName, "班级名称不可空白");
                txtClassName.Tag = "error";
                _DataListener.Reset();
                _DataListener.ResumeListen();

                return;
            }
            if (ValidClassName(PrimaryKey, txtClassName.Text) == false)
            {
                epClassName.SetError(txtClassName, "班级不可与其它班级重复");
                txtClassName.Tag = "error";
                _DataListener.Reset();
                _DataListener.ResumeListen();

                return;
            }
            epClassName.Clear();
            txtClassName.Tag = null;
            //}
            _DataListener.Reset();
            _DataListener.ResumeListen();

        }

        private void txtClassName_Leave(object sender, EventArgs e)
        {
            _DataListener.SuspendListen();

            _NamingRule = txtClassName.Text;

            if (ValidateNamingRule(txtClassName.Text))
            {
                int gradeYear = 0;
                if (int.TryParse(cboGradeYear.Text, out gradeYear))
                {
                    txtClassName.Text = ParseClassName(_NamingRule, gradeYear);
                }
                //_ClassRecord.NamingRule = _NamingRule;
            }
            else
            {
                _ClassRecord.NamingRule = _NamingRule;
            }
            _DataListener.Reset();
            _DataListener.ResumeListen();
            //if (!string.IsNullOrEmpty(_ClassRecord.NamingRule))
            if ((txtClassName.Text != _ClassRecord.Name) || (_NamingRule != _ClassRecord.NamingRule))
            {
                SaveButtonVisible = true;
                CancelButtonVisible = true;
                //txtClassName.Focus();
            }

            if (string.IsNullOrEmpty(_ClassRecord.NamingRule))
            {
                SaveButtonVisible = false;
                CancelButtonVisible = false;
            }
        }

        private void txtClassName_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_NamingRule))
            {

                _DataListener.SuspendListen();

                if (ValidateNamingRule(_NamingRule))
                {
                    //_StopEvent = true;
                    txtClassName.Text = _NamingRule;
                    //_StopEvent = false;
                }
                else
                    _NamingRule = txtClassName.Text;

                _DataListener.Reset();
                _DataListener.ResumeListen();
            }
        }

        public DetailContent GetContent()
        {
            return new ClassBaseInfoItem();
        }
    }
}

