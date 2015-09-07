using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;
using SchoolCore;
using K12.Data;

namespace StudentExtension_CN
{
     [FCode("StudentExtension_CN_ParentInfoItem", "家长信息")]
    public partial class ParentInfoItem : FISCA.Presentation.DetailContent
    {
        private ChangeListener _DataListener { get; set; }
        private EnhancedErrorProvider _Errors { get; set; }

        private bool _isBwBusy = false;
        private BackgroundWorker _BGWork;

        private ParentRecord _ParentRecord;
        private AddressRecord _AddressRecord;

        public ParentInfoItem()
        {
            _BGWork = new BackgroundWorker();
            _BGWork.DoWork += _BGWork_DoWork;
            _BGWork.RunWorkerCompleted += _BGWork_RunWorkerCompleted;
            InitializeComponent();
            Group = "家长信息";

            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtAddress));
            _DataListener.Add(new TextBoxSource(txtFatherName));
            _DataListener.Add(new TextBoxSource(txtFatherPhone));
            _DataListener.Add(new TextBoxSource(txtMotherName));
            _DataListener.Add(new TextBoxSource(txtMotherPhone));
            _DataListener.StatusChanged += _DataListener_StatusChanged;
        }

        void _DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBwBusy)
            {
                _isBwBusy = false;
                _BGWork.RunWorkerAsync();
                return;
            }
            LoadData();   
        }

        void _BGWork_DoWork(object sender, DoWorkEventArgs e)
        {          
            // 取得資料
            _ParentRecord = K12.Data.Parent.SelectByStudentID(PrimaryKey);
            _AddressRecord = Address.SelectByStudentID(PrimaryKey);

        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            //txtAddress.Text = "";
            //txtFatherName.Text = "";
            //txtFatherPhone.Text = "";
            //txtMotherName.Text = "";
            //txtMotherPhone.Text = "";

            if (_BGWork.IsBusy)
                _isBwBusy = true;
            else
                _BGWork.RunWorkerAsync();

        }

        private void LoadData()
        {
            _DataListener.SuspendListen();
            txtAddress.Text = _AddressRecord.MailingAddress;
            txtFatherName.Text = _ParentRecord.FatherName;
            txtFatherPhone.Text = _ParentRecord.FatherPhone;
            txtMotherName.Text = _ParentRecord.MotherName;
            txtMotherPhone.Text = _ParentRecord.MotherPhone;
            _DataListener.Reset();
            _DataListener.ResumeListen();

            this.Loading = false;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            LoadData();            
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            try
            {
                _AddressRecord.Mailing.Detail = txtAddress.Text;
                _ParentRecord.Father.Name = txtFatherName.Text;
                _ParentRecord.Father.Phone = txtFatherPhone.Text;
                _ParentRecord.Mother.Name = txtMotherName.Text;
                _ParentRecord.Mother.Phone = txtMotherPhone.Text;

                Address.Update(_AddressRecord);
                K12.Data.Parent.Update(_ParentRecord);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("家长信息," + ex.Message);
            }

            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }
    }
}
