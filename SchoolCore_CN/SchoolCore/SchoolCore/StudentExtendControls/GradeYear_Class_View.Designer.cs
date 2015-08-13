namespace SchoolCore.StudentExtendControls
{
    partial class GradeYear_Class_View
    {
        /// <summary> 
        /// 设计工具所需的变数。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的资源。
        /// </summary>
        /// <param name="disposing">如果应该处置 Managed 资源则为 true，否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计工具产生的程序代码

        /// <summary> 
        /// 此为设计工具支持所需的方法 - 请勿使用程序代码编辑器修改这个方法的内容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.advTree1 = new DevComponents.AdvTree.AdvTree();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            ((System.ComponentModel.ISupportInitialize)(this.advTree1)).BeginInit();
            this.SuspendLayout();
            // 
            // advTree1
            // 
            this.advTree1.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advTree1.AllowDrop = true;
            this.advTree1.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advTree1.BackgroundStyle.Class = "TreeBorderKey";
            this.advTree1.Columns.Add(this.columnHeader1);
            this.advTree1.ColumnsVisible = false;
            this.advTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advTree1.DragDropEnabled = false;
            this.advTree1.ExpandWidth = 16;
            this.advTree1.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.advTree1.Location = new System.Drawing.Point(0, 0);
            this.advTree1.Name = "advTree1";
            this.advTree1.NodesConnector = this.nodeConnector1;
            this.advTree1.NodeStyle = this.elementStyle1;
            this.advTree1.PathSeparator = ";";
            this.advTree1.Size = new System.Drawing.Size(338, 514);
            this.advTree1.Styles.Add(this.elementStyle1);
            this.advTree1.TabIndex = 1;
            this.advTree1.Text = "advTree1";
            this.advTree1.NodeDoubleClick += new DevComponents.AdvTree.TreeNodeMouseEventHandler(this.advTree1_NodeDoubleClick);
            this.advTree1.AfterNodeSelect += new DevComponents.AdvTree.AdvTreeNodeEventHandler(this.advTree1_AfterNodeSelect);
            this.advTree1.NodeClick += new DevComponents.AdvTree.TreeNodeMouseEventHandler(this.advTree1_NodeClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Width.Relative = 100;
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // GradeYear_Class_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.advTree1);
            this.Name = "GradeYear_Class_View";
            this.Size = new System.Drawing.Size(338, 514);
            ((System.ComponentModel.ISupportInitialize)(this.advTree1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree advTree1;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
    }
}

