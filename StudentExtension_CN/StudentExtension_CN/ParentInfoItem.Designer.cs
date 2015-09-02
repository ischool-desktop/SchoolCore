namespace StudentExtension_CN
{
    partial class ParentInfoItem
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParentInfoItem));
            this.lb1 = new DevComponents.DotNetBar.LabelX();
            this.txtFatherName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtFatherPhone = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtMotherPhone = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.txtMotherName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.txtAddress = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.SuspendLayout();
            // 
            // lb1
            // 
            resources.ApplyResources(this.lb1, "lb1");
            // 
            // 
            // 
            this.lb1.BackgroundStyle.Class = "";
            this.lb1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lb1.Name = "lb1";
            // 
            // txtFatherName
            // 
            // 
            // 
            // 
            this.txtFatherName.Border.Class = "TextBoxBorder";
            this.txtFatherName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            resources.ApplyResources(this.txtFatherName, "txtFatherName");
            this.txtFatherName.Name = "txtFatherName";
            // 
            // txtFatherPhone
            // 
            // 
            // 
            // 
            this.txtFatherPhone.Border.Class = "TextBoxBorder";
            this.txtFatherPhone.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            resources.ApplyResources(this.txtFatherPhone, "txtFatherPhone");
            this.txtFatherPhone.Name = "txtFatherPhone";
            // 
            // labelX1
            // 
            resources.ApplyResources(this.labelX1, "labelX1");
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Name = "labelX1";
            // 
            // txtMotherPhone
            // 
            // 
            // 
            // 
            this.txtMotherPhone.Border.Class = "TextBoxBorder";
            this.txtMotherPhone.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            resources.ApplyResources(this.txtMotherPhone, "txtMotherPhone");
            this.txtMotherPhone.Name = "txtMotherPhone";
            // 
            // labelX2
            // 
            resources.ApplyResources(this.labelX2, "labelX2");
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Name = "labelX2";
            // 
            // txtMotherName
            // 
            // 
            // 
            // 
            this.txtMotherName.Border.Class = "TextBoxBorder";
            this.txtMotherName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            resources.ApplyResources(this.txtMotherName, "txtMotherName");
            this.txtMotherName.Name = "txtMotherName";
            // 
            // labelX3
            // 
            resources.ApplyResources(this.labelX3, "labelX3");
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Name = "labelX3";
            // 
            // labelX4
            // 
            resources.ApplyResources(this.labelX4, "labelX4");
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Name = "labelX4";
            // 
            // txtAddress
            // 
            // 
            // 
            // 
            this.txtAddress.Border.Class = "TextBoxBorder";
            this.txtAddress.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            resources.ApplyResources(this.txtAddress, "txtAddress");
            this.txtAddress.Name = "txtAddress";
            // 
            // ParentInfoItem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.txtMotherPhone);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtMotherName);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.txtFatherPhone);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txtFatherName);
            this.Controls.Add(this.lb1);
            this.Name = "ParentInfoItem";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lb1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFatherName;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFatherPhone;
        private DevComponents.DotNetBar.Controls.TextBoxX txtMotherPhone;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtMotherName;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.TextBoxX txtAddress;
    }
}
