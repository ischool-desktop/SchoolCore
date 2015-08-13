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
            NavText = "班导师检视";

            SourceChanged += new EventHandler(SuperviseView_SourceChanged);
        }

        private void SuperviseView_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
        }

        #region NavView 成员

        private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();
        public new void Layout(List<string> PrimaryKeys)
        {
            //选取的结点的完整路径
            List<string> selectPath = new List<string>();
            #region 记录选取的结点的完整路径
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

            // 取得删除教师 ID
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
                // 过滤删除教师
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
                    //    gyearNode.Text = "一年级";
                    //    break;
                    //case 2:
                    //    gyearNode.Text = "二年级";
                    //    break;
                    //case 3:
                    //    gyearNode.Text = "三年级";
                    //    break;
                    //case 4:
                    //    gyearNode.Text = "四年级";
                    //    break;
                    default:
                        gyearNode.Text = "" + gyear + "年级";
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
                gyearNode.Text = "未分年级" + "(" + noGradYearList.Count + ")";
                items.Add(gyearNode, noGradYearList);

                rootNode.Nodes.Add(gyearNode);
            }



            rootNode.Text = "所有班导师(" + TotalCount + ")";

            rootNode.Expand();

            advTree1.Nodes.Add(rootNode);
            // 这里在加入所有班导师 
            teacherKeys.Clear();

            //foreach (List<string> str in gradeYearList.Values)
            //    teacherKeys.AddRange(str);
            // 有年级
            foreach (string str in tmp)
                teacherKeys.Add(str);
            // 无年级
            foreach (string str in noGradYearList)
                if (!teacherKeys.Contains(str))
                    teacherKeys.Add(str);

            TotalCount = teacherKeys.Count;

            rootNode.Text = "所有班导师(" + TotalCount + ")";

            //items.Add(rootNode, PrimaryKeys);
            items.Add(rootNode, teacherKeys);
            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }


            // 加入删除教师
            if (checkDeletedTeacheIDList.Count > 0)
            {

                //TotalCount += DeletedTeacheIDList.Count;
                DevComponents.AdvTree.Node DelTeacheNode = new DevComponents.AdvTree.Node();
                DelTeacheNode.Text = "删除教师" + "(" + checkDeletedTeacheIDList.Count + ")";
                items.Add(DelTeacheNode, checkDeletedTeacheIDList);
                advTree1.Nodes.Add(DelTeacheNode);
                //rootNode.Nodes.Add(DelTeacheNode);
            }

            // 非班导师
            // 是班导师ID

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
            NotClassTeacherNode.Text = "非班导师 (" + NotClassTeacherID.Count + ")";
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
            if (e.Node != null)
            {
                bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], SelectedAll, AddToTemp);
            }
            else
            {
                SetListPaneSource(new List<string>(), false, false);
            }
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], selAll, addToTemp);
            }
            catch (Exception) { }
        }

        private void advTree1_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], selAll, addToTemp);
            }
            catch (Exception) { }
        }

    }

}

