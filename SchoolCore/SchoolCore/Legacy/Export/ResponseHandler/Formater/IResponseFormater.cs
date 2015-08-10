using System;
using System.Collections.Generic;
using System.Text;
using SchoolCore.Legacy.Export.RequestHandler;
using System.Xml;

namespace SchoolCore.Legacy.Export.ResponseHandler.Formater
{
    public interface IResponseFormater
    {
        ExportFieldCollection Format(XmlElement element);
    }
}
