using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedMessageBoxLibrary
{
    /// <summary>
    /// Hyperlink helper class for the ExtendedMessageBox class
    /// </summary>
    public class Hyperlink
    {
        public string HyperlinkText { get; private set; }
        public string HyperlinkLinkData { get; private set; }
        /// <summary>
        /// Constructor for Hyperlink class
        /// </summary>
        /// <param name="text">The text to display for the link</param>
        /// <param name="link">The hyperlink path (http/ftp/file)</param>
        public Hyperlink(string text, string link)
        {
            HyperlinkText = text;
            HyperlinkLinkData = link;
        }
    }
}
