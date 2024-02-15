using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class FieldDetailAttribute : Attribute
    {
        #region Properties

        public FieldDataType DataType { get; private set; }

        public ControlType ControlType { get; private set; }

        public string DataSourceName { get; private set; }


        #endregion

        #region Ctor

        public FieldDetailAttribute(FieldDataType dataType = FieldDataType.String, string DataSource = "", ControlType controlType = ControlType.Textbox)
        {
            DataType = dataType;
            DataSourceName = DataSource;
            ControlType = controlType;
        }

        #endregion
    }
}
