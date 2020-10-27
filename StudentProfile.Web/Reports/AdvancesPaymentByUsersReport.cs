using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace StudentProfile.Web.Reports
{
    public partial class AdvancesPaymentByUsersReport : DevExpress.XtraReports.UI.XtraReport
    {
        public AdvancesPaymentByUsersReport()
        {
            InitializeComponent();
        }

        private void xrPictureBox2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            var pictureBox = (XRPictureBox)sender;

            const string path = "~\\Content\\logo.png";

            var relativePath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (File.Exists(relativePath))
            {
                pictureBox.ImageUrl = path;
            }
        }

        private void xrPageBreak1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRPageBreak control = sender as XRPageBreak;
            int value = CurrentRowIndex + 1;
            if (value % 12 == 0)
                control.Visible = true;
            else
                control.Visible = false;
        }
    }
}
