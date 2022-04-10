using FormTools.FormDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.UI
{
    public interface IElementView
    {
        ElementDescriptor RawDescriptor { get; }
        event FieldValueChangedEventHandler FieldValueChanged;

        /// <summary>
        /// Value for this field if the field is on the form
        /// </summary>
        string FieldValue { get; set; }

        /// <summary>
        /// Value for this field if the field is not on the form
        /// </summary>
        string FieldNotShownValue { get; }

        /// <summary>
        /// Whether the field is tickable
        /// </summary>
        bool Tickable { get; }

        /// <summary>
        /// Whether the field is ticked
        /// </summary>
        bool Ticked { get; }

        /// <summary>
        /// Retrieves the "val" value from dropdowns
        /// </summary>
        string FieldValValue { get; }

        /// <summary>
        /// Set the Prepop value of the field
        /// </summary>
        string PrepopValue { set; }

        bool Mandatory { set; get; }

    }

    public delegate void FieldValueChangedEventHandler(IElementView sender, EventArgs args);
}
