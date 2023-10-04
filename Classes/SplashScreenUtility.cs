using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{
    public static class SplashScreenUtility
    {
        public static void CloseSplashScreenIfNeeded()
        {
            if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
            {
                SplashScreenManager.CloseForm();
            }
        }
    }

}


