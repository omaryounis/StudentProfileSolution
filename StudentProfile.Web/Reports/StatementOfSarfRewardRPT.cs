using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace StudentProfile.Web.Reports
{
    public partial class StatementOfSarfRewardRPT : DevExpress.XtraReports.UI.XtraReport
    {
        public StatementOfSarfRewardRPT()
        {
            InitializeComponent();
        }
        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var pictureBox = (XRPictureBox)sender;

            const string path = "~\\Content\\logo.png";

            var relativePath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (File.Exists(relativePath))
            {
                pictureBox.ImageUrl = path;
            }
        }

        private void StatementOfSarfRewardRPT_AfterPrint(object sender, EventArgs e)
        {
            ((XtraReport)sender).PrintingSystem.Document.AutoFitToPagesWidth = 1;

        }
    }
}
