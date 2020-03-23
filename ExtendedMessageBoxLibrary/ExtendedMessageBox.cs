using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ExtendedMessageBoxLibrary
{
    /// <summary>
    /// Displays a message to the user in a modal dialog box
    /// blocking other actions in the application until it is
    /// dismissed. This extended messagebox contains everything
    /// provided in the System.Windows Forms.Messagebox dialog
    /// but extends it in two ways: it allows for a chackbox 
    /// and a timeout feature.
    /// </summary>
    public static class ExtendedMessageBox
    {
        #region PINVOKE STUFF FOR ICONS
        private const int MAX_PATH = 260;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("Shell32.dll", SetLastError = false)]
        private static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);
        [Flags]
        private enum SHGSI : uint
        {
            SHGSI_ICONLOCATION = 0,
            SHGSI_ICON = 0x000000100,
            SHGSI_SYSICONINDEX = 0x000004000,
            SHGSI_LINKOVERLAY = 0x000008000,
            SHGSI_SELECTED = 0x000010000,
            SHGSI_LARGEICON = 0x000000000,
            SHGSI_SMALLICON = 0x000000001,
            SHGSI_SHELLICONSIZE = 0x000000004
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public UInt32 cbSize;
            public IntPtr hIcon;
            public Int32 iSysIconIndex;
            public Int32 iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPath;
        }

        private enum SHSTOCKICONID : uint
        {
            SIID_DOCNOASSOC = 0,
            SIID_DOCASSOC = 1,
            SIID_APPLICATION = 2,
            SIID_FOLDER = 3,
            SIID_FOLDEROPEN = 4,
            SIID_DRIVE525 = 5,
            SIID_DRIVE35 = 6,
            SIID_DRIVEREMOVE = 7,
            SIID_DRIVEFIXED = 8,
            SIID_DRIVENET = 9,
            SIID_DRIVENETDISABLED = 10,
            SIID_DRIVECD = 11,
            SIID_DRIVERAM = 12,
            SIID_WORLD = 13,
            SIID_SERVER = 15,
            SIID_PRINTER = 16,
            SIID_MYNETWORK = 17,
            SIID_FIND = 22,
            SIID_HELP = 23,
            SIID_SHARE = 28,
            SIID_LINK = 29,
            SIID_SLOWFILE = 30,
            SIID_RECYCLER = 31,
            SIID_RECYCLERFULL = 32,
            SIID_MEDIACDAUDIO = 40,
            SIID_LOCK = 47,
            SIID_AUTOLIST = 49,
            SIID_PRINTERNET = 50,
            SIID_SERVERSHARE = 51,
            SIID_PRINTERFAX = 52,
            SIID_PRINTERFAXNET = 53,
            SIID_PRINTERFILE = 54,
            SIID_STACK = 55,
            SIID_MEDIASVCD = 56,
            SIID_STUFFEDFOLDER = 57,
            SIID_DRIVEUNKNOWN = 58,
            SIID_DRIVEDVD = 59,
            SIID_MEDIADVD = 60,
            SIID_MEDIADVDRAM = 61,
            SIID_MEDIADVDRW = 62,
            SIID_MEDIADVDR = 63,
            SIID_MEDIADVDROM = 64,
            SIID_MEDIACDAUDIOPLUS = 65,
            SIID_MEDIACDRW = 66,
            SIID_MEDIACDR = 67,
            SIID_MEDIACDBURN = 68,
            SIID_MEDIABLANKCD = 69,
            SIID_MEDIACDROM = 70,
            SIID_AUDIOFILES = 71,
            SIID_IMAGEFILES = 72,
            SIID_VIDEOFILES = 73,
            SIID_MIXEDFILES = 74,
            SIID_FOLDERBACK = 75,
            SIID_FOLDERFRONT = 76,
            SIID_SHIELD = 77,
            SIID_WARNING = 78,
            SIID_INFO = 79,
            SIID_ERROR = 80,
            SIID_KEY = 81,
            SIID_SOFTWARE = 82,
            SIID_RENAME = 83,
            SIID_DELETE = 84,
            SIID_MEDIAAUDIODVD = 85,
            SIID_MEDIAMOVIEDVD = 86,
            SIID_MEDIAENHANCEDCD = 87,
            SIID_MEDIAENHANCEDDVD = 88,
            SIID_MEDIAHDDVD = 89,
            SIID_MEDIABLURAY = 90,
            SIID_MEDIAVCD = 91,
            SIID_MEDIADVDPLUSR = 92,
            SIID_MEDIADVDPLUSRW = 93,
            SIID_DESKTOPPC = 94,
            SIID_MOBILEPC = 95,
            SIID_USERS = 96,
            SIID_MEDIASMARTMEDIA = 97,
            SIID_MEDIACOMPACTFLASH = 98,
            SIID_DEVICECELLPHONE = 99,
            SIID_DEVICECAMERA = 100,
            SIID_DEVICEVIDEOCAMERA = 101,
            SIID_DEVICEAUDIOPLAYER = 102,
            SIID_NETWORKCONNECT = 103,
            SIID_INTERNET = 104,
            SIID_ZIPFILE = 105,
            SIID_SETTINGS = 106,
            SIID_DRIVEHDDVD = 132,
            SIID_DRIVEBD = 133,
            SIID_MEDIAHDDVDROM = 134,
            SIID_MEDIAHDDVDR = 135,
            SIID_MEDIAHDDVDRAM = 136,
            SIID_MEDIABDROM = 137,
            SIID_MEDIABDR = 138,
            SIID_MEDIABDRE = 139,
            SIID_CLUSTEREDDRIVE = 140,
            SIID_MAX_ICONS = 175
        }
        #endregion
        private static int PICTURE_WIDTH = 32;
        private static int PICTURE_HEIGHT = 32;
        private static int BUTTON_WIDTH = 75;
        private static int BUTTON_HEIGHT = 25;
        private static int PADDING = 10;

        private static Form MobjForm = null;
        private static bool MbolChecked = false;
        private static bool MbolTimeout = false;
        private static string MstrMoreText = "";
        private static double MintDPI= 1;
        /// <summary>
        /// Displays a messagebox
        /// </summary>
        /// <param name="text">The text to display in the message box</param>
        /// <param name="caption">Text to display in the title bar of the message box</param>
        /// <param name="buttons">Which buttons to display in the message box</param>
        /// <param name="icon">Which icon to show in the message box</param>
        /// <param name="checkboxtext">Text to display in the checkbox. If left empty, no checkbox will appear.</param>
        /// <param name="hyperlink">A Hyperlink object (text and link), that will appear below the message</param>
        /// <param name="moredata">If provided this will show a More button which will expand the form to show more text</param>
        /// <param name="timeout">A value in seconds until the dialog self dismisses with a dialog result of cancel and timeout as true. No timeout will occur if no value is specified.</param>
        /// <returns>An extended dialog result object wiht a traditional dialog result, the state of the checkbox and whether the form timed out or the user took action.</returns>
        public static ExtendedDialogResult Show(string text,
                                           string caption = "",
                                           MessageBoxButtons buttons = MessageBoxButtons.OK,
                                           MessageBoxIcon icon = MessageBoxIcon.None,
                                           string checkboxtext = "",
                                           Hyperlink hyperlink = null,
                                           string moredata = null,
                                           int timeout = int.MaxValue)
        {
            try
            {
                MobjForm = CreateForm(text, caption, buttons, icon, checkboxtext, hyperlink, moredata, timeout);
                DialogResult LobjResult = MobjForm.ShowDialog();
                ExtendedDialogResult LobjReturnValue = new ExtendedDialogResult(LobjResult, MbolChecked, MbolTimeout);
                return LobjReturnValue;
            }
            catch (Exception PobjEx)
            {
                throw PobjEx; // pass it along
            }
        }

        /// <summary>
        /// Creates the form for display
        /// </summary>
        /// <param name="PstrText"></param>
        /// <param name="PstrTitle"></param>
        /// <param name="PobjButtons"></param>
        /// <param name="PobjIcon"></param>
        /// <param name="PstrCheckboxText"></param>
        /// <param name="PintTimeout"></param>
        /// <returns></returns>
        private static Form CreateForm(string PstrText, string PstrTitle,
                                       MessageBoxButtons PobjButtons,
                                       MessageBoxIcon PobjIcon,
                                       string PstrCheckboxText,
                                       Hyperlink PobjHyperlink,
                                       string PstrMore,
                                       int PintTimeout)
        {
            try
            {
                Form LobjForm = new Form();
                MintDPI = GetDPIFactor(LobjForm);
                LobjForm.StartPosition = FormStartPosition.CenterScreen;
                LobjForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                LobjForm.MinimizeBox = false;
                LobjForm.MaximizeBox = false;
                LobjForm.AutoSize = true;
                LobjForm.Text = ((PintTimeout < int.MaxValue && PintTimeout > 0) ?
                                  PstrTitle + " [" + (PintTimeout).ToString() + "]" :
                                  PstrTitle);
                // Create the panels
                Panel LobjTopPanel = GetTopPanel(PobjIcon, PstrText, PobjHyperlink);
                Panel LobjButtonPanel = GetButtonPanel(PobjButtons, PstrCheckboxText, PstrMore);
                LobjForm.Height = LobjTopPanel.Height + LobjButtonPanel.Height +
                                  (PADDING * 4) + GetTitleBarHeight(LobjForm);
                LobjForm.Width = LobjTopPanel.Width > LobjButtonPanel.Width ?
                                 LobjTopPanel.Width + (PADDING * 2) :
                                 LobjButtonPanel.Width + (PADDING * 2);
                LobjForm.Controls.Add(LobjButtonPanel);
                LobjForm.Controls.Add(LobjTopPanel);
                // On load we need to arrange a few things since we
                // get sized and modified on first paint
                LobjForm.Load += (o, e) => { ArrangeForm(); };
                // create the timer
                if (PintTimeout > 0 && PintTimeout < int.MaxValue)
                {
                    Timer LobjTimer = new Timer();
                    LobjTimer.Interval = 1000;
                    int LintCount = 0;
                    LobjTimer.Tick += (o, e) =>
                    {
                        LintCount++;
                        if (LintCount >= PintTimeout)
                        {
                            MbolTimeout = true;
                            LobjForm.DialogResult = DialogResult.Cancel;
                            LobjForm.Close();
                        }
                        else
                        {
                            // update the title bar
                            LobjForm.Text = PstrTitle + " [" + (PintTimeout - LintCount).ToString() + "]";
                        }
                    };
                    LobjTimer.Start();
                }
                return LobjForm;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the factor for form sizing
        /// </summary>
        /// <param name="PobjForm"></param>
        /// <returns></returns>
        private static double GetDPIFactor(Form PobjForm)
        {
            Graphics g = PobjForm.CreateGraphics();
            try
            {
                double LintFactor = (int)Math.Round(g.DpiX / 96.0f, 0);
                // now set all the semi-constants at the top
                PADDING = (int)(PADDING * LintFactor);
                PICTURE_HEIGHT = (int)(PICTURE_HEIGHT * LintFactor);
                PICTURE_WIDTH = (int)(PICTURE_WIDTH * LintFactor);
                BUTTON_WIDTH = (int)(BUTTON_WIDTH * LintFactor);
                BUTTON_HEIGHT = (int)(BUTTON_HEIGHT * LintFactor);
                return LintFactor;
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// After the form is drawn, we need to arrange
        /// </summary>
        private static void ArrangeForm()
        {
            try
            {
                // find the furthest right
                int LintRight = int.MinValue;
                List<Button> LobjButtons = new List<Button>();
                foreach (Control LobjCtl in MobjForm.Controls["panelButtons"].Controls)
                {
                    // buttons
                    if (LobjCtl is Button)
                    {
                        Button LobjBtn = LobjCtl as Button;
                        LobjButtons.Add(LobjBtn);
                        if ((LobjBtn.Left + LobjBtn.Width) > LintRight)
                            LintRight = LobjBtn.Left + LobjBtn.Width;
                    }
                }
                // now position them all to the left or right to fit perfectly
                int LintAdjust = MobjForm.Width - LintRight - (PADDING * LobjButtons.Count);
                foreach (Button LobjBtn in LobjButtons)
                {
                    LobjBtn.Left += LintAdjust;
                    if (LobjBtn.Left < 0)
                    {
                        MobjForm.Width += -1 * LobjBtn.Left;
                        LintAdjust += (-1 * LobjBtn.Left) + PADDING;
                        LobjBtn.Left = PADDING;
                    }
                }
                if (LobjButtons.Count == 1) LobjButtons[0].Left -= PADDING;

                // now if there is no icon and only a label then we want to
                // arrange our label right in the middle of the form
                if (MobjForm.Controls["panelTop"].Controls.Count == 1)
                {
                    MobjForm.Controls["panelTop"].Controls[0].Top =
                        MobjForm.Controls["panelTop"].Height / 2;
                }
            }
            catch (Exception PobjEx)
            {
                throw PobjEx;
            }
        }

        /// <summary>
        /// Gets the height of the title bar
        /// </summary>
        /// <param name="PobjForm"></param>
        /// <returns></returns>
        private static int GetTitleBarHeight(Form PobjForm)
        {
            try
            {
                Rectangle LobjScreenRectangle = PobjForm.RectangleToScreen(PobjForm.ClientRectangle);
                return LobjScreenRectangle.Top - PobjForm.Top;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the top panel of the message box
        /// ICON + TEXT
        /// </summary>
        /// <returns></returns>
        private static Panel GetTopPanel(MessageBoxIcon PobjIcon, string PstrText, Hyperlink PobjHyperlink)
        {
            try
            {
                Panel LobjPanel = new Panel();
                LobjPanel.Size = new Size(1, 1);
                LobjPanel.Name = "panelTop";
                LobjPanel.AutoSize = true;
                int LintLeft = PADDING * 2;
                int LintMiddle = PADDING;
                if (PobjIcon != MessageBoxIcon.None)
                {
                    PictureBox LobjIcon = new PictureBox();
                    LobjIcon = new PictureBox();
                    LobjIcon.Left = LintLeft;
                    LobjIcon.Top = PADDING * 2;
                    LobjIcon.Width = PICTURE_WIDTH;
                    LobjIcon.Height = PICTURE_HEIGHT;
                    LobjIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                    LobjIcon.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    LobjIcon.Image = GetIcon(PobjIcon);
                    LobjPanel.Controls.Add(LobjIcon);
                    LintLeft = LobjIcon.Left + LobjIcon.Width;
                    LintMiddle = (LobjIcon.Top + LobjIcon.Height) / 2;
                }
                Label LobjBox = new Label();
                LobjBox.Text = PstrText + "\n\n"; //<-- adds some space
                LobjBox.AutoSize = true;
                LobjBox.BorderStyle = BorderStyle.None;
                LobjBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                LobjBox.Left = LintLeft + PADDING;
                LobjBox.Top = LintMiddle;
                LobjBox.MaximumSize = new Size((int)(600 * MintDPI) - PICTURE_WIDTH - (PADDING * 3),
                                               (int)(600 * MintDPI) - PADDING);
                // add
                LobjPanel.Controls.Add(LobjBox);
                // hyperlink?
                if(PobjHyperlink != null)
                {
                    LinkLabel LobjLinkLabel = new LinkLabel();
                    LinkLabel.Link LobjLink = new LinkLabel.Link();
                    LobjLinkLabel.AutoSize = true;
                    LobjLinkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    LobjLinkLabel.Left = LobjBox.Left;
                    LobjLinkLabel.Top = LobjBox.Top + LobjBox.Height + PADDING;
                    LobjLinkLabel.Text = PobjHyperlink.HyperlinkText;
                    LobjLink.LinkData = PobjHyperlink.HyperlinkLinkData;
                    LobjLinkLabel.Links.Add(LobjLink);
                    LobjLinkLabel.LinkClicked += (o, e) => { Process.Start(e.Link.LinkData as string); };
                    LobjPanel.Controls.Add(LobjLinkLabel);
                }
                // size
                LobjPanel.Width = LintLeft + LobjBox.Width + (PADDING * 4);
                LobjPanel.Dock = DockStyle.Top;
                LobjPanel.MaximumSize = new Size((int)(600 * MintDPI), (int)(600 * MintDPI));
                LobjPanel.MinimumSize = new Size((int)(40 * MintDPI), (int)(40 * MintDPI));
                return LobjPanel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the button panel for each button
        /// [YES] [NO] [CANCEL]
        /// </summary>
        /// <param name="PobjButtons"></param>
        /// <returns></returns>
        private static Panel GetButtonPanel(MessageBoxButtons PobjButtons, 
                                            string PstrCheckboxText,
                                            string PstrMore)
        {
            try
            {
                List<Button> LobjButtonsList = new List<Button>();
                switch (PobjButtons)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        LobjButtonsList.Add(CreateButton(DialogResult.Abort));
                        LobjButtonsList.Add(CreateButton(DialogResult.Retry));
                        LobjButtonsList.Add(CreateButton(DialogResult.Ignore));
                        break;
                    case MessageBoxButtons.OK:
                        LobjButtonsList.Add(CreateButton(DialogResult.OK));
                        break;
                    case MessageBoxButtons.OKCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.OK));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel));
                        break;
                    case MessageBoxButtons.RetryCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.Retry));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel));
                        break;
                    case MessageBoxButtons.YesNo:
                        LobjButtonsList.Add(CreateButton(DialogResult.Yes));
                        LobjButtonsList.Add(CreateButton(DialogResult.No));
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        LobjButtonsList.Add(CreateButton(DialogResult.Yes));
                        LobjButtonsList.Add(CreateButton(DialogResult.No));
                        LobjButtonsList.Add(CreateButton(DialogResult.Cancel));
                        break;
                }
                // finally add the more button if needed
                if(!string.IsNullOrEmpty(PstrMore))
                {
                    // this creates the more... button
                    LobjButtonsList.Add(CreateButton(DialogResult.None));
                    MstrMoreText = PstrMore;
                }

                Panel LobjPanel = new Panel();
                LobjPanel.Name = "panelButtons";
                LobjPanel.BackColor = SystemColors.ControlLight;
                LobjPanel.Dock = DockStyle.Bottom;
                LobjPanel.MaximumSize = new Size((int)(600 * MintDPI), (int)(100 * MintDPI));
                int LintButtonCount = 0;
                int LintLeft = 0;
                int LintHeight = 0;
                // add checkboxes
                int LintBoxRight = 0;
                if (!string.IsNullOrEmpty(PstrCheckboxText))
                {
                    CheckBox LobjBox = new CheckBox();
                    LobjBox.AutoSize = true;
                    LobjBox.Text = PstrCheckboxText;
                    LobjBox.Left = PADDING;
                    LobjBox.Top = PADDING;
                    LobjBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    LobjBox.Click += (o, e) => { MbolChecked = LobjBox.Checked; };
                    LobjBox.SendToBack();
                    LobjPanel.Controls.Add(LobjBox);
                    LintBoxRight = LobjBox.Left + LobjBox.Width;
                }
                // add buttons
                LintHeight += BUTTON_HEIGHT;
                foreach (Button LobjButton in LobjButtonsList)
                {
                    LintLeft = (BUTTON_WIDTH) * LintButtonCount;
                    LobjButton.Left = LintLeft;
                    LobjButton.Top = PADDING / 2;
                    LobjPanel.Controls.Add(LobjButton);
                    LintButtonCount++;
                }
                // adjust of the panel is too small
                if (LintBoxRight > 0 && LintBoxRight > LobjButtonsList[0].Left)
                {
                    // we need to move the buttons down
                    foreach (Button LobjButton in LobjButtonsList)
                    {
                        LobjButton.Top += BUTTON_HEIGHT;
                    }
                    LobjPanel.Height = LobjButtonsList[0].Top + LobjButtonsList[0].Height + (PADDING / 2);
                }
                else
                {
                    // change the height
                    LobjPanel.Height = LintHeight + PADDING;
                }
                return LobjPanel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Expands the form
        /// </summary>
        private static void ExpandForm()
        {
            try
            {
                Panel LobjButtonPanel = MobjForm.Controls.Find("panelButtons", true)[0] as Panel;
                TextBox LobjBox = new TextBox();
                LobjBox.Width = MobjForm.Width - (PADDING * 3);
                LobjBox.Left = PADDING;
                LobjBox.Top = LobjButtonPanel.Height;
                LobjBox.ReadOnly = true;
                LobjBox.ScrollBars = ScrollBars.Vertical;
                LobjBox.Multiline = true;
                LobjBox.MinimumSize = new Size(LobjBox.Width, 120);
                LobjBox.Text = MstrMoreText;
                LobjBox.TabStop = false;
                LobjBox.BackColor = SystemColors.ButtonFace;
                LobjBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                LobjButtonPanel.MaximumSize = new Size((int)(600 * MintDPI), 
                                                       LobjButtonPanel.Height + LobjBox.Height + PADDING);
                LobjButtonPanel.Height += LobjBox.Height + PADDING;
                // add to the bottom of the button panel
                LobjButtonPanel.Controls.Add(LobjBox);
                // diable the more button
                Button LobjMoreButton = MobjForm.Controls.Find("moreButton", true)[0] as Button;
                LobjMoreButton.Enabled = false;
                
            }
            catch(Exception PobjEx)
            {
                throw PobjEx;
            }
        }

        /// <summary>
        /// Creates a generic button based on the default text
        /// </summary>
        /// <param name="PobjDialogResult"></param>
        /// <returns></returns>
        private static Button CreateButton(DialogResult PobjDialogResult)
        {
            try
            {
                Button LobjButton = new Button();
                if (PobjDialogResult != DialogResult.None)
                {
                    LobjButton.Text = "&" + PobjDialogResult.ToString();
                    LobjButton.DialogResult = PobjDialogResult;
                    // EVENT to close the form on a button click
                    LobjButton.Click += (o, e) => { MobjForm.Close(); };
                }
                else
                {
                    LobjButton.Text = "&More >";
                    LobjButton.Click += (o, e) => { ExpandForm(); };
                    LobjButton.Name = "moreButton";
                }
                LobjButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;// | AnchorStyles.Left;
                LobjButton.Width = BUTTON_WIDTH;
                LobjButton.Height = BUTTON_HEIGHT;
                return LobjButton;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the bitmap image of the selected icon
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static Bitmap GetIconBitmap(MessageBoxIcon PobjIcon)
        {
            try
            {
                Bitmap LobjReturn = null;
                switch (PobjIcon)
                {
                    case MessageBoxIcon.Error:
                        //case MessageBoxIcon.Hand:
                        // case MessageBoxIcon.Stop:
                        LobjReturn = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Information:
                        //case MessageBoxIcon.Asterisk:
                        LobjReturn = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        //case MessageBoxIcon.Exclamation:
                        LobjReturn = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        LobjReturn = SystemIcons.Question.ToBitmap();
                        break;
                }
                return ResizeBitmap(LobjReturn);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resizes the system icon to a higher resolution
        /// </summary>
        /// <param name="PobjBitmap"></param>
        /// <returns></returns>
        private static Bitmap ResizeBitmap(Bitmap PobjBitmap)
        {
            try
            {
                Size LobjS = new Size(PICTURE_WIDTH, PICTURE_HEIGHT);
                Bitmap LobjBm = new Bitmap(LobjS.Width, LobjS.Height);

                using (Graphics LobjG = Graphics.FromImage(LobjBm))
                {
                    LobjG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    LobjG.DrawImage(PobjBitmap, new Rectangle(Point.Empty, LobjS));
                }

                return LobjBm;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// PINVOKES WINDOWS
        /// This gets the built in icons from Windows to display
        /// a better image for the form
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static Bitmap GetIcon(MessageBoxIcon PobjIcon)
        {
            try
            {
                SHSTOCKICONINFO LobjSii = new SHSTOCKICONINFO();
                LobjSii.cbSize = (UInt32)Marshal.SizeOf(typeof(SHSTOCKICONINFO));

                Marshal.ThrowExceptionForHR(SHGetStockIconInfo(ConvertIconCode(PobjIcon),
                     SHGSI.SHGSI_ICON | SHGSI.SHGSI_LARGEICON,
                     ref LobjSii));
                Bitmap LobjReturnBitmap = Icon.FromHandle(LobjSii.hIcon).ToBitmap();
                DestroyIcon(LobjSii.hIcon); // cleanup
                return LobjReturnBitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the bitmap image of the selected icon
        /// </summary>
        /// <param name="PobjIcon"></param>
        /// <returns></returns>
        private static SHSTOCKICONID ConvertIconCode(MessageBoxIcon PobjIcon)
        {
            try
            {
                switch (PobjIcon)
                {
                    case MessageBoxIcon.Error:
                        //case MessageBoxIcon.Hand:
                        // case MessageBoxIcon.Stop:
                        return SHSTOCKICONID.SIID_ERROR;
                    case MessageBoxIcon.Information:
                        //case MessageBoxIcon.Asterisk:
                        return SHSTOCKICONID.SIID_INFO;
                    case MessageBoxIcon.Warning:
                        //case MessageBoxIcon.Exclamation:
                        return SHSTOCKICONID.SIID_WARNING;
                    case MessageBoxIcon.Question:
                        return SHSTOCKICONID.SIID_HELP;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
