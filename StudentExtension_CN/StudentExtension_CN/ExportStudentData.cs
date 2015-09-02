using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.ComponentModel;
using System.IO;
using K12.Data;
using StudentExtension_CN.DAO;

namespace StudentExtension_CN
{
    public class ExportStudentData
    {
        List<string> _StudentIDList;
        BackgroundWorker _bgWorker;
        List<StudentData> _StudentDataList;
        Workbook _wb;

        public ExportStudentData(List<string> StudentIDList)
        {
            _bgWorker = new BackgroundWorker();
            _StudentDataList = new List<StudentData>();
            _bgWorker.DoWork += _bgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            _StudentIDList = StudentIDList;
        
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_wb != null)
            {
                Utiltiy.CompletedXls("学生基本资料", _wb);
            }
        }

        public void Start()
        {
            _bgWorker.RunWorkerAsync();
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudentDataList.Clear();
            // 讀取樣版
            _wb = new Workbook();
            _wb.Open(new MemoryStream(Properties.Resources.StudentData));

            // 讀取學生資料
            List<StudentRecord> StudRecList = Student.SelectByIDs(_StudentIDList);

            // 讀取班級資訊
            List<ClassRecord> ClassRecList = Class.SelectAll();
            Dictionary<string, string> ClassNameIDict = new Dictionary<string, string>();
            foreach (ClassRecord cr in ClassRecList)
                ClassNameIDict.Add(cr.ID, cr.Name);
            
            // 讀取父母資料
            List<ParentRecord> ParentRecordList = Parent.SelectByStudentIDs(_StudentIDList);
            Dictionary<string, ParentRecord> ParentRecordDict = new Dictionary<string, ParentRecord>();
            foreach (ParentRecord pr in ParentRecordList)
                ParentRecordDict.Add(pr.RefStudentID, pr);

            // 讀取地址
            List<AddressRecord> AddressRecordList = Address.SelectByStudentIDs(_StudentIDList);
            Dictionary<string, AddressRecord> AddressRecordDict = new Dictionary<string, AddressRecord>();
            foreach (AddressRecord ar in AddressRecordList)
                AddressRecordDict.Add(ar.RefStudentID, ar);

            // 填值
            foreach (StudentRecord sr in StudRecList)
            {
                StudentData sd = new StudentData();
                sd.StudentID = sr.ID;
                sd.StudentNumber = sr.StudentNumber;
                sd.Birthday = sr.Birthday;                
                sd.Gender = sr.Gender;
                sd.LoginName = sr.SALoginName;
                sd.Name = sr.Name;
                sd.SeatNo = sr.SeatNo;

                // 班級
                if (ClassNameIDict.ContainsKey(sr.RefClassID))
                    sd.ClassName = ClassNameIDict[sr.RefClassID];

                // 父母親資料
                if (ParentRecordDict.ContainsKey(sr.ID))
                {
                    sd.FatherName = ParentRecordDict[sr.ID].FatherName;
                    sd.FatherPhone = ParentRecordDict[sr.ID].FatherPhone;
                    sd.MotherName = ParentRecordDict[sr.ID].MotherName;
                    sd.MotherPhone = ParentRecordDict[sr.ID].MotherPhone;                
                }
                
                // 地址
                if (AddressRecordDict.ContainsKey(sr.ID))
                {
                    sd.Address = AddressRecordDict[sr.ID].MailingAddress;
                }
                _StudentDataList.Add(sd);
            }

            // 依學號排序
            _StudentDataList = (from data in _StudentDataList orderby data.StudentNumber ascending select data).ToList();
                        

            // 寫入 Excel
            //學生系統編號,學號,班級,座號,姓名,性別,生日,登入帳號,父親姓名,父親電話,母親姓名,母親電話,聯絡地址
            int RowIdx=1;
            foreach (StudentData sd in _StudentDataList)
            {
                _wb.Worksheets[0].Cells[RowIdx, 0].PutValue(sd.StudentID);
                _wb.Worksheets[0].Cells[RowIdx, 1].PutValue(sd.StudentNumber);
                _wb.Worksheets[0].Cells[RowIdx, 2].PutValue(sd.ClassName);
                if(sd.SeatNo.HasValue)
                    _wb.Worksheets[0].Cells[RowIdx, 3].PutValue(sd.SeatNo.Value);
                _wb.Worksheets[0].Cells[RowIdx, 4].PutValue(sd.Name);
                _wb.Worksheets[0].Cells[RowIdx, 5].PutValue(sd.Gender);
                if(sd.Birthday.HasValue)
                    _wb.Worksheets[0].Cells[RowIdx, 6].PutValue(sd.Birthday.Value.ToShortDateString());
                _wb.Worksheets[0].Cells[RowIdx, 7].PutValue(sd.LoginName);
                _wb.Worksheets[0].Cells[RowIdx, 8].PutValue(sd.FatherName);
                _wb.Worksheets[0].Cells[RowIdx, 9].PutValue(sd.FatherPhone);
                _wb.Worksheets[0].Cells[RowIdx, 10].PutValue(sd.MotherName);
                _wb.Worksheets[0].Cells[RowIdx, 11].PutValue(sd.MotherPhone);
                _wb.Worksheets[0].Cells[RowIdx, 12].PutValue(sd.Address);

                RowIdx++;
            }

            _wb.Worksheets[0].AutoFitColumns();
        }
    }
}
