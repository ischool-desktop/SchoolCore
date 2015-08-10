using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using SchoolCore.Legacy.Export.RequestHandler;

namespace SchoolCore.Legacy.Export.ResponseHandler.Connector
{
    public interface IExportConnector
    {      
        void SetSelectedFields(FieldCollection exportFields);
        void AddCondition(string argument);        
        ExportTable Export();
    }
}
