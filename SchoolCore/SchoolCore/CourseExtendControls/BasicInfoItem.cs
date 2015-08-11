using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using FISCA.DSAUtil;
using System.Xml;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;
using DevComponents.Editors;


namespace SchoolCore.CourseExtendControls
{
    [FCode("JHSchool.Course.Detail0000", "基本資料")]
    internal partial class BasicInfoItem : FISCA.Presentation.DetailContent
    {
        public BasicInfoItem()
        {
            InitializeComponent();
            Group = "基本資料";

        }
    }
}
