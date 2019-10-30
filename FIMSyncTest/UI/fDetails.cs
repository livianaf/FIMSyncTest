using System.Windows.Forms;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public partial class fDetails : Form {
        public fDetails() { InitializeComponent(); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        protected override bool ProcessDialogKey(Keys keyData) {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape) {
                this.Close();
                return true;
                }
            return base.ProcessDialogKey(keyData);
            }
        }
    }
