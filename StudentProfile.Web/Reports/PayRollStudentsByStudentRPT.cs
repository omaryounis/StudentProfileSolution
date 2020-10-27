using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace StudentProfile.Web.Reports
{
    public partial class PayRollStudentsByStudentRPT : DevExpress.XtraReports.UI.XtraReport
    {
        private int rowIndex = 0;
        public PayRollStudentsByStudentRPT()
        {
            InitializeComponent();
        }

        private void PayRollStudentsByStudentRPT_AfterPrint(object sender, EventArgs e)
        {
            ((XtraReport)sender).PrintingSystem.Document.AutoFitToPagesWidth = 1;
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

        private void xrPivotGrid1_FieldValueDisplayText(object sender, DevExpress.XtraReports.UI.PivotGrid.PivotFieldDisplayTextEventArgs e)
        {
            if (e.ValueType == DevExpress.XtraPivotGrid.PivotGridValueType.GrandTotal && e.DataField != null)
                e.DisplayText = $"الاجمالى {e.Value}";

            if (e.ValueType == DevExpress.XtraPivotGrid.PivotGridValueType.Total && e.DataField != null)
                e.DisplayText = $"ج. {e.Value}";

            if (e.Field != null && !e.IsColumn && e.Field.Caption == "م")
            {
                e.DisplayText = String.Format("{0}  {1}", rowIndex + 1, " ");
                rowIndex += 1;
            }
        }
    }
}
