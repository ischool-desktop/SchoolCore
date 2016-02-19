using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Framework;
using FISCA.Presentation;
using System.Windows.Forms;
using SchoolCore;
using System.Text.RegularExpressions;
using SchoolCore.TeacherExtendControls.Ribbon;
using Framework.Security;
using System.Data;

namespace SchoolCore
{
    public class Teacher : LegacyPresentBase<TeacherRecord>
    {
        private static Teacher _Instance = null;
        public static Teacher Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Teacher(K12.Presentation.NLDPanels.Teacher);
                return _Instance;
            }
        }

        public new void AddDetailBulider(IDetailBulider item)
        {
            DetailContent content = item.GetContent();
            if (content == null) return;

            if (Attribute.IsDefined(content.GetType(), typeof(FeatureCodeAttribute)))
            {
                FeatureCodeAttribute fca = Attribute.GetCustomAttribute(content.GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
                if (fca != null)
                {
                    if (Framework.Legacy.GlobalOld.Acl[content.GetType()].Viewable)
                        base.AddDetailBulider(item);
                }
            }
            else base.AddDetailBulider(item);
        }

        public void SetupPresentation()
        {
            Teacher.Instance.RibbonBarItems["编辑"].Index = 0;
            Teacher.Instance.RibbonBarItems["资料统计"].Index = 1;

            #region RibbonBar 教师/编辑
            RibbonBarItem rbItem = Teacher.Instance.RibbonBarItems["编辑"];
            rbItem["新增"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["新增"].Image = TeacherExtendControls.Ribbon.Resources.btnAddTeacher;
            rbItem["新增"].Enable = User.Acl["JHSchool.Teacher.Ribbon0000"].Executable;
            rbItem["新增"].Click += delegate
            {
                new SchoolCore.TeacherExtendControls.Ribbon.AddTeacher().ShowDialog();
            };

            rbItem["删除"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["删除"].Image = TeacherExtendControls.Ribbon.Resources.btnDeleteTeacher;
            rbItem["删除"].Enable = User.Acl["JHSchool.Teacher.Ribbon0010"].Executable;
            rbItem["删除"].Click += delegate
            {
                if (SelectedList.Count == 1)
                {
                    K12.Data.TeacherRecord thRec = K12.Data.Teacher.SelectByID(SelectedList[0].ID);
                    thRec.Status = K12.Data.TeacherRecord.TeacherStatus.刪除;

                    string msg = string.Format("确定要删除「{0}」？", thRec.Name);
                    if (FISCA.Presentation.Controls.MsgBox.Show(msg, "删除教师", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        K12.Data.Teacher.Update(thRec);
                        Teacher.Instance.SyncDataBackground(thRec.ID);
                        PermRecLogProcess prlp = new PermRecLogProcess();
                        prlp.SaveLog("学籍.教师", "删除教师", "删除教师,姓名:" + thRec.Name);
                    }
                    else
                        return;
                }
            };


            ListPaneField idField = new ListPaneField("ID");
            idField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = e.Key;
            };
            //this.AddListPaneField(idField);

            ListPaneField nameField = new ListPaneField("姓名");
            nameField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].Name;
            };
            AddListPaneField(nameField);

            ListPaneField nicknameField = new ListPaneField("昵称");
            nicknameField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].Nickname;
            };
            AddListPaneField(nicknameField);

            ListPaneField TeacherLoginName = new ListPaneField("登入账号");
            TeacherLoginName.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].TALoginName;
            };
            AddListPaneField(TeacherLoginName);

            ListPaneField genderField = new ListPaneField("性别");
            genderField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].Gender;
            };
            AddListPaneField(genderField);


            ListPaneField superviseClassField = new ListPaneField("带班班级");
            superviseClassField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                var superviseClass = "";
                foreach (var item in Class.Instance.GetTecaherSupervisedClass(Items[e.Key]))
                {
                    superviseClass += (superviseClass == "" ? "" : "、") + item.Name;
                }

                e.Value = superviseClass;
            };
            Teacher.Instance.AddListPaneField(superviseClassField);

            // 加入身份证号,联络电话的权限管理
            ListPaneField IDNumberField = new ListPaneField("身份证号");
            IDNumberField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].IDNumber;
            };
            if (User.Acl["Teacher.Field.身份证号"].Executable)
                Present.AddListPaneField(IDNumberField);

            Catalog ribbonField = RoleAclSource.Instance["教师"]["清单字段"];
            ribbonField.Add(new RibbonFeature("Teacher.Field.身份证号", "身份证号"));

            ListPaneField telField = new ListPaneField("联络电话");
            telField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                e.Value = Items[e.Key].ContactPhone;
            };
            if (User.Acl["Teacher.Field.联络电话"].Executable)
                Present.AddListPaneField(telField);

            ribbonField = RoleAclSource.Instance["教师"]["清单字段"];
            ribbonField.Add(new RibbonFeature("Teacher.Field.联络电话", "联络电话"));

            //Teacher.Instance.AddView(new JHSchool.TeacherExtendControls.SubjectView());
            Teacher.Instance.AddView(new SchoolCore.TeacherExtendControls.SuperviseView());
            //Teacher.Instance.AddView(new JHSchool.TeacherExtendControls.CategoryView());
            Present.NavPaneContexMenu.GetChild("重新整理").Click += delegate { this.SyncAllBackground(); };

            Teacher.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<SchoolCore.TeacherExtendControls.BaseInfoItem>());

            #endregion
            #region 增加导师搜寻条件钮

            ConfigData cd = User.Configuration["TeacherSearchOptionPreference"];

            SearchTeacherName = SearchConditionMenu["姓名"];
            SearchTeacherName.AutoCheckOnClick = true;
            SearchTeacherName.AutoCollapseOnClick = false;
            SearchTeacherName.Checked = cd.GetBoolean("SearchTeacherName", true);
            SearchTeacherName.Click += delegate
            {
                cd.SetBoolean("SearchTeacherName", SearchTeacherName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchTeacherRefId = SearchConditionMenu["身份证号"];
            SearchTeacherRefId.AutoCheckOnClick = true;
            SearchTeacherRefId.AutoCollapseOnClick = false;
            SearchTeacherRefId.Checked = cd.GetBoolean("SearchTeacherRefId", true);
            SearchTeacherRefId.Click += delegate
            {
                cd.SetBoolean("SearchTeacherRefId", SearchTeacherRefId.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchTeacherLoginID = SearchConditionMenu["登入账号"];
            SearchTeacherLoginID.AutoCheckOnClick = true;
            SearchTeacherLoginID.AutoCollapseOnClick = false;
            SearchTeacherLoginID.Checked = cd.GetBoolean("SearchTeacherLoginID", true);
            SearchTeacherLoginID.Click += delegate
            {
                cd.SetBoolean("SearchTeacherLoginID", SearchTeacherLoginID.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            Present.Search += new EventHandler<SearchEventArgs>(Teacher_Search);




            #endregion

            #region 注册权限管理
            Catalog ribbon = RoleAclSource.Instance["教师"]["功能按钮"];
            ribbon.Add(new RibbonFeature("JHSchool.Teacher.Ribbon0000", "新增教师资料"));
            ribbon.Add(new RibbonFeature("JHSchool.Teacher.Ribbon0010", "删除教师资料"));


            Catalog detail = RoleAclSource.Instance["教师"]["数据项"];
            detail.Add(new DetailItemFeature(typeof(TeacherExtendControls.BaseInfoItem)));

            #endregion

            // 建立筛选
            foreach (string each in AllStatus)
                CreateFilterItem(each);

            bool havedef = false;
            foreach (string each in AllStatus)
                havedef |= FilterMenu[each].Checked;

            if (!havedef)
                FilterMenu["一般"].Checked = true;


            MotherForm.AddPanel(K12.Presentation.NLDPanels.Teacher);
            _Initilized = true;
            UseFilter = true;

            FillFilter();
        }

        private bool _Initilized = false;
        private Teacher(NLDPanel present)
            : base(present)
        {

        }

        #region 教师搜寻主功能

        private MenuButton SearchTeacherName, SearchTeacherRefId, SearchTeacherLoginID;

        void Teacher_Search(object sender, SearchEventArgs e)
        {
            try
            {
                List<TeacherRecord> TeacherList = new List<TeacherRecord>(Teacher.Instance.Items);
                Dictionary<string, TeacherRecord> results = new Dictionary<string, TeacherRecord>();
                Regex rx = new Regex(e.Condition, RegexOptions.IgnoreCase);

                if (SearchTeacherName.Checked)
                {
                    foreach (TeacherRecord each in TeacherList)
                    {
                        string nameAndNickname = (each.Name != null) ? each.Name : "";
                        nameAndNickname += (each.Nickname != null) ? each.Nickname : "";
                        if (rx.Match(nameAndNickname).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                if (SearchTeacherRefId.Checked)
                {
                    foreach (TeacherRecord each in TeacherList)
                    {
                        string name = (each.IDNumber != null) ? each.IDNumber : "";
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                if (SearchTeacherLoginID.Checked)
                {
                    #region 取得老师ID与LoginName

                    FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
                    DataTable dr = _queryHelper.Select("select id,st_login_name from teacher where st_login_name is not null");

                    Dictionary<string, TeacherRecord> teacherDict = new Dictionary<string, TeacherRecord>();
                    foreach (TeacherRecord rec in TeacherList)
                        teacherDict.Add(rec.ID, rec);

                    foreach (DataRow row in dr.Rows)
                    {
                        string id = "" + row[0];
                        string loginName = "" + row[1];

                        if (rx.Match(loginName).Success)
                        {
                            if (teacherDict.ContainsKey(id))
                            {
                                if (!results.ContainsKey(id))
                                    results.Add(id, teacherDict[id]);
                            }
                        }
                    }

                    #endregion
                }

                e.Result.AddRange(results.Values.AsKeyList());
            }
            catch (Exception) { }
        }


        #endregion

        #region 教师筛选 SubFunction
        ConfigData preference = User.Configuration["TeacherDefaultFiltersPreference"];

        private void CreateFilterItem(string name)
        {
            FilterMenu[name].AutoCheckOnClick = true;
            FilterMenu[name].AutoCollapseOnClick = false;
            FilterMenu[name].Checked = preference.GetBoolean(name, false);
            FilterMenu[name].Click += delegate
            {
                FillFilter();
                preference.SetBoolean(name, FilterMenu[name].Checked);

                preference.SaveAsync();
            };
        }

        private string[] AllStatus
        {
            get { return new string[] { "一般", "删除" }; }
        }


        #endregion

        protected override Dictionary<string, TeacherRecord> GetAllData()
        {
            Dictionary<string, TeacherRecord> items = new Dictionary<string, TeacherRecord>();
            foreach (var item in Feature.QueryTeacher.GetAllTeachers())
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        protected override Dictionary<string, TeacherRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, TeacherRecord> items = new Dictionary<string, TeacherRecord>();
            foreach (var item in Feature.QueryTeacher.GetTeachers(primaryKeys))
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        protected override void FillFilter()
        {
            //数据加载中或数据未加载或画面没有设定完成就什么都不做
            if (!_Initilized || !Loaded) return;

            // old
            //List<string> primaryKeys = new List<string>();
            //foreach (var item in Items)
            //{
            //    primaryKeys.Add(item.ID);
            //}

            // New add
            List<string> primaryKeys = new List<string>();
            List<string> filters = new List<string>();

            foreach (string each in AllStatus)
                if (FilterMenu[each].Checked)
                    filters.Add(each);


            foreach (K12.Data.TeacherRecord item in K12.Data.Teacher.SelectAll())
            {
                string str = item.Status.ToString();
                if (filters.Contains(str))
                    if (!primaryKeys.Contains(item.ID))
                        primaryKeys.Add(item.ID);

            }

            Present.SetFilteredSource(primaryKeys);
        }

        protected override List<string> AsKeyList(List<TeacherRecord> list)
        {
            return list.AsKeyList();
        }
    }

    public static class Teacher_Extends
    {
        /// <summary>
        /// 读取教师的系统编号转换成 List。
        /// </summary>
        public static List<string> AsKeyList(this IEnumerable<TeacherRecord> teachers)
        {
            List<string> keys = new List<string>();
            foreach (TeacherRecord each in teachers)
                keys.Add(each.ID);

            return keys;
        }
    }
}

