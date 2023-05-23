using System;
using System.Windows.Forms;

namespace Minesweeper
{
    public static class FormExtensions
    {
        /*
            Helper function which enables cross-thread invoking of functions if necessary
        */
        internal static void MainThreadInvoke(this Control control, Action func)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(func);
            }
            else
            {
                func();
            }
        }
    }
}
