using DimeXplorer;
using System;
using System.Windows.Forms;

namespace Mario
{
    public partial class frmTransaction : Form
    {
        private string _filename = System.IO.Directory.GetCurrentDirectory() + @"\leveldata.xml";
        public frmTransaction()
        {
            InitializeComponent();
            txtAddress.Text = Wallet.GetPrivateWallet();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddress.Text)) {
                MessageBox.Show("Insert your wallet address please!","Warning");
                return;
            }

            if (txtAddress.Text.Length != 34 || !txtAddress.Text.Substring(0, 1).Equals("7"))
            {
                MessageBox.Show("Your wallet address is formally uncorrect!\nCheck your typing!", "Warning");
                return;
            }

            Wallet.SetPrivateWallet(txtAddress.Text);
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
