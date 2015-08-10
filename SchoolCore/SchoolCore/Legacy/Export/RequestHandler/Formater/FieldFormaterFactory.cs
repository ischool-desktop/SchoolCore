using System;
using System.Collections.Generic;
using System.Text;
using SchoolCore.Legacy.Export.RequestHandler.Generator;

namespace SchoolCore.Legacy.Export.RequestHandler.Formater
{
    public class FieldFormaterFactory
    {
        public static IFieldFormater CreateInstance(ExportType type)
        {
            switch (type)
            {
                case ExportType.ExportStudent:
                    return new BaseFieldFormater();
                default:
                    return new BaseFieldFormater();
            }
        }
    }
}
