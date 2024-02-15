using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public enum FieldDataType
    {
        DateTime = 0,
        String = 1,
        Short = 2,
        Int = 3
    }

    public enum ControlType
    {
        Dropdown = 0,
        DatePicker = 1,
        Toggle = 2,
        Textbox = 3
    }
}
