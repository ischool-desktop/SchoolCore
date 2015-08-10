using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using SchoolCore.Legacy.Export.RequestHandler.Formater;
using SchoolCore.Legacy.Export.RequestHandler.Generator;
using SchoolCore.Legacy.Export.RequestHandler;
using SchoolCore.Legacy.Export.Util;
using SchoolCore.Legacy.Export.RequestHandler.Generator.Condition;
using SchoolCore.Legacy.Export.RequestHandler.Generator.Orders;
using SchoolCore.Legacy.Export.ResponseHandler.Formater;
using SchoolCore.Feature.Legacy;

namespace SchoolCore.Legacy.Export.ResponseHandler.Connector
{
    public class ExportStudentConnector : IExportConnector
    {
        //private DSConnection _connection;
        private FieldCollection _selectFields;
        // �@�w�� ID
        private FieldCollection _selectFieldsID;

        private List<string> _conditions;

        public ExportStudentConnector()
        {
            _conditions = new List<string>();
        }

        #region IExportConnector ����

        public void SetSelectedFields(FieldCollection fields)
        {
            _selectFields = fields;
        }

        public void AddCondition(string studentid)
        {
            _conditions.Add(studentid);
        }



        public ExportTable Export()
        {
            // ���o������Ӫ�
            XmlElement schoolLocationList = Config.GetSchoolLocationList().GetContent().BaseElement;

            // ���o�ץX�W�h�y�z
            XmlElement descElement = StudentBulkProcess.GetExportDescription();
            IFieldFormater fieldFormater = new BaseFieldFormater();
            IResponseFormater responseFormater = new ResponseFormater();

            FieldCollection fieldCollection = fieldFormater.Format(descElement);
            ExportFieldCollection exportFields = responseFormater.Format(descElement);



            fieldCollection = FieldUtil.Match(fieldCollection, _selectFields);
            exportFields = FieldUtil.Match(exportFields, _selectFields);

            //// ���窱�A�ɥ[�J
            //if (_selectFields.FindByDisplayText("���A") != null)
            //{
            //    fieldCollection.Add(_selectFields.FindByDisplayText("���A"));
            //    ExportField ex = new ExportField();
            //    ex.DisplayText = "���A";
            //    ex.RequestName = "StudentStatus";
            //    ex.ColumnIndex = exportFields.Length;
            //    ex.DataType = "";
            //    ex.XPath = "";
            //    exportFields.Add(ex);

            //}


            IRequestGenerator reqGenerator = new ExportStudentRequestGenerator();
            
            _selectFieldsID = new FieldCollection();
            foreach (Field fd in _selectFields)
                _selectFieldsID.Add(fd);

            if (_selectFieldsID.Find("StudentID") == null)
            {
                Field fd1 = new Field();
                fd1.FieldName = "StudentID";
                fd1.DisplayText = "�ǥͨt�νs��";
                _selectFieldsID.Add(fd1);
            }
            reqGenerator.SetSelectedFields(_selectFieldsID);

            // �w�]��-1, ���M�|�Ǧ^�Ҧ��ǥ�
            ICondition condition = new BaseCondition("ID", "-1");
            reqGenerator.AddCondition(condition);
            foreach (string id in _conditions)
            {
                ICondition condition2 = new BaseCondition("ID", id);
                reqGenerator.AddCondition(condition2);
            }
            
            reqGenerator.AddOrder(new Order("GradeYear"));
            reqGenerator.AddOrder(new Order("Department"));
            reqGenerator.AddOrder(new Order("RefClassID"));
            reqGenerator.AddOrder(new Order("SeatNo"));

            DSRequest request = reqGenerator.Generate();
            DSResponse response = QueryStudent.GetExportList(request);

            ExportTable table = new ExportTable();

                       
            
            foreach (ExportField field in exportFields)
                table.AddColumn(field);

            //// ���o�ǥͪ��A
            //Dictionary<string, string> StudStatusDic = new Dictionary<string, string>();
            //foreach (JHSchool.Data.JHStudentRecord stud in JHSchool.Data.JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource ))
            //    StudStatusDic.Add(stud.ID, stud.Status.ToString());            

            foreach (XmlElement record in response.GetContent().GetElements("Student"))
            {
               
                ExportRow row = table.AddRow();
                foreach (ExportField column in table.Columns)
                {
                    int columnIndex = column.ColumnIndex;
                    ExportCell cell = row.Cells[columnIndex];

                    XmlNode cellNode = record.SelectSingleNode(column.XPath);

                    //if(column.DisplayText !="���A")
                    //    cellNode = record.SelectSingleNode(column.XPath);
                    // CustodianOtherInfo/CustodianOtherInfo[1]/EducationDegree[1]

                    #region �o�q�{���O�B�z�פJ/�ץX�{�����@�P���D
                    if (column.XPath.StartsWith("CustodianOtherInfo/Custodian"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("CustodianOtherInfo/Custodian", "CustodianOtherInfo/CustodianOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("CustodianOtherInfo/CustodianOtherInfo", "CustodianOtherInfo/Custodian");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    if (column.XPath.StartsWith("FatherOtherInfo/Father"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("FatherOtherInfo/Father", "FatherOtherInfo/FatherOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("FatherOtherInfo/FatherOtherInfo", "FatherOtherInfo/Father");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    if (column.XPath.StartsWith("MotherOtherInfo/Mother"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("MotherOtherInfo/Mother", "MotherOtherInfo/MotherOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("MotherOtherInfo/MotherOtherInfo", "MotherOtherInfo/Mother");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    #endregion

                    if (cellNode != null)
                    {
                        if (column.FieldName == "GraduateSchoolLocationCode")
                            cell.Value = GetCounty(schoolLocationList, cellNode.InnerText);
                        else if (column.FieldName == "DeptName") //�B�z��O�~�Ӱ��D�C
                        {
                            //�o����쪺��Ƥ@�w�|�Q�^�ǡA�]���]�w�F Mandatory �ݩʡC
                            XmlNode selfDept = record.SelectSingleNode("SelfDeptName");
                            if (string.IsNullOrEmpty(selfDept.InnerText))
                                cell.Value = cellNode.InnerText;
                            else
                                cell.Value = selfDept.InnerText;
                        }
                        else if (column.FieldName == "Status")
                        { 
                            cell.Value =GetStudStatusStr(cellNode.InnerText );
                        }
                        else
                            cell.Value = cellNode.InnerText;
                    }

                    //if (column.DisplayText == "���A")//record.SelectSingleNode("StudentID")!=null )
                    //{
                    //    // �ǥͪ��A
                    //    if (StudStatusDic.ContainsKey(record.SelectSingleNode("StudentID").InnerText))
                    //        cell.Value = StudStatusDic[record.SelectSingleNode("StudentID").InnerText];
                    //}

                }
            }
            return table;
        }

        
        // ���o�ǥͪ��A�W��
        private string GetStudStatusStr(string code)
        {
            string retValue = string.Empty;

            if (code == "1")
                retValue = "�@��";

            if (code == "4")
                retValue = "���";

            if (code == "8")
                retValue = "����";

            if (code == "16")
                retValue = "���~������";

            if (code == "256")
                retValue = "�R��";

            return retValue;
        
        }

        private string GetCounty(XmlElement list, string code)
        {
            foreach (XmlNode node in list.SelectNodes("Location"))
            {
                XmlElement element = (XmlElement)node;
                if (element.GetAttribute("Code") == code)
                    return element.InnerText;
            }
            return string.Empty;
        }

        #endregion
    }
}
