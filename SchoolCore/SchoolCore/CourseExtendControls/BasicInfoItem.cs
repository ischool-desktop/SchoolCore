using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace SchoolCore.CourseExtendControls
{
    [FCode("JHSchool.Course.Detail0000", "基本資料")]
    internal partial class BasicInfoItem : FISCA.Presentation.DetailContent
    {
        bool _isbgWorkerBusy = false;
        BackgroundWorker _bgWorker;
        ChangeListener _DataListener;
        K12.Data.CourseRecord _CourseRecord;
        public BasicInfoItem()
        {
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += _bgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            InitializeComponent();
            Group = "基本資料";
            // 檢查資料是否變更
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtCourseName));
            _DataListener.Add(new TextBoxSource(txtSubject));
            _DataListener.Add(new TextBoxSource(txtPeriodCredit));
            _DataListener.Add(new ComboBoxSource(cboClass,ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboMultiTeacher, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cbxSchoolYear, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cbxSemester, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += _DataListener_StatusChanged;
            K12.Data.Course.AfterChange += Course_AfterChange;
            K12.Data.Course.AfterDelete += Course_AfterDelete;
            Disposed += new EventHandler(BaseInfoItem_Disposed);
        }

        void _DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void Course_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            Course.Instance.SyncAllBackground();
        }

        void Course_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(Course_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_bgWorker.IsBusy)
                        _bgWorker.RunWorkerAsync();
                }            
            }
        }

        void BaseInfoItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Course.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(Course_AfterChange);
            K12.Data.Course.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(Course_AfterDelete);
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isbgWorkerBusy)
            {
                _isbgWorkerBusy = false;
                _bgWorker.RunWorkerAsync();
                return;
            }
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 讀取課程
   
            // 讀取教師

            // 依課程ID 讀取授課教師
            

        }

        /// <summary>
        /// 透過課程 ID 取得授課教師記錄
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        private List<K12.Data.TCInstructRecord> GetTCInstructRecordByCourseID(string CourseID)
        {
            List<K12.Data.TCInstructRecord> tcRecordList = new List<K12.Data.TCInstructRecord>();
            
            return tcRecordList;        
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (_bgWorker.IsBusy)
                _isbgWorkerBusy = true;
            else
                _bgWorker.RunWorkerAsync();

//            ClearErrorMessage();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            base.OnSaveButtonClick(e);
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        //    DataBindToForm();
        }
    }
}
