using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.DSAUtil;
using System.Xml;
using Framework.Feature;

namespace SchoolCore
{
    public partial class SchoolInfoMangement : BaseForm
    {
        SchoolInfoEntity SchoolInfoEnt;
        public SchoolInfoMangement()
        {
            InitializeComponent();

        }

        // 以后交给 DAL
        private SchoolInfoEntity getSchoolInfoData()
        {
            XmlElement Element = Config.GetSchoolInfo();
            XmlElement sElement = Config.GetSystemConfig();
            SchoolInfoEntity sie = new SchoolInfoEntity();

            sie.ChinsesName = getNodeData("ChineseName", Element, "SchoolInformation");
            sie.EnglishName = getNodeData("EnglishName", Element, "SchoolInformation");
            sie.ChineseAddress = getNodeData("Address", Element, "SchoolInformation");
            sie.EnglishAddress = getNodeData("EnglishAddress", Element, "SchoolInformation");
            sie.Code = getNodeData("Code", Element, "SchoolInformation");
            sie.Fax = getNodeData("Fax", Element, "SchoolInformation");
            sie.Telephone = getNodeData("Telephone", Element, "SchoolInformation");
            sie.SchoolYear = getNodeData("DefaultSchoolYear", sElement, "SystemConfig");
            sie.Semester = getNodeData("DefaultSemester", sElement, "SystemConfig");

            //校长
            sie.ChancellorChsName = getNodeData("ChancellorChineseName", Element, "SchoolInformation");
            sie.ChancellorEngName = getNodeData("ChancellorEnglishName", Element, "SchoolInformation");
            sie.ChancellorCellPhone = getNodeData("ChancellorCellPhone", Element, "SchoolInformation");
            sie.ChancellorEmail = getNodeData("ChancellorEmail", Element, "SchoolInformation");

            //教务主任
            sie.EduDirectorName = getNodeData("EduDirectorName", Element, "SchoolInformation");
            sie.EduDirectorCellPhone = getNodeData("EduDirectorCellPhone", Element, "SchoolInformation");
            sie.EduDirectorEmail = getNodeData("EduDirectorEmail", Element, "SchoolInformation");

            //学务主任
            sie.StuDirectorName = getNodeData("StuDirectorName", Element, "SchoolInformation");
            sie.StuDirectorCellPhone = getNodeData("StuDirectorCellPhone", Element, "SchoolInformation");
            sie.StuDirectorEmail = getNodeData("StuDirectorEmail", Element, "SchoolInformation");

            sie.AssociatedWithName = getNodeData("AssociatedWithName", Element, "SchoolInformation");
            sie.AssociatedWithCellPhone = getNodeData("AssociatedWithCellPhone", Element, "SchoolInformation");
            sie.AssociatedWithEmail = getNodeData("AssociatedWithEmail", Element, "SchoolInformation");

            sie.OtherTitle = getNodeData("OtherTitle", Element, "SchoolInformation");
            sie.OtherName = getNodeData("OtherName", Element, "SchoolInformation");
            sie.OtherCellPhone = getNodeData("OtherCellPhone", Element, "SchoolInformation");
            sie.OtherEmail = getNodeData("OtherEmail", Element, "SchoolInformation");

            return sie;
        }


        private string getNodeData(string nodeName, XmlElement Element, string nodesName)
        {
            string value = "";
            foreach (XmlElement xe in Element.SelectNodes(nodesName))
            {
                if (xe.SelectSingleNode(nodeName) != null)
                    value = xe.SelectSingleNode(nodeName).InnerText;
            }
            return value;
        }

