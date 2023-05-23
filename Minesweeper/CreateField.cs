using System;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class CreateField : Form
    {
        #region non-static attributes
        public int width = 0;
        public int height = 0;
        public int mines = 0;
        #endregion

        #region constructors and overrides
        public CreateField(int a, int b, int c)
        {
            InitializeComponent();
            numericUpDown1.Value = a;
            numericUpDown2.Value = b;
            numericUpDown3.Value = c;
        }
        #endregion

        #region events
        private void SubmitValues(object sender, EventArgs e)
        {
            width = (int)numericUpDown1.Value;
            height = (int)numericUpDown2.Value;
            mines = (int)numericUpDown3.Value;
            if (((float)mines / (width * height)) > 0.75)
            {
                MessageBox.Show("Mines aren't allowed to make up more than 75% of the field. The maximum for the currently set width and height is " + ((int)((height * width) * 0.75f)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void RedirectEnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                SubmitValues(sender, new EventArgs());
            }
        }
        #endregion
    }
}
