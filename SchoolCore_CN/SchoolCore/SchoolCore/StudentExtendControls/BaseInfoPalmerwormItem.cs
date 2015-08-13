using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using SchoolCore.Feature.Legacy;
using SchoolCore;
using FCode = Framework.Security.FeatureCodeAttribute;
//using K12.EduAdminDataMapping;

namespace SchoolCore.StudentExtendControls
{
    [FCode("JHSchool.Student.Detail0000", "基本数据")]
    internal partial class BaseInfoPalmerwormItem : FISCA.Presentation.DetailContent
    {
        private bool _isInitialized = false;
        private EnhancedErrorProvider _errors = new EnhancedErrorProvider();
        private bool _isBGBusy = false;
        private BackgroundWorker _BGWorker;
        private K12.Data.StudentRecord _StudRec;
        private string _defaultLoginID = string.Empty;
        private string _defaultIDNumber = string.Empty;

        // 入学照片
        private string _FreshmanPhotoStr = string.Empty;

        // 毕业照片
        private string _GraduatePhotoStr = string.Empty;

        private ChangeListener _DataListener { get; set; }
        PermRecLogProcess prlp;

        public BaseInfoPalmerwormItem()
        {
            InitializeComponent();
            Group = "基本数据";
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtName));
            _DataListener.Add(new TextBoxSource(txtSSN));
            _DataListener.Add(new TextBoxSource(txtBirthDate));
            _DataListener.Add(new TextBoxSource(txtBirthPlace));
            _DataListener.Add(new TextBoxSource(txtEngName));
            _DataListener.Add(new TextBoxSource(txtLoginID));
            _DataListener.Add(new TextBoxSource(txtLoginPwd));
            _DataListener.Add(new ComboBoxSource(cboGender, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboNationality, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboAccountType, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);

            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            prlp = new PermRecLogProcess();
            Initialize();
            K12.Data.Student.AfterChange += Student_AfterChange;
            K12.Data.Student.AfterDelete += Student_AfterDelete;
            Disposed += BaseInfoPalmerwormItem_Disposed;
        }

        void BaseInfoPalmerwormItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Student.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(Student_AfterChange);
            K12.Data.Student.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(Student_AfterDelete);
        }

        void Student_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            Student.Instance.SyncAllBackground();
        }

        void Student_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(Student_AfterChange), sender, e);
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
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGBusy)
            {
                _isBGBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }
            BindDataToForm();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {

            SetFormDataToDALRec();

            // 检查生日


            // 检查性别
            List<string> checkGender = new List<string>();
            checkGender.Add("男");
            checkGender.Add("");
            checkGender.Add("女");

            if (!checkGender.Contains(cboGender.Text))
            {
                _errors.SetError(cboGender, "性别错误，请确认数据。");
                return;
            }

            DateTime dt;

            if (!string.IsNullOrEmpty(txtBirthDate.Text))
            {
                if (!DateTime.TryParse(txtBirthDate.Text, out dt))
                {
                    _errors.SetError(txtBirthDate, "日期错误，请确认数据。");
                    return;
                }
            }
            else
            {
                _StudRec.Birthday = null;
            }

            List<string> checkID = new List<string>();
            List<string> checkSSN = new List<string>();


            foreach (K12.Data.StudentRecord studRec in K12.Data.Student.SelectAll())
            {
                checkID.Add(studRec.SALoginName);
                checkSSN.Add(studRec.IDNumber);
            }
            if (!string.IsNullOrEmpty(_StudRec.SALoginName))
                if (checkID.Contains(_StudRec.SALoginName))
                {
                    if (_defaultLoginID != _StudRec.SALoginName)
                    {
                        _errors.SetError(txtLoginID, "学生登入账号重复，请确认资料。");
                        return;
                    }
                }
            if (!string.IsNullOrEmpty(_StudRec.IDNumber))
                if (checkSSN.Contains(_StudRec.IDNumber))
                {
                    if (_defaultIDNumber != _StudRec.IDNumber)
                    {
                        _errors.SetError(txtSSN, "身分证号重复，请确认数据。");
                        return;
                    }
                }

            K12.Data.Student.Update(_StudRec);
            SetAfterEditLog();
            Student.Instance.SyncDataBackground(PrimaryKey);
            _errors.Clear();
            //BindDataToForm();
        }

        private void SetFormDataToDALRec()
        {
            _StudRec.AccountType = cboAccountType.Text;

            DateTime dt;
            if (DateTime.TryParse(txtBirthDate.Text, out dt))
                _StudRec.Birthday = dt;

            _StudRec.BirthPlace = txtBirthPlace.Text;
            _StudRec.EnglishName = txtEngName.Text;
            _StudRec.Gender = cboGender.Text;
            _StudRec.IDNumber = txtSSN.Text;
            _StudRec.Name = txtName.Text;
            _StudRec.Nationality = cboNationality.Text;
            _StudRec.SALoginName = txtLoginID.Text;
            _StudRec.SAPassword = txtLoginPwd.Text;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            _DataListener.SuspendListen();
            _errors.Clear();
            ClearFormValue();
            LoadDALDataToForm();
            _DataListener.Reset();
            _DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get Photo
            _FreshmanPhotoStr = _GraduatePhotoStr = string.Empty;
            _FreshmanPhotoStr = K12.Data.Photo.SelectFreshmanPhoto(PrimaryKey);
            _GraduatePhotoStr = K12.Data.Photo.SelectGraduatePhoto(PrimaryKey);

            // studentRec
            _StudRec = K12.Data.Student.SelectByID(PrimaryKey);
        }



        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            _errors.Clear();
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }

        //将画面清空
        private void ClearFormValue()
        {
            txtBirthDate.Text = txtBirthPlace.Text = txtEngName.Text = txtLoginID.Text = txtName.Text = txtSSN.Text = cboAccountType.Text = cboGender.Text = cboNationality.Text = string.Empty;
        }

        private void BindDataToForm()
        {
            // 主要加当学生被删除时检查
            if (_StudRec != null)
            {
                _DataListener.SuspendListen();
                ClearFormValue();
                LoadDALDataToForm();
                SetBeforeEditLog();

                // get checkDefault
                _defaultIDNumber = _StudRec.IDNumber;
                _defaultLoginID = _StudRec.SALoginName;
                this.Loading = false;
                SaveButtonVisible = false;
                CancelButtonVisible = false;
                _DataListener.Reset();
                _DataListener.ResumeListen();
            }
        }

        private void SetBeforeEditLog()
        {
            prlp.SetBeforeSaveText("姓名", txtName.Text);
            prlp.SetBeforeSaveText("身分证号", txtSSN.Text);
            prlp.SetBeforeSaveText("生日", txtBirthDate.Text);
            prlp.SetBeforeSaveText("性别", cboGender.Text);
            prlp.SetBeforeSaveText("国籍", cboNationality.Text);
            prlp.SetBeforeSaveText("出生地", txtBirthPlace.Text);
            prlp.SetBeforeSaveText("英文姓名", txtEngName.Text);
            prlp.SetBeforeSaveText("登入账号", txtLoginID.Text);
            prlp.SetBeforeSaveText("账号类型", cboAccountType.Text);
        }

        private void SetAfterEditLog()
        {
            prlp.SetAfterSaveText("姓名", txtName.Text);
            prlp.SetAfterSaveText("身分证号", txtSSN.Text);
            prlp.SetAfterSaveText("生日", txtBirthDate.Text);
            prlp.SetAfterSaveText("性别", cboGender.Text);
            prlp.SetAfterSaveText("国籍", cboNationality.Text);
            prlp.SetAfterSaveText("出生地", txtBirthPlace.Text);
            prlp.SetAfterSaveText("英文姓名", txtEngName.Text);
            prlp.SetAfterSaveText("登入账号", txtLoginID.Text);
            prlp.SetAfterSaveText("账号类型", cboAccountType.Text);
            prlp.SetActionBy("学籍", "学生基本资料");
            prlp.SetAction("修改学生基本资料");
            prlp.SetDescTitle("姓名:" + _StudRec.Name + ",学号:" + _StudRec.StudentNumber + ",");
            prlp.SaveLog("", "", "Student", PrimaryKey);

        }

        private void LoadDALDataToForm()
        {
            if (_StudRec.Birthday.HasValue)
                txtBirthDate.Text = _StudRec.Birthday.Value.ToShortDateString();
            txtBirthPlace.Text = _StudRec.BirthPlace;
            txtEngName.Text = _StudRec.EnglishName;
            txtLoginID.Text = _StudRec.SALoginName;
            txtLoginPwd.Text = _StudRec.SAPassword;
            txtName.Text = _StudRec.Name;
            txtSSN.Text = _StudRec.IDNumber;
            cboAccountType.Text = _StudRec.AccountType;
            cboGender.Text = _StudRec.Gender;
            cboNationality.Text = _StudRec.Nationality;
            // 解析
            try
            {

                pic1.Image = Photo.ConvertFromBase64Encoding(_FreshmanPhotoStr, pic1.Width, pic1.Height);
            }
            catch (Exception)
            {
                pic1.Image = pic1.InitialImage;
            }

            try
            {
                pic2.Image = Photo.ConvertFromBase64Encoding(_GraduatePhotoStr, pic2.Width, pic2.Height);
            }
            catch (Exception)
            {
                pic2.Image = pic2.InitialImage;
            }

        }

        public DetailContent GetContent()
        {
            return new BaseInfoPalmerwormItem();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            //载入国家列表
            try
            {
                //List<string> dataList = new List<string>();
                //foreach (string item in Utility.GetNationalityMappingDict().Keys)
                //    dataList.Add(item);
                //cboNationality.Items.AddRange(dataList.ToArray());
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }

            //this.cboNationality.Items.Add("中华民国");
            //this.cboNationality.Items.Add("中华人民共合国");
            //this.cboNationality.Items.Add("孟加拉国");
            //this.cboNationality.Items.Add("缅甸");
            //this.cboNationality.Items.Add("印度尼西亚");
            //this.cboNationality.Items.Add("日本");
            //this.cboNationality.Items.Add("韩国");
            //this.cboNationality.Items.Add("马来西亚");
            //this.cboNationality.Items.Add("菲律宾");
            //this.cboNationality.Items.Add("新加坡");
            //this.cboNationality.Items.Add("泰国");
            //this.cboNationality.Items.Add("越南");
            //this.cboNationality.Items.Add("文莱");
            //this.cboNationality.Items.Add("澳大利亚");
            //this.cboNationality.Items.Add("新西兰");
            //this.cboNationality.Items.Add("埃及");
            //this.cboNationality.Items.Add("南非");
            //this.cboNationality.Items.Add("法国");
            //this.cboNationality.Items.Add("意大利");
            //this.cboNationality.Items.Add("瑞典");
            //this.cboNationality.Items.Add("英国");
            //this.cboNationality.Items.Add("德国");
            //this.cboNationality.Items.Add("加拿大");
            //this.cboNationality.Items.Add("哥斯达黎加");
            //this.cboNationality.Items.Add("危地马拉");
            //this.cboNationality.Items.Add("美国");
            //this.cboNationality.Items.Add("阿根廷");
            //this.cboNationality.Items.Add("巴西");
            //this.cboNationality.Items.Add("哥伦比亚");
            //this.cboNationality.Items.Add("巴拉圭");
            //this.cboNationality.Items.Add("乌拉圭");
            //this.cboNationality.Items.Add("其他");

            cboGender.Items.AddRange(new string[] { "男", "女" });




            _isInitialized = true;
        }


        private void buttonItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic1.Size);
                    pic1.Image = newBmp;

                    _FreshmanPhotoStr = ToBase64String(Photo.Resize(new Bitmap(orgBmp)));
                    K12.Data.Photo.UpdateFreshmanPhoto(_FreshmanPhotoStr, PrimaryKey);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic2.Size);
                    pic2.Image = newBmp;

                    _GraduatePhotoStr = ToBase64String(Photo.Resize(new Bitmap(orgBmp)));

                    K12.Data.Photo.UpdateGraduatePhoto(_GraduatePhotoStr, PrimaryKey);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private static string ToBase64String(Bitmap newBmp)
        {
            MemoryStream ms = new MemoryStream();
            newBmp.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();

            return Convert.ToBase64String(bytes);
        }

        //另存照片
        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_FreshmanPhotoStr);
        }

        //另存照片
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            SavePicture(_GraduatePhotoStr);
        }

        private void SavePicture(string imageString)
        {
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG 影像|*.png;";
            sd.FileName = txtSSN.Text + ".png";

            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(sd.FileName, FileMode.Create);
                    byte[] imageData = Convert.FromBase64String(imageString);
                    fs.Write(imageData, 0, imageData.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }


        private void txtBirthDate_Validated_1(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);

            if (!txtBirthDate.IsValid)
                _errors.SetError(txtBirthDate, "请输入 yyyy/mm/dd 符合日期格式文字");
        }

        private void txtSSN_Validating(object sender, CancelEventArgs e)
        {
            ValidateIDNumber();
        }

        private void txtLoginID_Validating(object sender, CancelEventArgs e)
        {
            ValidateLoginID();
        }

        // 检查
        private void ValidateIDNumber()
        {
            _errors.SetError(txtSSN, string.Empty);

            if (string.IsNullOrEmpty(txtSSN.Text))
            {
                _errors.SetError(txtSSN, string.Empty);
                return;
            }

            if (QueryStudent.IDNumberExists(PrimaryKey, txtSSN.Text))
                _errors.SetError(txtSSN, "身分证号重复，请确认数据。");

        }

        private void ValidateLoginID()
        {
            _errors.SetError(txtLoginID, string.Empty);

            if (string.IsNullOrEmpty(txtLoginID.Text))
            {
                _errors.SetError(txtLoginID, string.Empty);
                return;
            }

            if (QueryStudent.LoginIDExists(txtLoginID.Text, PrimaryKey))
                _errors.SetError(txtLoginID, "账号重复，请重新选择。");
        }

        #region 清除照片
        //清除新生照片
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您确定要清除此学生的照片吗？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _FreshmanPhotoStr = string.Empty;
                pic1.Image = pic1.InitialImage;
                K12.Data.Photo.UpdateFreshmanPhoto("", PrimaryKey);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        //清除毕业照片
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您确定要清除此学生的照片吗？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _GraduatePhotoStr = string.Empty;
                pic2.Image = pic2.InitialImage;
                K12.Data.Photo.UpdateGraduatePhoto("", PrimaryKey);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }
        #endregion

        private void txtBirthDate_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);
        }

        private void cboGender_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(cboGender, string.Empty);
        }
    }
}

