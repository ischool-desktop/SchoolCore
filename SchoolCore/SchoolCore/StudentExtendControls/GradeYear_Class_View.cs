using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace SchoolCore.StudentExtendControls
{
    public partial class GradeYear_Class_View : NavView
    {
        public GradeYear_Class_View()
        {
            InitializeComponent();
            NavText = "班級檢視";

            Class.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);
            SourceChanged += new EventHandler(GradeYear_Class_View_SourceChanged);
        }

        void GradeYear_Class_View_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
        }

        void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            Layout(mPrimaryKeys);
        }

        #region NavView 成員

        private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();
        private DevComponents.AdvTree.AdvTree advTree1;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private List<string> mPrimaryKeys = new List<string>();

        public new void Layout(List<string> PrimaryKeys)
        {
            //選取的結點的完整路徑
            mPrimaryKeys = PrimaryKeys;

            List<string> selectPath = new List<string>();
            #region 記錄選取的結點的完整路徑
            var selectNode = advTree1.SelectedNode;
            if (selectNode != null)
            {
                while (selectNode != null)
                {
                    selectPath.Insert(0, selectNode.Text);
                    selectNode = selectNode.Parent;
                }
            }
            #endregion
            advTree1.Nodes.Clear();
            items.Clear();


            SortedList<int?, List<string>> gradeYearList = new SortedList<int?, List<string>>();
            List<string> nullGradeList = new List<string>();
            List<string> nullClassList = new List<string>();
            Dictionary<ClassRecord, List<string>> classList = new Dictionary<ClassRecord, List<string>>();
            Dictionary<ClassRecord, int?> classGradeYear = new Dictionary<ClassRecord, int?>();
            List<ClassRecord> classes = new List<ClassRecord>();

            DevComponents.AdvTree.Node rootNode = new DevComponents.AdvTree.Node();

            rootNode.Text = "所有學生(" + PrimaryKeys.Count + ")";

            foreach (var key in PrimaryKeys)
            {
                var studentRec = Student.Instance.Items[key];
                ClassRecord classRec = studentRec.Class;
                string gradeYear = (classRec == null ? "" : classRec.GradeYear);

                //JHSchool.C001
                //if (User.Acl["JHSchool.C001"].PermissionString != gradeYear)
                //    continue;

                int gyear = 0;
                int? g;
                if (int.TryParse(gradeYear, out gyear))
                {
                    g = gyear;
                    if (!gradeYearList.ContainsKey(g))
                        gradeYearList.Add(g, new List<string>());
                    gradeYearList[g].Add(key);
                }
                else
                {
                    g = null;
                    nullGradeList.Add(key);
                }
                if (classRec != null)
                {
                    if (!classList.ContainsKey(classRec))
                    {
                        classList.Add(classRec, new List<string>());
                        classes.Add(classRec);
                    }
                    classList[classRec].Add(key);
                    if (!classGradeYear.ContainsKey(classRec))
                        classGradeYear.Add(classRec, g);
                }
                else
                    nullClassList.Add(key);
            }
            classes.Sort();

            foreach (var gyear in gradeYearList.Keys)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();
                switch (gyear)
                {
                    //case 1:
                    //    gyearNode.Text = "一年級";
                    //    break;
                    //case 2:
                    //    gyearNode.Text = "二年級";
                    //    break;
                    //case 3:
                    //    gyearNode.Text = "三年級";
                    //    break;
                    //case 4:
                    //    gyearNode.Text = "四年級";
                    //    break;
                    default:
                        gyearNode.Text = "" + gyear + "年級";
                        break;

                }

                gyearNode.Text += "(" + gradeYearList[gyear].Count + ")";

                items.Add(gyearNode, gradeYearList[gyear]);

                rootNode.Nodes.Add(gyearNode);

                foreach (var classRec in classes)
                {
                    if (classGradeYear[classRec] == gyear)
                    {
                        DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                        classNode.Text = classRec.Name + "(" + classList[classRec].Count + ")";

                        items.Add(classNode, classList[classRec]);

                        gyearNode.Nodes.Add(classNode);
                    }
                }
            }
            if (nullGradeList.Count > 0)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();

                gyearNode.Text = "未分年級(" + nullGradeList.Count + ")";

                items.Add(gyearNode, nullGradeList);

                rootNode.Nodes.Add(gyearNode);

                foreach (var classRec in classes)
                {
                    if (classGradeYear[classRec] == null)
                    {
                        DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                        classNode.Text = classRec.Name + "(" + classList[classRec].Count + ")";

                        items.Add(classNode, classList[classRec]);

                        gyearNode.Nodes.Add(classNode);
                    }
                }
                if (nullClassList.Count > 0)
                {
                    DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                    classNode.Text = "未分班(" + nullClassList.Count + ")";

                    items.Add(classNode, nullClassList);

                    gyearNode.Nodes.Add(classNode);
                }
            }

            rootNode.Expand();

            advTree1.Nodes.Add(rootNode);

            items.Add(rootNode, PrimaryKeys);

            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }


            //advTree1.Focus();
        }

        private DevComponents.AdvTree.Node SelectNode(List<string> selectPath, int level, DevComponents.AdvTree.NodeCollection nodeCollection)
        {
            foreach (var item in nodeCollection)
            {
                if (item is DevComponents.AdvTree.Node)
                {
                    var node = (DevComponents.AdvTree.Node)item;
                    if (node.Text == selectPath[level])
                    {
                        if (selectPath.Count - 1 == level)
                            return node;
                        else
                        {
                            var childNode = SelectNode(selectPath, level + 1, node.Nodes);
                            if (childNode == null)
                                return node;
                            else
                                return childNode;
                        }
                    }
                }
            }
            return null;
        }

        #endregion
        private void advTree1_AfterNodeSelect(object sender, DevComponents.AdvTree.AdvTreeNodeEventArgs e)
        {

        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {

        }

        private void advTree1_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {

        }

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
            this.advTree1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
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
            this.advTree1.Size = new System.Drawing.Size(224, 446);
            this.advTree1.Styles.Add(this.elementStyle1);
            this.advTree1.TabIndex = 2;
            this.advTree1.Text = "advTree1";
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
            this.elementStyle1.Class = "";
            this.elementStyle1.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // GradeYear_Class_View
            // 
            this.Controls.Add(this.advTree1);
            this.Name = "GradeYear_Class_View";
            ((System.ComponentModel.ISupportInitialize)(this.advTree1)).EndInit();
            this.ResumeLayout(false);

        }

        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
    }
}
