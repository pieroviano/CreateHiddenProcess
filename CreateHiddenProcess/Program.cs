using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CreateHiddenProcess
{
    class Program
    {
        private static ParentForm _parentForm;

        static void Main(string[] args)
        {
            _parentForm = new ParentForm();
            _parentForm.ShowDialog();
        }

    }
}
