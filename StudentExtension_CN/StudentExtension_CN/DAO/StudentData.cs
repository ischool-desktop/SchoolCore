using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentExtension_CN.DAO
{
    /// <summary>
    /// 學生資料
    /// </summary>
    public class StudentData
    {
        public string StudentID { get; set; }

        public string Name { get; set; }

        public string StudentNumber { get; set; }

        public string Gender { get; set; }

        public string LoginName { get; set; }

        public string ClassName  { get; set; }

        public int? SeatNo { get; set; }

        public DateTime? Birthday { get; set; }

        public string FatherName { get; set; }

        public string FatherPhone { get; set; }

        public string MotherName { get; set; }

        public string MotherPhone { get; set; }

        public string Address { get; set; }

    }
}
