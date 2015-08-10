using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SchoolCore.Legacy.Export.RequestHandler.Formater
{
    public interface IFieldFormater
    {
        FieldCollection Format(XmlElement element);
    }
}
