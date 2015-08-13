using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using SchoolCore;
using K12.Data;
using FCode = Framework.Security.FeatureCodeAttribute;


namespace SchoolCore.TeacherExtendControls
{
    [FCode("JHSchool.Teacher.Detail0000", "教师基本资料")]
    internal partial class BaseInfoItem : FISCA.Presentation.DetailContent
    {
        ErrorProvider epName = new ErrorProvider();
        ErrorProvider epNick = new ErrorProvider();
        ErrorProvider epGender = new ErrorProvider();
        ErrorProvider epLoginName = new ErrorProvider();
        ErrorProvider epIDNumber = new ErrorProvider();
        private bool _isBGWorkBusy = false;
        private BackgroundWorker _BGWork;
        private K12.Data.TeacherRecord _TeacherRec;
        private Dictionary<string, string> _AllTeacherNameDic;
        private Dictionary<string, string> _AllLogIDDic;
        Dictionary<string, string> _AllIDNumberDict = new Dictionary<string, string>();
        PermRecLogProcess prlp;
        private ChangeListener _DataListener;

        public BaseInfoItem()
        {
            InitializeComponent();
            Group = "教师基本资料";
            _BGWork = new BackgroundWorker();
            _BGWork.DoWork += new DoWorkEventHandler(_BGWork_DoWork);
            _BGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWork_RunWorkerCompleted);
            _AllTeacherNameDic = new Dictionary<string, string>();
            _AllLogIDDic = new Dictionary<string, string>();
            prlp = new PermRecLogProcess();
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtName));
            _DataListener.Add(new TextBoxSource(txtIDNumber));
            _DataListener.Add(new TextBoxSource(txtNickname));
            _DataListener.Add(new TextBoxSource(txtPhone));
            _DataListener.Add(new TextBoxSource(txtEmail));
            _DataListener.Add(new TextBoxSource(txtCategory));
            _DataListener.Add(new TextBoxSource(txtSTLoginAccount));
            _DataListener.Add(new TextBoxSource(txtSTLoginPwd));
            _DataListener.Add(new ComboBoxSource(cboAccountType, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboGender, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            K12.Data.Teacher.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterChange);
            K12.Data.Teacher.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterDelete);
            Disposed += new EventHandler(BaseInfoItem_Disposed);
        }

        void JHTeacher_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            Teacher.Instance.SyncAllBackground();
        }

        void BaseInfoItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Teacher.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterChange);
            K12.Data.Teacher.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterDelete);
        }

        void JHTeacher_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHTeacher_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWork.IsBusy)
                        _BGWork.RunWorkerAsync();
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

        void _BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGWorkBusy)
            {
                _isBGWorkBusy = false;
                _BGWork.RunWorkerAsync();
                return;
            }

            DataBindToForm();
        }

        // 加载数据到画面
        private void DataBindToForm()
        {
            if (_TeacherRec == null)
                _TeacherRec = new K12.Data.TeacherRecord();

            _DataListener.SuspendListen();

            txtName.Text = _TeacherRec.Name;
            txtIDNumber.Text = _TeacherRec.IDNumber;
            cboGender.Text = _TeacherRec.Gender;
            txtNickname.Text = _TeacherRec.Nickname;
            txtPhone.Text = _TeacherRec.ContactPhone;
            txtEmail.Text = _TeacherRec.Email;
            txtCategory.Text = _TeacherRec.Category;
            txtSTLoginAccount.Text = _TeacherRec.TALoginName;
            txtSTLoginPwd.Text = _TeacherRec.TAPassword;
            cboAccountType.Text = _TeacherRec.AccountType;

            try
            {
                pic1.Image = SchoolCore.Photo.ConvertFromBase64Encoding(_TeacherRec.Photo, pic1.Width, pic1.Height);
            }
            catch (Exception)
            {
                pic1.Image = pic1.InitialImage;
            }



            // Log
            prlp.SetBeforeSaveText("姓名", txtName.Text);
            prlp.SetBeforeSaveText("身分证号", txtIDNumber.Text);
            prlp.SetBeforeSaveText("性别", cboGender.Text);
            prlp.SetBeforeSaveText("昵称", txtNickname.Text);
            prlp.SetBeforeSaveText("联络电话", txtPhone.Text);
            prlp.SetBeforeSaveText("电子信箱", txtEmail.Text);
            prlp.SetBeforeSaveText("教师类别", txtCategory.Text);
            prlp.SetBeforeSaveText("登入账号", txtSTLoginAccount.Text);
            prlp.SetBeforeSaveText("登入密码", txtSTLoginPwd.Text);
            prlp.SetBeforeSaveText("账号类型", cboAccountType.Text);
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            _DataListener.Reset();
            _DataListener.ResumeListen();
        }

        void _BGWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _AllTeacherNameDic.Clear();
            _AllLogIDDic.Clear();
            _AllIDNumberDict.Clear();

            foreach (K12.Data.TeacherRecord TR in K12.Data.Teacher.SelectAll())
            {
                _AllTeacherNameDic.Add(TR.Name + TR.Nickname, TR.ID);

                if (!string.IsNullOrEmpty(TR.TALoginName))
                    _AllLogIDDic.Add(TR.TALoginName, TR.ID);

                // 身分证号检查
                if (!string.IsNullOrWhiteSpace(TR.IDNumber))
                    if (!_AllIDNumberDict.ContainsKey(TR.IDNumber))
                        _AllIDNumberDict.Add(TR.IDNumber, TR.ID);
            }

            // 读取教师数据
            _TeacherRec = K12.Data.Teacher.SelectByID(PrimaryKey);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (_BGWork.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWork.RunWorkerAsync();

            ClearErrorMessage();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {

            // 验证数据
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                epName.SetError(txtName, "姓名不可空白!");
                return;
            }

            // 检查账号是否重复
            if (_AllLogIDDic.ContainsKey(txtSTLoginAccount.Text))
            {
                if (_AllLogIDDic[txtSTLoginAccount.Text] != _TeacherRec.ID)
                {
                    epLoginName.SetError(txtSTLoginAccount, "登入账号重复!");
                    return;
                }
            }

            // 检查姓名+昵称是否重复
            string checkName = txtName.Text + txtNickname.Text;

            if (_AllTeacherNameDic.ContainsKey(checkName))
            {
                if (_AllTeacherNameDic[checkName] != _TeacherRec.ID)
                {
                    epName.SetError(txtName, "姓名+昵称重复，请检查!");
                    epLoginName.SetError(txtNickname, "姓名+昵称重复，请检查!");
                    return;
                }
            }

            // 检查身分证号是否重复
            if (_AllIDNumberDict.ContainsKey(txtIDNumber.Text))
            {
                if (_TeacherRec.ID != _AllIDNumberDict[txtIDNumber.Text])
                {
                    epIDNumber.SetError(txtIDNumber, "身分证号重复，请检查!");
                    return;
                }
            }

            // 回填到 DAL
            _TeacherRec.AccountType = cboAccountType.Text;
            _TeacherRec.Category = txtCategory.Text;
            _TeacherRec.ContactPhone = txtPhone.Text;
            _TeacherRec.Email = txtEmail.Text;
            _TeacherRec.Gender = cboGender.Text;
            _TeacherRec.IDNumber = txtIDNumber.Text;
            _TeacherRec.TALoginName = txtSTLoginAccount.Text;
            _TeacherRec.Name = txtName.Text;
            _TeacherRec.Nickname = txtNickname.Text;
            _TeacherRec.TAPassword = txtSTLoginPwd.Text;

            // 存档
            K12.Data.Teacher.Update(_TeacherRec);

            // Save Log
            prlp.SetAfterSaveText("姓名", txtName.Text);
            prlp.SetAfterSaveText("身分证号", txtIDNumber.Text);
            prlp.SetAfterSaveText("性别", cboGender.Text);
            prlp.SetAfterSaveText("昵称", txtNickname.Text);
            prlp.SetAfterSaveText("联络电话", txtPhone.Text);
            prlp.SetAfterSaveText("电子信箱", txtEmail.Text);
            prlp.SetAfterSaveText("教师类别", txtCategory.Text);
            prlp.SetAfterSaveText("登入账号", txtSTLoginAccount.Text);
            prlp.SetAfterSaveText("登入密码", txtSTLoginPwd.Text);
            prlp.SetAfterSaveText("账号类型", cboAccountType.Text);


            prlp.SetDescTitle("教师姓名：" + txtName.Text + ",");
            prlp.SetActionBy("学籍", "教师基本资料");
            prlp.SetAction("修改教师基本资料");
            prlp.SaveLog("", "", "teacher", PrimaryKey);
            DataBindToForm();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            Teacher.Instance.SyncDataBackground(PrimaryKey);
            Class.Instance.SyncAllBackground();
            ClearErrorMessage();
        }

        private void ClearErrorMessage()
        {
            epGender.Clear();
            epLoginName.Clear();
            epName.Clear();
            epNick.Clear();
            epIDNumber.Clear();
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            DataBindToForm();
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

                    _TeacherRec.Photo = ToBase64String(SchoolCore.Photo.Resize(new Bitmap(orgBmp)));
                    K12.Data.Teacher.Update(_TeacherRec);
                    prlp.SaveLog("学籍系统-教师基本数据", "变更教师照片", "teacher", PrimaryKey, "变更教师:" + _TeacherRec.Name + "的照片");
                    DataBindToForm();

                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        public DetailContent GetContent()
        {
            return new BaseInfoItem();
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

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_TeacherRec.Photo);
        }

        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您确定要清除此学生的照片吗？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _TeacherRec.Photo = string.Empty;
                pic1.Image = pic1.InitialImage;
                K12.Data.Teacher.Update(_TeacherRec);
                prlp.SaveLog("学籍系统-教师基本数据", "变更教师照片", "teacher", PrimaryKey, "变更教师:" + _TeacherRec.Name + "的照片");
                DataBindToForm();
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        private void SavePicture(string imageString)
        {
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG 影像|*.png;";
            sd.FileName = txtIDNumber.Text + ".png";

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

        private void txtIDNumber_TextChanged(object sender, EventArgs e)
        {
            epIDNumber.Clear();
        }
    }
}

