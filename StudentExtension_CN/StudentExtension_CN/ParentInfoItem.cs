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
    public partial class ParentInfoItem : FISCA.Presentation.DetailContent
    {
        private ChangeListener _DataListener { get; set; }
        private EnhancedErrorProvider _Errors { get; set; }

        private bool _isBwBusy = false;
        private BackgroundWorker _BGWork;

        public ParentInfoItem()
        {
            _BGWork = new BackgroundWorker();
            _BGWork.DoWork += _BGWork_DoWork;
            _BGWork.RunWorkerCompleted += _BGWork_RunWorkerCompleted;
            InitializeComponent();
            Group = "家長資訊";

        }

        void _BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void _BGWork_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_isBwBusy)
            {
                _isBwBusy = false;
                _BGWork.RunWorkerAsync();
                return;
            }           
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            
        }
    }
}
