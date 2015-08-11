using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using System.Xml;
using FISCA.Authentication;

namespace SchoolCore.Feature
{
    public static class EditStudent
    {
        public static void SaveStudentRecordEditor(IEnumerable<SchoolCore.Editor.StudentRecordEditor> editors)
        {
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateStudentList");
            DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");
            List<LogInfo> logs = new List<LogInfo>();
            List<string> reflashList = new List<string>();
            bool hasUpdate = false;
            foreach (var editor in editors)
            {
                if (editor.EditorStatus == SchoolCore.Editor.EditorStatus.Update)
                {
                    #region 更新
                    LogInfo log = new LogInfo() { Action = "修改學生資料", Entity = "Student", EntityID = editor.ID };
                    string description = "";
                    updateHelper.AddElement("Student");
                    updateHelper.AddElement("Student", "Field");
                    if (editor.Birthday != editor.Student.Birthday)
                    {
                        updateHelper.AddElement("Student/Field", "Birthdate", editor.Birthday);
                        description += "生日 由\"" + editor.Student.Birthday + "\"變更為\"" + editor.Birthday + "\"";
                    }
                    if (editor.Gender != editor.Student.Gender)
                    {
                        updateHelper.AddElement("Student/Field", "Gender", editor.Gender);
                        description += "性別 由\"" + editor.Student.Gender + "\"變更為\"" + editor.Gender + "\"";
                    }
                    if (editor.IDNumber != editor.Student.IDNumber)
                    {
                        updateHelper.AddElement("Student/Field", "IDNumber", editor.IDNumber);
                        description += "身分證號 由\"" + editor.Student.IDNumber + "\"變更為\"" + editor.IDNumber + "\"";
                    }
                    if (editor.Name != editor.Student.Name)
                    {
                        updateHelper.AddElement("Student/Field", "Name", editor.Name);
                        description += "姓名 由\"" + editor.Student.Name + "\"變更為\"" + editor.Name + "\"";
                    }
                    if (editor.SeatNo != editor.Student.SeatNo)
                    {
                        updateHelper.AddElement("Student/Field", "SeatNo", editor.SeatNo);
                        description += "座號 由\"" + editor.Student.SeatNo + "\"變更為\"" + editor.SeatNo + "\"";
                    }
                    if (editor.Status != editor.Student.Status)
                    {
                        updateHelper.AddElement("Student/Field", "Status", editor.Status);
                        description += "狀態 由\"" + editor.Student.Status + "\"變更為\"" + editor.Status + "\"";
                    }
                    if (editor.StudentNumber != editor.Student.StudentNumber)
                    {
                        updateHelper.AddElement("Student/Field", "StudentNumber", editor.StudentNumber);
                        description += "學號 由\"" + editor.Student.StudentNumber + "\"變更為\"" + editor.StudentNumber + "\"";
                    }
                    if (editor.RefClassID != editor.Student.RefClassID)
                    {
                        updateHelper.AddElement("Student/Field", "RefClassID", editor.RefClassID);
                        description += "班級 由\"" + (editor.Student.Class == null ? "" : editor.Student.Class.Name) + "\"變更為\"" + (SchoolCore.Class.Instance.Items[editor.RefClassID] == null ? "" : SchoolCore.Class.Instance.Items[editor.RefClassID].Name) + "\"";
                    }
                    //if (editor.OverrideDepartmentID != editor.Student.OverrideDepartmentID)
                    //{
                    //    updateHelper.AddElement("Student/Field", "OverrideDeptID", editor.OverrideDepartmentID);
                    //    description += "指定科別 由\"" + (JHSchool.Department.Instance[editor.Student.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.Student.OverrideDepartmentID].FullName) + "\"變更為\"" + (JHSchool.Department.Instance[editor.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.OverrideDepartmentID].FullName) + "\"";
                    //}
                    if (editor.Nationality != editor.Student.Nationality)
                    {
                        updateHelper.AddElement("Student/Field", "Nationality", editor.Nationality);
                        description += "國籍 由\"" + editor.Student.Nationality + "\"變更為\"" + editor.Nationality + "\"";
                    }


                    //天啊，這個 Log 怎麼寫比較好。

                    //if (editor.OverrideProgramPlanID != editor.Student.OverrideProgramPlanID)
                    //{
                    //    updateHelper.AddElement("Student/Field", "RefGraduationPlanID", editor.OverrideProgramPlanID);
                    //    description += "指定課程規劃 由\"" + (JHSchool.Department.Instance[editor.Student.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.Student.OverrideDepartmentID].FullName) + "\"變更為\"" + (JHSchool.Department.Instance[editor.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.OverrideDepartmentID].FullName) + "\"";
                    //}
                    updateHelper.AddElement("Student/Field", "RefGraduationPlanID", "" + editor.OverrideProgramPlanID);
                    updateHelper.AddElement("Student/Field", "RefScoreCalcRuleID", "" + editor.OverrideScoreCalcRuleID);

                    updateHelper.AddElement("Student", "Condition");
                    updateHelper.AddElement("Student/Condition", "ID", editor.ID);
                    log.Description = description;
                    logs.Add(log);
                    hasUpdate = true;
                    reflashList.Add(editor.ID);
                    #endregion
                    #region 新增
                    if (editor.EditorStatus == SchoolCore.Editor.EditorStatus.Insert)
                    {
                        throw new NotImplementedException("新增學生尚未實作");
                    }
                    #endregion
                }
                if (hasUpdate)
                {
                    DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(updateHelper.BaseElement));
                    logs.SaveAll();
                    Student.Instance.SyncDataBackground(reflashList);
                }
            }
        }
  
