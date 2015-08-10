﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using FISCA.Authentication;
using K12.Data;

namespace SchoolCore.InternalExtendControls.Tagging
{
    /// <summary>
    /// 管理 Tag 的表單(學生、班級、教師、課程共用的 Parent)。
    /// </summary>
    internal partial class TagForm : BaseForm
    {

        //        private Dictionary<string, TagRecordEditor> Editors = new Dictionary<string, TagRecordEditor>();
        private Dictionary<string, TagConfigRecord> Editors = new Dictionary<string, TagConfigRecord>();
        private const string AllTagText = "<顯示所有類別>";

        public TagForm()
        {
            InitializeComponent();

            if (DSAServices.IsLogined)
            {
                RefreshTagData();
                TagConfig.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(TagConfig_AfterDelete);
                TagConfig.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(TagConfig_AfterInsert);
                TagConfig.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(TagConfig_AfterUpdate);
            }
        }

        void TagConfig_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            foreach (string each in e.PrimaryKeys)
            {
                TagConfigRecord rec = TagConfig.SelectByID(each);

                if (rec != null)
                {
                    if (Editors.ContainsKey(each))
                        Editors[each] = rec;
                    else
                        Editors.Add(each, rec);
                }
                else
                {
                    if (Editors.ContainsKey(each))
                        Editors.Remove(each);
                    else
                        throw new ArgumentException(string.Format("Tag 錯誤，編號：{0}", each));
                }
            }

            RefreshTagData();
        }

        void TagConfig_AfterInsert(object sender, K12.Data.DataChangedEventArgs e)
        {
            foreach (string each in e.PrimaryKeys)
            {
                TagConfigRecord rec = TagConfig.SelectByID(each);

                if (rec != null)
                {
                    if (Editors.ContainsKey(each))
                        Editors[each] = rec;
                    else
                        Editors.Add(each, rec);
                }
                else
                {
                    if (Editors.ContainsKey(each))
                        Editors.Remove(each);
                    else
                        throw new ArgumentException(string.Format("Tag 錯誤，編號：{0}", each));
                }
            }

            RefreshTagData();
        }

        void TagConfig_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            foreach (string each in e.PrimaryKeys)
            {
                TagConfigRecord rec = TagConfig.SelectByID(each);

                if (rec != null)
                {
                    if (Editors.ContainsKey(each))
                        Editors[each] = rec;
                    else
                        Editors.Add(each, rec);
                }
                else
                {
                    if (Editors.ContainsKey(each))
                        Editors.Remove(each);
                    else
                        throw new ArgumentException(string.Format("Tag 錯誤，編號：{0}", each));
                }
            }

            RefreshTagData();
        }

        /// <summary>
        /// 重新讀取 Tag 資料，會自動呼叫 RefreshPrefixList、RefreshTagNameList。
        /// </summary>
        private void RefreshTagData()
        {
            List<TagConfigRecord> records = TagConfig.SelectByCategory(Category);
            Editors = new Dictionary<string, TagConfigRecord>();
            foreach (TagConfigRecord each in records)
                Editors.Add(each.ID, each);

            RefreshPrefixList();
            RefreshTagNameList();
            SchoolCore.Tag.Instance.SyncAllBackground();
        }

        private void TagForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //SchoolCore.Tag.Instance.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);
        }

        protected virtual K12.Data.TagCategory Category { get { throw new NotImplementedException(); } }

        protected virtual void DoDelete(TagConfigRecord record)
        {
            throw new NotImplementedException();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTagData();
        }

        private void cboGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTagNameList();
        }

        private void RefreshPrefixList()
        {
            string origin_selected = cboGroup.Text;

            List<string> prefixes = new List<string>();

            prefixes.Add(AllTagText);
            foreach (TagConfigRecord each in TagConfig.SelectByCategory(Category))
            {
                if (!prefixes.Contains(each.Prefix))
                    prefixes.Add(each.Prefix);
            }

            cboGroup.Items.Clear();
            prefixes.Sort();
            cboGroup.Items.AddRange(prefixes.ToArray());

            int selIndex = cboGroup.FindString(origin_selected);
            if (selIndex == -1)
                cboGroup.SelectedIndex = (cboGroup.Items.Count > 0 ? 0 : -1);
            else
                cboGroup.SelectedIndex = selIndex;
        }

        private void RefreshTagNameList()
        {
            string origin_selected_name = CurrentSelectedName;

            DGV.Rows.Clear();

            string prefix = cboGroup.Text;

            List<TagConfigRecord> EditorsList = new List<TagConfigRecord>();

            foreach (TagConfigRecord each in Editors.Values)
                EditorsList.Add(each);
            EditorsList.Sort(new Comparison<TagConfigRecord>(TagConfigRecordFullNameSorter));

            foreach (TagConfigRecord each in EditorsList)
            {
                if (each.Prefix == prefix || prefix == AllTagText)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(DGV, new TagColor(each.Color).Image, each.FullName);
                    row.Tag = each;

                    DGV.Rows.Add(row);

                    if (each.FullName == origin_selected_name)
                        row.Selected = true;
                }
            }
        }

        // 排序用
        private int TagConfigRecordFullNameSorter(TagConfigRecord x, TagConfigRecord y)
        {
            return x.FullName.CompareTo(y.FullName);
        }

        /// <summary>
        /// 取得畫面上使用者所選擇的 Tag 資料。
        /// </summary>
        private TagConfigRecord CurrentSelectedTag
        {
            get
            {
                if (DGV.SelectedRows.Count > 0)
                    return DGV.SelectedRows[0].Tag as TagConfigRecord;
                else
                    return null;
            }
        }

        /// <summary>
        /// 目前選擇到的 Tag 全名。
        /// </summary>
        private string CurrentSelectedName
        {
            get
            {
                if (CurrentSelectedTag != null)
                    return CurrentSelectedTag.FullName;
                else
                    return string.Empty;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TagEditor.InsertTag(Category);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (CurrentSelectedTag != null)
                DoDelete(CurrentSelectedTag);
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (CurrentSelectedTag != null)
                TagEditor.ModifyTag(CurrentSelectedTag);
        }
    }
}