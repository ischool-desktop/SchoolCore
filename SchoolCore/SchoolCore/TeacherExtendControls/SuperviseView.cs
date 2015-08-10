﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace SchoolCore.TeacherExtendControls
{
    public partial class SuperviseView : NavView
    {
        List<string> teacherKeys = new List<string>();

        public SuperviseView()
        {
            InitializeComponent();
            NavText = "班導師檢視";

            SourceChanged += new EventHandler(SuperviseView_SourceChanged);
        }

        private void SuperviseView_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
        }

        #region NavView 成員

        private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();
        public new void Layout(List<string> PrimaryKeys)
        {
            //選取的結點的完整路徑
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

            // 取得刪除教師 ID
            List<string> DeletedTeacheIDList = new List<string>();
            List<string> checkDeletedTeacheIDList = new List<string>();
            List<K12.Data.TeacherRecord> TeacherRecs = K12.Data.Teacher.SelectAll();
            foreach (K12.Data.TeacherRecord tr in TeacherRecs)
                if (tr.Status == K12.Data.TeacherRecord.TeacherStatus.刪除)
                    DeletedTeacheIDList.Add(tr.ID);

            SortedList<int?, List<string>> gradeYearList = new SortedList<int?, List<string>>();
            List<string> noGradYearList = new List<string>();

            DevComponents.AdvTree.Node rootNode = new DevComponents.AdvTree.Node();

            int TotalCount = 0;

            foreach (var key in PrimaryKeys)
            {
                // 過濾刪除教師
                if (DeletedTeacheIDList.Contains(key))
                {
                    checkDeletedTeacheIDList.Add(key);
                    continue;
                }
                var teacherRec = Teacher.Instance.Items[key];

                foreach (var classRec in Class.Instance.GetTecaherSupervisedClass(teacherRec))
                {
                    string gradeYear = (classRec == null ? "" : classRec.GradeYear);

                    int gyear = 0;
                    int? g;
                    if (int.TryParse(gradeYear, out gyear))
                    {
                        g = gyear;
                        if (!gradeYearList.ContainsKey(g))
                            gradeYearList.Add(g, new List<string>());

                        if (!gradeYearList[g].Contains(key))
                            gradeYearList[g].Add(key);
                    }
                    else
                    {
                        g = null;

                        if (!noGradYearList.Contains(key))
                            noGradYearList.Add(key);
                    }
                }
            }

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
                // TotalCount += gradeYearList[gyear].Count;
                gyearNode.Text += "(" + gradeYearList[gyear].Count + ")";
                items.Add(gyearNode, gradeYearList[gyear]);
                rootNode.Nodes.Add(gyearNode);
            }

            List<string> tmp = new List<string>();

            foreach (List<string> strList in gradeYearList.Values)
            {
                foreach (string str in strList)
                    if (!tmp.Contains(str))
                        tmp.Add(str);
            }



            if (noGradYearList.Count > 0)
            {
                //                TotalCount += noGradYearList.Count;
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();
                gyearNode.Text = "未分年級" + "(" + noGradYearList.Count + ")";
                items.Add(gyearNode, noGradYearList);

                rootNode.Nodes.Add(gyearNode);
            }



            rootNode.Text = "所有班導師(" + TotalCount + ")";

            rootNode.Expand();

            advTree1.Nodes.Add(rootNode);
            // 這裡在加入所有班導師 
            teacherKeys.Clear();

            //foreach (List<string> str in gradeYearList.Values)
            //    teacherKeys.AddRange(str);
            // 有年級
            foreach (string str in tmp)
                teacherKeys.Add(str);
            // 無年級
            foreach (string str in noGradYearList)
                if (!teacherKeys.Contains(str))
                    teacherKeys.Add(str);

            TotalCount = teacherKeys.Count;

            rootNode.Text = "所有班導師(" + TotalCount + ")";

            //items.Add(rootNode, PrimaryKeys);
            items.Add(rootNode, teacherKeys);
            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }


            // 加入刪除教師
            if (checkDeletedTeacheIDList.Count > 0)
            {

                //TotalCount += DeletedTeacheIDList.Count;
                DevComponents.AdvTree.Node DelTeacheNode = new DevComponents.AdvTree.Node();
                DelTeacheNode.Text = "刪除教師" + "(" + checkDeletedTeacheIDList.Count + ")";
                items.Add(DelTeacheNode, checkDeletedTeacheIDList);
                advTree1.Nodes.Add(DelTeacheNode);
                //rootNode.Nodes.Add(DelTeacheNode);
            }

            // 非班導師
            // 是班導師ID

            List<string> isClassTeacherID = new List<string>();
            List<string> NotClassTeacherID = new List<string>();
            foreach (K12.Data.ClassRecord classRec in K12.Data.Class.SelectAll())
            {
                isClassTeacherID.Add(classRec.RefTeacherID);
            }

            foreach (K12.Data.TeacherRecord teachRec in K12.Data.Teacher.SelectAll())
            {

                if (!isClassTeacherID.Contains(teachRec.ID))
                {
                    if (teachRec.Status == K12.Data.TeacherRecord.TeacherStatus.一般)
                        NotClassTeacherID.Add(teachRec.ID);
                }
            }
            DevComponents.AdvTree.Node NotClassTeacherNode = new DevComponents.AdvTree.Node();
            NotClassTeacherNode.Text = "非班導師 (" + NotClassTeacherID.Count + ")";
            items.Add(NotClassTeacherNode, NotClassTeacherID);
            advTree1.Nodes.Add(NotClassTeacherNode);

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
            this.node1 = new DevComponents.AdvTree.Node();
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
            this.advTree1.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.advTree1.Location = new System.Drawing.Point(3, 0);
            this.advTree1.Name = "advTree1";
            this.advTree1.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.node1});
            this.advTree1.NodesConnector = this.nodeConnector1;
            this.advTree1.NodeStyle = this.elementStyle1;
            this.advTree1.PathSeparator = ";";
            this.advTree1.Size = new System.Drawing.Size(135, 446);
            this.advTree1.Styles.Add(this.elementStyle1);
            this.advTree1.TabIndex = 0;
            this.advTree1.Text = "advTree1";
            // 
            // node1
            // 
            this.node1.Expanded = true;
            this.node1.Name = "node1";
            this.node1.Text = "node1";
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
            // SuperviseView
            // 
            this.Controls.Add(this.advTree1);
            this.Name = "SuperviseView";
            ((System.ComponentModel.ISupportInitialize)(this.advTree1)).EndInit();
            this.ResumeLayout(false);

        }

        private DevComponents.AdvTree.AdvTree advTree1;
        private DevComponents.AdvTree.Node node1;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;

    }
}
