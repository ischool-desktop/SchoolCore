using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolCore
{
    public class CourseTagRecord : GeneralTagRecord
    {
        protected override string GetEntityID(System.Xml.XmlElement data)
        {
            return data.SelectSingleNode("CourseID").InnerText;
        }

        public CourseRecord Course { get { return JHSchool.Course.Instance[RefEntityID]; } }
    }
}
