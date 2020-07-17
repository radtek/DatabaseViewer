using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA_SMO
{
   public class WindowWrapper : System.Windows.Forms.IWin32Window
   {
      private IntPtr _hwnd;

      // Set the property as part of the constructor
      public WindowWrapper(IntPtr handle)
      {
         _hwnd = handle;
      }

      // Retrieve the property value
      public IntPtr Handle
      {
         get { return _hwnd; }
      }
   }
}
