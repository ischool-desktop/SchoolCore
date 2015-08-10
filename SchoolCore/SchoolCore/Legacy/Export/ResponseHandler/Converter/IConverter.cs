using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolCore.Legacy.Export.ResponseHandler.Converter
{
    public interface IConverter
    {
        string Convert(string value);
    }
}
