using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace SchoolCore.ClassExtendControls
{
    public partial class Class_View : NavView
    {
        public Class_View()
        {
            InitializeComponent();
            this.NavText = "班级检视";
            SourceChanged += Class_View_SourceChanged;
        }

        void Class_View_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
        }

        #region NavView 成员
        Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();

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

            //用来记录年级及班级对应的数据结构，第一维记录年级，第二维记录年级下的班级编号
            SortedList<int?, List<string>> gradeYearList = new SortedList<int?, List<string>>();

            //用来记录未分类的班级编号
            List<string> nullGradeList = new List<string>();

            DevComponents.AdvTree.Node rootNode = new DevComponents.AdvTree.Node();

            rootNode.Text = "所有班级(" + PrimaryKeys.Count + ")";

            //取得所有班级编号
            foreach (var key in PrimaryKeys)
            {
                //根据学生记录取得班级记录
                ClassRecord classRec = Class.Instance[key];

                //根据班级记录取得年级，若是年级为null则年级为空白
                string gradeYear = (classRec == null ? "" : classRec.GradeYear);
                int gyear = 0;
                int? g;

                //将gradeYear转型成int
                if (int.TryParse(gradeYear, out gyear))
                {
                    g = gyear;
                    if (!gradeYearList.ContainsKey(g))
                        gradeYearList.Add(g, new List<string>());

                    //将班级编号加入所属年级的集合当中
                    gradeYearList[g].Add(key);
                }
                else
                {
                    //加入没有分类的班级
                    g = null;
                    nullGradeList.Add(key);
                }
            }

            foreach (var gyear in gradeYearList.Keys)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();
                gyearNode.Text = "" + gyear + "年级";

                gyearNode.Text += "(" + gradeYearList[gyear].Count + ")";

                items.Add(gyearNode, gradeYearList[gyear]);

                rootNode.Nodes.Add(gyearNode);
            }

            if (nullGradeList.Count > 0)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();
                gyearNode.Text = "未分年级(" + nullGradeList.Count + ")";
                items.Add(gyearNode, nullGradeList);

                rootNode.Nodes.Add(gyearNode);
            }

            advTree1.Nodes.Add(rootNode);

            rootNode.Expand();

            items.Add(rootNode, PrimaryKeys);

            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }
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

