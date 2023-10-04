using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace OrderManagerEF

{
    public partial class ProgressForm : SplashScreen
    {
        public ProgressForm()
        {
            InitializeComponent();
            MessageLabel.Visible = false; // Hide the label initially
        }

        public enum SplashScreenCommand
        {
            SetProgress,
            SetMessage
        }

        public void SetProgress(int value)
        {
            progressBarControl1.EditValue = value;
        }

        public void SetMessage(string message)
        {
            // Assuming you have a label named errorMessageLabel on your form
            MessageLabel.Visible = true; // Show the label
            MessageLabel.Text = message;

        }

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);

            if (cmd is SplashScreenCommand command)
            {
                switch (command)
                {
                    case SplashScreenCommand.SetProgress:
                        SetProgress((int)arg);
                        break;
                    case SplashScreenCommand.SetMessage:
                        SetMessage((string)arg);
                        break;
                }
            }
        }
    }

}