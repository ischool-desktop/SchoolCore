using FISCA;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentExtension_CN
{
    public class Program
    {
        [ApplicationMain()]
        static public void Main()
        {
            // 班級資訊
           SchoolCore.Student.Instance.AddDetailBulider(new DetailBulider<StudentExtension_CN.ClassItem>());
        }
    }
}
