using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SchoolCore.Legacy.ImportSupport
{
    public  interface IDataAccess
    {
        XmlElement GetImportFieldList();

        XmlElement GetValidateFieldRule();

        XmlElement GetUniqueFieldData();

        XmlElement GetShiftCheckList(params string[] fieldNameList);

        void InsertImportData(XmlElement data);

        void UpdateImportData(XmlElement data);
    }
}
