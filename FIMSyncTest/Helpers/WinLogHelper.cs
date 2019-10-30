using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

//_________________________________________________________________________________________________________
//_________________________________________________________________________________________________________
namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public enum Level : int {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4,
        Debug = 5,
        Reset = 10
        };
    //_________________________________________________________________________________________________________
    // cWinLog.Init();
    // cWinLog.Show();
    // cWinLog.Log(Level.Debug, "A debug level message");
    // cWinLog.Log(Level.Verbose, "A verbose level message");
    // cWinLog.Log(Level.Info, "A info level message");
    // cWinLog.Log(Level.Warning, "A warning level message");
    // cWinLog.Log(Level.Error, "A error level message");
    // cWinLog.Log(Level.Critical, "A critical level message");
    // cWinLog.UnPause();
    // cWinLog.Dispose();
    //_________________________________________________________________________________________________________
    public static class cWinLog {
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static ListBoxLog listBoxLog { get; set; }
        private static Form fLog { get; set; }
        private static ListBox cLst { get; set; }
        private static Form fParent { get; set; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void Init() { fLog = CreateForm(); listBoxLog = new ListBoxLog(cLst); }
        public static void Dispose() { if (fLog == null) return; listBoxLog.Dispose(); }
        public static void Show(Form Parent) {
            fParent = Parent; 
            if (fLog == null) cWinLog.Init();
            if (!fLog.Visible)
                fLog.Show(fParent);
            else
                fLog.Hide();
            listBoxLog.Paused = false;
            }
        public static void Show(bool Opc) {
            if (fParent == null) return;
            if (!fLog.Visible && Opc) fLog.Show(fParent);
            if (fLog.Visible && !Opc) fLog.Hide();
            }
        public static void Log(string message) { if (fLog == null) cWinLog.Init(); listBoxLog.Log(message); }
        public static void Log(string format, params object[] args) { if (fLog == null) cWinLog.Init(); listBoxLog.Log(format, args); }
        public static void Log(Level level, string format, params object[] args) { if (fLog == null) cWinLog.Init(); listBoxLog.Log(level, format, args); }
        public static void Log(Level level, string message) { if (fLog == null) cWinLog.Init(); listBoxLog.Log(level, message); }
        public static void ClearLog() { if (fLog == null) cWinLog.Init(); listBoxLog.Log(Level.Reset, ""); }
        //public static void UnPause() { cWinLog.Init(); listBoxLog.Paused=false; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static Form CreateForm() {
            // cLst
            cLst = new ListBox();
            cLst.Dock = DockStyle.Fill;
            cLst.FormattingEnabled = true;
            cLst.HorizontalScrollbar = true;
            cLst.IntegralHeight = false;
            cLst.Location = new System.Drawing.Point(0, 0);
            cLst.Name = "cLst";
            cLst.SelectionMode = SelectionMode.MultiExtended;
            cLst.Size = new System.Drawing.Size(800, 200);
            cLst.TabIndex = 0;
            // fLogcs
            Form f = new Form();
            f.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            f.AutoScaleMode = AutoScaleMode.Font;
            f.AutoScroll = true;
            f.ClientSize = new System.Drawing.Size(800, 200);
            f.ControlBox = false;
            f.Controls.Add(cLst);
            f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            f.Name = "fLogcs";
            f.Opacity = 0.8D;
            f.ShowInTaskbar = false;
            f.Text = "Macro Execution Log";
            f.StartPosition = FormStartPosition.Manual;
            f.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - f.Width, Screen.PrimaryScreen.WorkingArea.Height - f.Height - 24);
            return f;
            }
        }
    //_________________________________________________________________________________________________________
    // Based on https://exceptionshub.com/elegant-log-window-in-winforms-c.html
    //_________________________________________________________________________________________________________
    public sealed class ListBoxLog : IDisposable {
        private const string DEFAULT_MESSAGE_FORMAT = "{0} [{5}] : {8}";
        private const int DEFAULT_MAX_LINES_IN_LISTBOX = 2000;

        private bool _disposed;
        private ListBox _listBox;
        private string _messageFormat;
        private int _maxEntriesInListBox;
        private long _maxLengthItem { get; set; }
        private bool _canAdd;
        private bool _paused;
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void OnHandleCreated(object sender, EventArgs e) { _canAdd = true; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void OnHandleDestroyed(object sender, EventArgs e) { _canAdd = false; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void DrawItemHandler(object sender, DrawItemEventArgs e) {
            if (e.Index >= 0) {
                e.DrawBackground();
                e.DrawFocusRectangle();

                LogEvent logEvent = ((ListBox)sender).Items[e.Index] as LogEvent;

                // SafeGuard against wrong configuration of list box
                if (logEvent == null) {
                    logEvent = new LogEvent(Level.Critical, ((ListBox)sender).Items[e.Index].ToString());
                    }

                Color color;
                switch (logEvent.Level) {
                    case Level.Critical:
                        color = Color.White;
                        break;
                    case Level.Error:
                        color = Color.Red;
                        break;
                    case Level.Warning:
                        color = Color.Goldenrod;
                        break;
                    case Level.Info:
                        color = Color.Green;
                        break;
                    case Level.Verbose:
                        color = Color.Blue;
                        break;
                    default:
                        color = Color.Black;
                        break;
                    }

                if (logEvent.Level == Level.Critical) {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), e.Bounds);
                    }
                e.Graphics.DrawString(FormatALogEventMessage(logEvent, _messageFormat), new Font("Lucida Console", 8.25f, FontStyle.Regular), new SolidBrush(color), e.Bounds);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void KeyDownHandler(object sender, KeyEventArgs e) {
            if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.C)) {
                CopyToClipboard();
                }
            if (e.KeyCode == Keys.Escape) _listBox.Parent.Hide();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void ClearAllOnClickHandler(object sender, EventArgs e) { _listBox.Items.Clear(); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void SelectAllOnClickHandler(object sender, EventArgs e) {
            for (int i = 0; i < _listBox.Items.Count; i++) 
                _listBox.SetSelected(i, true);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void CopyMenuOnClickHandler(object sender, EventArgs e) { CopyToClipboard(); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void CopyMenuPopupHandler(object sender, EventArgs e) {
            ContextMenu menu = sender as ContextMenu;
            if (menu != null) {
                menu.MenuItems[0].Enabled = (_listBox.SelectedItems.Count > 0);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private class LogEvent {
            public LogEvent(Level level, string message) {
                EventTime = DateTime.Now;
                Level = level;
                Message = message;
                }

            public readonly DateTime EventTime;

            public readonly Level Level;
            public readonly string Message;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void WriteEvent(LogEvent logEvent) {
            if ((logEvent != null) && (_canAdd)) {
                _listBox.BeginInvoke(new AddALogEntryDelegate(AddALogEntry), logEvent);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private delegate void AddALogEntryDelegate(object item);
        private void AddALogEntry(object item) {
            // Clean Log Window if required
            if(((LogEvent)item).Level == Level.Reset){ _listBox.Items.Clear(); return; }

            // Add Log Entry
            _listBox.Items.Add(item);
            string sCadena = FormatALogEventMessage(((LogEvent)item), _messageFormat);
            try { System.IO.File.AppendAllText(Config.LogMacroFile, sCadena + "\r\n"); } catch (Exception) { }
            long lCadena = sCadena.Length;
            if (lCadena > _maxLengthItem) {
                _maxLengthItem = lCadena;  
                // Create a Graphics object to use when determining the size of the largest item in the ListBox.
                Graphics g = _listBox.CreateGraphics();
                // Determine the size for HorizontalExtent using the MeasureString method using the new item in the list.
                int hzSize = (int)g.MeasureString(sCadena, _listBox.Font).Width;
                // Set the HorizontalExtent property.
                _listBox.HorizontalExtent = (int)(hzSize*4/3);
                }

            if (_listBox.Items.Count > _maxEntriesInListBox) {
                _listBox.Items.RemoveAt(0);
                }

            if (!_paused) _listBox.TopIndex = _listBox.Items.Count - 1;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private string LevelName(Level level) {
            switch (level) {
                case Level.Critical: return "Critical";
                case Level.Error: return "Error";
                case Level.Warning: return "Warning";
                case Level.Info: return "Info";
                case Level.Verbose: return "Verbose";
                case Level.Debug: return "Debug";
                default: return string.Format("<value={0}>", (int)level);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private string FormatALogEventMessage(LogEvent logEvent, string messageFormat) {
            string message = logEvent.Message;
            if (message == null) { message = "<NULL>"; }
            return string.Format(messageFormat,
                /* {0} */ logEvent.EventTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                /* {1} */ logEvent.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                /* {2} */ logEvent.EventTime.ToString("yyyy-MM-dd"),
                /* {3} */ logEvent.EventTime.ToString("HH:mm:ss.fff"),
                /* {4} */ logEvent.EventTime.ToString("HH:mm:ss"),

                /* {5} */ LevelName(logEvent.Level)[0],
                /* {6} */ LevelName(logEvent.Level),
                /* {7} */ (int)logEvent.Level,

                /* {8} */ message);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void CopyToClipboard() {
            if (_listBox.SelectedItems.Count > 0) {
                StringBuilder selectedItemsAsRTFText = new StringBuilder();
                selectedItemsAsRTFText.AppendLine(@"{\rtf1\ansi\deff0{\fonttbl{\f0\fcharset0 Courier;}}");
                selectedItemsAsRTFText.AppendLine(@"{\colortbl;\red255\green255\blue255;\red255\green0\blue0;\red218\green165\blue32;\red0\green128\blue0;\red0\green0\blue255;\red0\green0\blue0}");
                foreach (LogEvent logEvent in _listBox.SelectedItems) {
                    selectedItemsAsRTFText.AppendFormat(@"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ", (logEvent.Level == Level.Critical) ? 2 : 1, (logEvent.Level == Level.Critical) ? 1 : ((int)logEvent.Level > 5) ? 6 : ((int)logEvent.Level) + 1);
                    selectedItemsAsRTFText.Append(FormatALogEventMessage(logEvent, _messageFormat));
                    selectedItemsAsRTFText.AppendLine(@"\par}");
                    }
                selectedItemsAsRTFText.AppendLine(@"}");
                StringBuilder selectedItemsAsText = new StringBuilder();
                foreach (LogEvent logEvent in _listBox.SelectedItems) {
                    selectedItemsAsText.AppendLine(FormatALogEventMessage(logEvent, _messageFormat));
                    }
                DataObject data_object = new DataObject();
                data_object.SetData(DataFormats.Rtf, selectedItemsAsRTFText.ToString());
                data_object.SetData(DataFormats.Text, selectedItemsAsText.ToString());

                System.Diagnostics.Debug.WriteLine(selectedItemsAsRTFText.ToString());
                // Copy only RTF format
                //Clipboard.SetData(DataFormats.Rtf, selectedItemsAsRTFText.ToString());
                // Copy RTF and TXT format
                Clipboard.SetDataObject(data_object);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public ListBoxLog(ListBox listBox) : this(listBox, DEFAULT_MESSAGE_FORMAT, DEFAULT_MAX_LINES_IN_LISTBOX) { }
        public ListBoxLog(ListBox listBox, string messageFormat) : this(listBox, messageFormat, DEFAULT_MAX_LINES_IN_LISTBOX) { }
        public ListBoxLog(ListBox listBox, string messageFormat, int maxLinesInListbox) {
            _disposed = false;

            _listBox = listBox;
            _messageFormat = messageFormat;
            _maxEntriesInListBox = maxLinesInListbox;

            _paused = false;

            _canAdd = listBox.IsHandleCreated;

            _listBox.SelectionMode = SelectionMode.MultiExtended;

            _listBox.HandleCreated += OnHandleCreated;
            _listBox.HandleDestroyed += OnHandleDestroyed;
            _listBox.DrawItem += DrawItemHandler;
            _listBox.KeyDown += KeyDownHandler;

            MenuItem[] menuItems = new MenuItem[] { new MenuItem("Copy", new EventHandler(CopyMenuOnClickHandler)), new MenuItem("Select All", new EventHandler(SelectAllOnClickHandler)), new MenuItem("Clear All", new EventHandler(ClearAllOnClickHandler)) };
            _listBox.ContextMenu = new ContextMenu(menuItems);
            _listBox.ContextMenu.Popup += new EventHandler(CopyMenuPopupHandler);

            _listBox.DrawMode = DrawMode.OwnerDrawFixed;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public void Log(string message) { Log(Level.Debug, message); }
        public void Log(string format, params object[] args) { Log(Level.Debug, (format == null) ? null : string.Format(format, args)); }
        public void Log(Level level, string format, params object[] args) { Log(level, (format == null) ? null : string.Format(format, args)); }
        public void Log(Level level, string message) { WriteEvent(new LogEvent(level, message)); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public bool Paused {
            get { return _paused; }
            set { _paused = value; }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        ~ListBoxLog() {
            if (!_disposed) {
                Dispose(false);
                _disposed = true;
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public void Dispose() {
            if (!_disposed) {
                Dispose(true);
                GC.SuppressFinalize(this);
                _disposed = true;
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void Dispose(bool disposing) {
            if (_listBox != null) {
                _canAdd = false;

                _listBox.HandleCreated -= OnHandleCreated;
                _listBox.HandleCreated -= OnHandleDestroyed;
                _listBox.DrawItem -= DrawItemHandler;
                _listBox.KeyDown -= KeyDownHandler;

                _listBox.ContextMenu.MenuItems.Clear();
                _listBox.ContextMenu.Popup -= CopyMenuPopupHandler;
                _listBox.ContextMenu = null;

                _listBox.Items.Clear();
                _listBox.DrawMode = DrawMode.Normal;
                _listBox = null;
                }
            }
        }
    }