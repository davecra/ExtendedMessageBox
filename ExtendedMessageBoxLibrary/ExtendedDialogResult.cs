using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtendedMessageBoxLibrary
{
    public class ExtendedDialogResult
    {
        /// <summary>
        /// Which button the user clicked in the dialog.
        /// </summary>
        public DialogResult Result { get; private set; }
        /// <summary>
        /// Will be true if the user checked the checkbox
        /// </summary>
        public bool IsChecked { get; private set; }
        /// <summary>
        /// Will be true if the dialog timed out on its own
        /// </summary>
        public bool TimedOut { get; private set; }
        /// <summary>
        /// The constructor for the Extended dialog result object
        /// </summary>
        /// <param name="result">The dialog result from the form</param>
        /// <param name="ischecked">The state of the checkbox on the form</param>
        /// <param name="timedout">Whether the form closed itslef or not</param>
        internal ExtendedDialogResult(DialogResult result, bool ischecked, bool timedout)
        {
            Result = result;
            IsChecked = ischecked;
            TimedOut = timedout;
        }
    }
}
