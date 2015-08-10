using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolCore
{
    public class ClassTagRecord : GeneralTagRecord
    {
        protected override string GetEntityID(System.Xml.XmlElement data)
        {
            return data.SelectSingleNode("ClassID").InnerText;
        }

        public ClassRecord Class { get { return JHSchool.Class.Instance[RefEntityID]; } }
    }
}