        // 以后交给 DAL
        private void setSchoolInfoData(SchoolInfoEntity ScInfoEntity)
        {
            //Framework.Feature.Config.SetSchoolInfo();
            // 学校基本资料
            DSXmlHelper helper = new DSXmlHelper("GetSchoolInfoResponse");
            helper.AddElement("SchoolInformation");
            helper.AddElement("SchoolInformation", "ChineseName", ScInfoEntity.ChinsesName);
            helper.AddElement("SchoolInformation", "EnglishName", ScInfoEntity.EnglishName);
            helper.AddElement("SchoolInformation", "Address", ScInfoEntity.ChineseAddress);
            helper.AddElement("SchoolInformation", "EnglishAddress", ScInfoEntity.EnglishAddress);
            helper.AddElement("SchoolInformation", "Code", ScInfoEntity.Code);
            helper.AddElement("SchoolInformation", "Fax", ScInfoEntity.Fax);
            helper.AddElement("SchoolInformation", "Telephone", ScInfoEntity.Telephone);

            helper.AddElement("SchoolInformation", "ChancellorChineseName", ScInfoEntity.ChancellorChsName);
            helper.AddElement("SchoolInformation", "ChancellorEnglishName", ScInfoEntity.ChancellorEngName);
            helper.AddElement("SchoolInformation", "ChancellorCellPhone", ScInfoEntity.ChancellorCellPhone);
            helper.AddElement("SchoolInformation", "ChancellorEmail", ScInfoEntity.ChancellorEmail);

            helper.AddElement("SchoolInformation", "EduDirectorName", ScInfoEntity.EduDirectorName);
            helper.AddElement("SchoolInformation", "EduDirectorCellPhone", ScInfoEntity.EduDirectorCellPhone);
            helper.AddElement("SchoolInformation", "EduDirectorEmail", ScInfoEntity.EduDirectorEmail);

            helper.AddElement("SchoolInformation", "StuDirectorName", ScInfoEntity.StuDirectorName);
            helper.AddElement("SchoolInformation", "StuDirectorCellPhone", ScInfoEntity.StuDirectorCellPhone);
            helper.AddElement("SchoolInformation", "StuDirectorEmail", ScInfoEntity.StuDirectorEmail);

            helper.AddElement("SchoolInformation", "AssociatedWithName", ScInfoEntity.AssociatedWithName);
            helper.AddElement("SchoolInformation", "AssociatedWithCellPhone", ScInfoEntity.AssociatedWithCellPhone);
            helper.AddElement("SchoolInformation", "AssociatedWithEmail", ScInfoEntity.AssociatedWithEmail);

            helper.AddElement("SchoolInformation", "OtherTitle", ScInfoEntity.OtherTitle);
            helper.AddElement("SchoolInformation", "OtherName", ScInfoEntity.OtherName);
            helper.AddElement("SchoolInformation", "OtherCellPhone", ScInfoEntity.OtherCellPhone);
            helper.AddElement("SchoolInformation", "OtherEmail", ScInfoEntity.OtherEmail);


            Config.SetSchoolInfo(helper.BaseElement);

            // 学年度学期
            DSXmlHelper helper1 = new DSXmlHelper("SetSystemConfigRequest");
            helper1.AddElement("SystemConfig");
            helper1.AddElement("SystemConfig", "DefaultSchoolYear", SchoolInfoEnt.SchoolYear);
            helper1.AddElement("SystemConfig", "DefaultSemester", SchoolInfoEnt.Semester);
            Config.SetSystemConfig(helper1.BaseElement);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            SchoolInfoEnt.ChineseAddress = txtSchoolChsAddress.Text;
            SchoolInfoEnt.ChinsesName = txtSchoolChsName.Text;
            SchoolInfoEnt.Code = txtSchoolCode.Text; //学校代码

            SchoolInfoEnt.Telephone = txtPhone.Text;
            SchoolInfoEnt.EnglishAddress = txtSchoolEngAddess.Text;
            SchoolInfoEnt.EnglishName = txtSchoolEngName.Text;
            SchoolInfoEnt.Fax = txtFax.Text;
            SchoolInfoEnt.SchoolYear = intSchoolYear.Text;
            SchoolInfoEnt.Semester = intSemester.Text;

            SchoolInfoEnt.ChancellorChsName = txtChancellorChsName.Text;
            SchoolInfoEnt.ChancellorEngName = txtChancellorEngName.Text;
            SchoolInfoEnt.ChancellorCellPhone = txtChancellorCellPhone.Text;
            SchoolInfoEnt.ChancellorEmail = txtChancellorEmail.Text;

            SchoolInfoEnt.EduDirectorName = txtEduDirectorName.Text;
            SchoolInfoEnt.EduDirectorCellPhone = txtEduDirectorCellPhone.Text;
            SchoolInfoEnt.EduDirectorEmail = txtEduDirectorEmail.Text;

            SchoolInfoEnt.StuDirectorName = txtStuDirectorName.Text;
            SchoolInfoEnt.StuDirectorCellPhone = txtStuDirectorCellPhone.Text;
            SchoolInfoEnt.StuDirectorEmail = txtStuDirectorEmail.Text;

            SchoolInfoEnt.AssociatedWithName = txtAssociatedWithName.Text;
            SchoolInfoEnt.AssociatedWithCellPhone = txtAssociatedWithCellPhone.Text;
            SchoolInfoEnt.AssociatedWithEmail = txtAssociatedWithEmail.Text;

            SchoolInfoEnt.OtherTitle = txtOtherTitle.Text;
            SchoolInfoEnt.OtherName = txtOtherName.Text;
            SchoolInfoEnt.OtherCellPhone = txtOtherCellPhone.Text;
            SchoolInfoEnt.OtherEmail = txtOtherEmail.Text;


            setSchoolInfoData(SchoolInfoEnt);
            MessageBox.Show("数据储存完成");
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("核心", "修改", "修改学校基本数据.");
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SchoolInfoMangement_Load(object sender, EventArgs e)
        {
            SchoolInfoEnt = getSchoolInfoData();

            txtChancellorChsName.Text = SchoolInfoEnt.ChancellorChsName;
            txtChancellorEngName.Text = SchoolInfoEnt.ChancellorEngName;
            txtChancellorCellPhone.Text = SchoolInfoEnt.ChancellorCellPhone;
            txtChancellorEmail.Text = SchoolInfoEnt.ChancellorEmail;

            txtEduDirectorName.Text = SchoolInfoEnt.EduDirectorName;
            txtEduDirectorCellPhone.Text = SchoolInfoEnt.EduDirectorCellPhone;
            txtEduDirectorEmail.Text = SchoolInfoEnt.EduDirectorEmail;

            txtStuDirectorName.Text = SchoolInfoEnt.StuDirectorName;
            txtStuDirectorCellPhone.Text = SchoolInfoEnt.StuDirectorCellPhone;
            txtStuDirectorEmail.Text = SchoolInfoEnt.StuDirectorEmail;

            txtAssociatedWithName.Text = SchoolInfoEnt.AssociatedWithName;
            txtAssociatedWithCellPhone.Text = SchoolInfoEnt.AssociatedWithCellPhone;
            txtAssociatedWithEmail.Text = SchoolInfoEnt.AssociatedWithEmail;

            txtOtherTitle.Text = SchoolInfoEnt.OtherTitle;
            txtOtherName.Text = SchoolInfoEnt.OtherName;
            txtOtherCellPhone.Text = SchoolInfoEnt.OtherCellPhone;
            txtOtherEmail.Text = SchoolInfoEnt.OtherEmail;


            txtFax.Text = SchoolInfoEnt.Fax;
            txtPhone.Text = SchoolInfoEnt.Telephone;
            txtSchoolChsAddress.Text = SchoolInfoEnt.ChineseAddress;
            txtSchoolChsName.Text = SchoolInfoEnt.ChinsesName;
            txtSchoolCode.Text = SchoolInfoEnt.Code;
            txtSchoolEngAddess.Text = SchoolInfoEnt.EnglishAddress;
            txtSchoolEngName.Text = SchoolInfoEnt.EnglishName;
            if (!string.IsNullOrEmpty(SchoolInfoEnt.SchoolYear))
                intSchoolYear.Text = SchoolInfoEnt.SchoolYear;

            if (!string.IsNullOrEmpty(SchoolInfoEnt.Semester))
                intSemester.Text = SchoolInfoEnt.Semester;
        }
    }
    public class SchoolInfoEntity
    {
        /// <summary>
        /// 学校代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 学年度
        /// </summary>
        public string SchoolYear { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string ChinsesName { get; set; }

        /// <summary>
        /// 学校英文名称
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 学校地址
        /// </summary>
        public string ChineseAddress { get; set; }

        /// <summary>
        /// 学校英文地址
        /// </summary>
        public string EnglishAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        #region 学校主管

        /// <summary>
        /// 校长中文姓名
        /// </summary>
        public string ChancellorChsName { get; set; }

        /// <summary>
        /// 校长英文姓名
        /// </summary>
        public string ChancellorEngName { get; set; }

        /// <summary>
        /// 校长手机
        /// </summary>
        public string ChancellorCellPhone { get; set; }

        /// <summary>
        /// 校长Emai
        /// </summary>
        public string ChancellorEmail { get; set; }

        /// <summary>
        /// 教务主任姓名
        /// </summary>
        public string EduDirectorName { get; set; }

        /// <summary>
        /// 教务主任手机
        /// </summary>
        public string EduDirectorCellPhone { get; set; }

        /// <summary>
        /// 教务主任Email
        /// </summary>
        public string EduDirectorEmail { get; set; }

        /// <summary>
        /// 学务主任姓名
        /// </summary>
        public string StuDirectorName { get; set; }

        /// <summary>
        /// 学务主任手机
        /// </summary>
        public string StuDirectorCellPhone { get; set; }

        /// <summary>
        /// 学务主任Email
        /// </summary>
        public string StuDirectorEmail { get; set; }

        #endregion

        /// <summary>
        /// 信息联系人
        /// </summary>
        public string AssociatedWithName { get; set; }

        /// <summary>
        /// 信息联系人手机
        /// </summary>
        public string AssociatedWithCellPhone { get; set; }

        /// <summary>
        /// 信息联系人Email
        /// </summary>
        public string AssociatedWithEmail { get; set; }

        /// <summary>
        /// 其它名称
        /// </summary>
        public string OtherTitle { get; set; }

        /// <summary>
        /// 其它姓名
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
        /// 其它手机
        /// </summary>
        public string OtherCellPhone { get; set; }

        /// <summary>
        /// 其它Email
        /// </summary>
        public string OtherEmail { get; set; }
    }
}

