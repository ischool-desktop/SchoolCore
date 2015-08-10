using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;

namespace SchoolCore.Legacy.ImportSupport
{
    public interface IValidatorFactory : IFieldValidatorFactory, IRowValidatorFactory
    {
    }
}