        internal static string AddStudent(string name)
        {
            string sid = "";
            DSXmlHelper req = new DSXmlHelper("InsertRequest");

            req.AddElement("Student");
            req.AddElement("Student", "Field");
            //req.AddElement("Student", "Field");
            req.AddElement("Student/Field", "Name", name);
            DSXmlHelper rsp = DSAServices.CallService("SmartSchool.Student.Insert", new DSRequest(req.BaseElement)).GetContent();
            foreach (XmlElement xm in rsp.GetElements("NewID"))
                sid = xm.InnerText;
            return sid;

            //SmartSchool.student.insert
            //Student.Instance.SyncDataBackground()
        }
        internal static void DelStudent(string StudentID)
        {

            DSXmlHelper req = new DSXmlHelper("UpdateStudentList");
            req.AddElement("Student");
            req.AddElement("Student", "Field");
            req.AddElement("Student/Field", "Status", "刪除");
            req.AddElement("Student", "Condition");
            req.AddElement("Student/Condition", "ID", StudentID);
            DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(req.BaseElement));
        }

        //internal static void SaveStudentTagRecordEditor(IEnumerable<SchoolCore.Editor.StudentTagRecordEditor> editors)
        //{
        //    MultiThreadWorker<SchoolCore.Editor.StudentTagRecordEditor> worker = new MultiThreadWorker<SchoolCore.Editor.StudentTagRecordEditor>();
        //    worker.MaxThreads = 3;
        //    worker.PackageSize = 100;
        //    worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<SchoolCore.Editor.StudentTagRecordEditor> e)
        //    {
        //        DSXmlHelper updateHelper = new DSXmlHelper("Request");
        //        DSXmlHelper insertHelper = new DSXmlHelper("Request");
        //        DSXmlHelper deleteHelper = new DSXmlHelper("Request");
        //        List<string> synclist = new List<string>(); //這個目前沒作用
        //        bool hasInsert = false, hasDelete = false;

        //        foreach (var editor in e.List)
        //        {
        //            #region 更新
        //            if (editor.EditorStatus == SchoolCore.Editor.EditorStatus.Update)
        //            {
        //                deleteHelper.AddElement("Tag");
        //                deleteHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
        //                deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

        //                hasDelete = true;
        //                synclist.Add(editor.RefEntityID);

        //                insertHelper.AddElement("Tag");
        //                insertHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
        //                insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

        //                hasInsert = true;
        //            }
        //            #endregion

        //            #region 新增
        //            if (editor.EditorStatus == SchoolCore.Editor.EditorStatus.Insert)
        //            {
        //                insertHelper.AddElement("Tag");
        //                insertHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
        //                insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

        //                hasInsert = true;
        //            }
        //            #endregion

        //            #region 刪除
        //            if (editor.EditorStatus == SchoolCore.Editor.EditorStatus.Delete)
        //            {
        //                deleteHelper.AddElement("Tag");
        //                deleteHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
        //                deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

        //                hasDelete = true;
        //                synclist.Add(editor.RefEntityID);
        //            }
        //            #endregion
        //        }

        //        if (hasInsert)
        //        {
        //            DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.AddStudentTag", new DSRequest(insertHelper.BaseElement)).GetContent();
        //            foreach (XmlElement each in response.GetElements("NewID"))
        //                synclist.Add(each.InnerText);
        //        }

        //        if (hasDelete)
        //            DSAServices.CallService("SmartSchool.Tag.RemoveStudentTag", new DSRequest(deleteHelper.BaseElement));
        //    };
        //    List<PackageWorkEventArgs<SchoolCore.Editor.StudentTagRecordEditor>> packages = worker.Run(editors);
        //    foreach (PackageWorkEventArgs<SchoolCore.Editor.StudentTagRecordEditor> each in packages)
        //    {
        //        if (each.HasException)
        //            throw each.Exception;
        //    }
        //}
    }
}
