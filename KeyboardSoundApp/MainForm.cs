using System;
using System.Windows.Forms;

namespace KeyboardSoundApp
{
    public partial class MainForm : Form
    {
        private Button toggleButton;

        private void InitializeComponent()
        {
            this.toggleButton = new Button();
            this.toggleButton.Text = "Activar";
            this.toggleButton.Location = new System.Drawing.Point(50, 50);
            this.toggleButton.Click += toggleButton_Click;
            this.Controls.Add(this.toggleButton);
            this.Text = "Keyboard Sound App";
            this.ClientSize = new System.Drawing.Size(200, 150);
        }

    }
}
