using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace VerusSententiaeFull
{
    public partial class AutoClosingMessageBox : Notification
    {
        public AutoClosingMessageBox(string message, int seconds)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }
    }

    // Usage example
    // new AutoClosingMessageBox("Your message here", 2).Show();

}
