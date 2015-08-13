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

namespace SchoolCore
{
    public partial class UserInfoManager : FISCA.Presentation.Controls.BaseForm
    {
        private ErrorProvider _errorProvider;
        public UserInfoManager()
        {
            InitializeComponent();
            _errorProvider = new ErrorProvider();

            //lblUserid.Text = "【 " + CurrentUser.Instance.UserName + " 】";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DoubleCheck();
            bool valid = true;
            foreach (Control control in Controls)
            {
                if (_errorProvider.GetError(control) != string.Empty)
                    valid = false;
            }
            if (!valid)
            {
                MsgBox.Show("密码数据有误，请先修正后再行储存！");
                return;
            }

            try
            {
                //计算密码杂~!
                ChangePassword(PasswordHash.Compute(txtPassword.Text));
            }
            catch (Exception ex)
            {
                MsgBox.Show("密码变更失败 :" + ex.Message);
                return;
            }
            string accesspoint = FISCA.Authentication.DSAServices.AccessPoint;
            string username = FISCA.Authentication.DSAServices.UserAccount;
            //string accesspoint = CurrentUser.Instance.AccessPoint;
            //string username = CurrentUser.Instance.UserName;
            try
            {

                //CurrentUser.Instance.SetConnection(accesspoint, username, txtPassword.Text);
                //CurrentUser.Instance.SetConnection(accesspoint, username, txtPassword.Text);
            }
            catch (Exception ex)
            {
                MsgBox.Show("重新建立联机失败 : " + ex.Message);
                return;
            }
            MsgBox.Show("密码变更完成！");
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("核心", "修改", "修改用户账号密码.");
            this.Close();
        }

        private void DoubleCheck()
        {
            _errorProvider.SetError(txtConfirm, string.Empty);
            if (txtConfirm.Text == string.Empty)
                _errorProvider.SetError(txtConfirm, "请输入确认密码！");
            else if (txtConfirm.Text != txtPassword.Text)
                _errorProvider.SetError(txtConfirm, "确认密码与新密码不符！");
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPassword_Validated(object sender, EventArgs e)
        {
            _errorProvider.SetError(txtPassword, string.Empty);
            if (txtPassword.Text == string.Empty)
                _errorProvider.SetError(txtPassword, "密码不可空白！");

            else if (txtPassword.Text.Length < 4)
                _errorProvider.SetError(txtPassword, "密码长度不可少于4码！");
        }

        public void ChangePassword(string newPassword)
        {
            DSXmlHelper helper = new DSXmlHelper("ChangePassword");
            helper.AddElement("CurrentUser");
            helper.AddElement("CurrentUser", "NewPassword", newPassword);
            helper.AddElement("CurrentUser", "Condition");
            //helper.AddElement("CurrentUser/Condition", "UserName", CurrentUser.Instance.UserName.ToUpper());
            helper.AddElement("CurrentUser/Condition", "UserName", FISCA.Authentication.DSAServices.UserAccount.ToUpper());
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Personal.ChangePassword", new DSRequest(helper));

        }

        private void UserInfoManager_Load(object sender, EventArgs e)
        {
            lblUserid.Text = "【 " + FISCA.Authentication.DSAServices.UserAccount + " 】";
        }

    }
}

