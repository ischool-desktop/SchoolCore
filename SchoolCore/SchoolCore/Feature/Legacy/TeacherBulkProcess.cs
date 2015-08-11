using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace SmartSchool.Feature.Teacher
{
    public class TeacherBulkProcess
    {
        [AutoRetryOnWebException()]
        public static XmlElement GetExportDescription()
        {
            //Ū��XML���y�z
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SchoolCore.Properties.Resources.JH_T_ExportDescription);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetExportDescription");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetBulkDescription()
        {
            //Ū��XML���y�z
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SchoolCore.Properties.Resources.JH_T_BulkDescription);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetBulkDescription");
        }

        #region 2008/04/02 �Юv�פJ��g�A���եΡA���_

        public static XmlElement GetImportFieldList()
        {
            //Ū��XML���y�z
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SchoolCore.Properties.Resources.JH_T_ImportFieldList);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetImportFieldList");
        }

        public static XmlElement GetUniqueFieldData()
        {
            return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetUniqueFieldData");
        }

        public static XmlElement GetShiftCheckList(params string[] fieldList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Teacher.BulkProcessJH.GetShiftCheckList";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        #endregion

        private static XmlElement CallNoneRequestService(string serviceName)
        {
            string strServiceName = serviceName;
            DSResponse rsp = DSAServices.CallService(serviceName, new DSRequest());

            if (rsp.GetContent() == null)
                throw new Exception("�A�ȥ��^�ǥ�������T�C(" + strServiceName + ")");

            return rsp.GetContent().BaseElement;
        }

        [AutoRetryOnWebException()]
        public static DSResponse GetExportList(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Teacher.BulkProcessJH.Export", request);
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetFieldValidationRule()
        {
            //Ū��XML���y�z
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SchoolCore.Properties.Resources.JH_T_FieldValidationRule);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetFieldValidationRule");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetPrimaryKeyList()
        {
            return CallNoneRequestService("SmartSchool.Teacher.BulkProcessJH.GetPrimaryKeyList");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetShiftCheckList(string key, string value)
        {
            DSXmlHelper request = new DSXmlHelper("GetShiftCheckList");
            request.AddElement(key);
            request.AddElement(value);

            return DSAServices.CallService("SmartSchool.Teacher.BulkProcessJH.GetShiftCheckList", new DSRequest(request)).GetContent().BaseElement;
        }

        public static void InsertImportTeacher(XmlElement request)
        {
            DSAServices.CallService("SmartSchool.Teacher.BulkProcessJH.InsertImportTeacher", new DSRequest(request));
        }

        [AutoRetryOnWebException()]
        public static void UpdateImportTeacher(XmlElement request)
        {
            DSAServices.CallService("SmartSchool.Teacher.BulkProcessJH.UpdateImportTeacher", new DSRequest(request));
        }
    }
}
