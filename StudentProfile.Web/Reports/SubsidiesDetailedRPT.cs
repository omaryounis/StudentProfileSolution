using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using StudentProfile.DAL.Models;
using DevExpress.DataAccess.Sql.DataApi;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for SubsidiesDetailedRPT
/// </summary>
public class SubsidiesDetailedRPT : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
    private PageHeaderBand PageHeader;
    private PageFooterBand PageFooter;
    private ReportFooterBand ReportFooter;

    private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
    private XRLabel xrLabelLevel;
    private XRLabel xrLabelDegree;
    private XRLabel xrLabel1;
    private XRLabel xrLabel2;
    private XRLabel xrLabel4;
    private XRLabel xrLabel3;
    private XRLabel xrLabel5;
    private GroupFooterBand GroupFooter1;
    private XRLabel xrLabel6;
    private XRLabel xrLabel16;
    private XRLabel xrLabel15;
    private XRLabel xrLabel14;
    private XRLabel xrLabel13;
    private XRLabel xrLabel12;
    private XRLabel xrLabel11;
    private XRLabel xrLabel10;
    private XRLabel xrLabel7;
    private XRLabel xrLabel8;
    private XRLabel xrLabel9;
    private XRLabel xrLabelThirdLevelAppoval;
    private XRLabel xrLabelSecondLevelAppoval;
    private XRLabel xrLabelFirstLevelAppoval;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public SubsidiesDetailedRPT()
    {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        DevExpress.DataAccess.ConnectionParameters.CustomStringConnectionParameters customStringConnectionParameters1 =
            new DevExpress.DataAccess.ConnectionParameters.CustomStringConnectionParameters();
        System.ComponentModel.ComponentResourceManager resources =
            new System.ComponentModel.ComponentResourceManager(typeof(SubsidiesDetailedRPT));
        DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
        DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
        DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
        this.Detail = new DevExpress.XtraReports.UI.DetailBand();
        this.xrLabelThirdLevelAppoval = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabelSecondLevelAppoval = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabelFirstLevelAppoval = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabelDegree = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabelLevel = new DevExpress.XtraReports.UI.XRLabel();
        this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
        this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
        this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
        this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
        this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
        this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
        this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
        this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
        this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
        this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
        ((System.ComponentModel.ISupportInitialize) (this)).BeginInit();
        // 
        // Detail
        // 
        this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[]
        {
            this.xrLabelThirdLevelAppoval,
            this.xrLabelSecondLevelAppoval,
            this.xrLabelFirstLevelAppoval,
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1,
            this.xrLabelDegree,
            this.xrLabelLevel
        });
        this.Detail.HeightF = 23F;
        this.Detail.Name = "Detail";
        this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // xrLabelThirdLevelAppoval
        // 
        this.xrLabelThirdLevelAppoval.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabelThirdLevelAppoval.LocationFloat = new DevExpress.Utils.PointFloat(651.2375F, 0F);
        this.xrLabelThirdLevelAppoval.Name = "xrLabelThirdLevelAppoval";
        this.xrLabelThirdLevelAppoval.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabelThirdLevelAppoval.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabelThirdLevelAppoval.StylePriority.UseBorders = false;
        this.xrLabelThirdLevelAppoval.StylePriority.UseTextAlignment = false;
        this.xrLabelThirdLevelAppoval.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.xrLabelThirdLevelAppoval.BeforePrint +=
            new System.Drawing.Printing.PrintEventHandler(this.xrLabelThirdLevelAppoval_BeforePrint);
        // 
        // xrLabelSecondLevelAppoval
        // 
        this.xrLabelSecondLevelAppoval.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabelSecondLevelAppoval.LocationFloat = new DevExpress.Utils.PointFloat(603.144F, 0F);
        this.xrLabelSecondLevelAppoval.Name = "xrLabelSecondLevelAppoval";
        this.xrLabelSecondLevelAppoval.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabelSecondLevelAppoval.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabelSecondLevelAppoval.StylePriority.UseBorders = false;
        this.xrLabelSecondLevelAppoval.StylePriority.UseTextAlignment = false;
        this.xrLabelSecondLevelAppoval.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.xrLabelSecondLevelAppoval.BeforePrint +=
            new System.Drawing.Printing.PrintEventHandler(this.xrLabelSecondLevelAppoval_BeforePrint);
        // 
        // xrLabelFirstLevelAppoval
        // 
        this.xrLabelFirstLevelAppoval.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabelFirstLevelAppoval.LocationFloat = new DevExpress.Utils.PointFloat(555.0505F, 0F);
        this.xrLabelFirstLevelAppoval.Name = "xrLabelFirstLevelAppoval";
        this.xrLabelFirstLevelAppoval.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabelFirstLevelAppoval.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabelFirstLevelAppoval.StylePriority.UseBorders = false;
        this.xrLabelFirstLevelAppoval.StylePriority.UseTextAlignment = false;
        this.xrLabelFirstLevelAppoval.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.xrLabelFirstLevelAppoval.BeforePrint +=
            new System.Drawing.Printing.PrintEventHandler(this.xrLabelFirstLevelAppoval_BeforePrint);
        // 
        // xrLabel4
        // 
        this.xrLabel4.Borders =
            ((DevExpress.XtraPrinting.BorderSide) ((((DevExpress.XtraPrinting.BorderSide.Left |
                                                      DevExpress.XtraPrinting.BorderSide.Top)
                                                     | DevExpress.XtraPrinting.BorderSide.Right)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AdvanceValue]")
        });
        this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(699.3311F, 0F);
        this.xrLabel4.Name = "xrLabel4";
        this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel4.SizeF = new System.Drawing.SizeF(87.5022F, 23F);
        this.xrLabel4.StylePriority.UseBorders = false;
        this.xrLabel4.StylePriority.UseTextAlignment = false;
        this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel3
        // 
        this.xrLabel3.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AdvanceSettingName]")
        });
        this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(464.0821F, 0F);
        this.xrLabel3.Name = "xrLabel3";
        this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel3.SizeF = new System.Drawing.SizeF(90.80176F, 23F);
        this.xrLabel3.StylePriority.UseBorders = false;
        this.xrLabel3.StylePriority.UseTextAlignment = false;
        this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel2
        // 
        this.xrLabel2.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[empFileNum]")
        });
        this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(191.8435F, 0F);
        this.xrLabel2.Name = "xrLabel2";
        this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel2.SizeF = new System.Drawing.SizeF(90.71854F, 23F);
        this.xrLabel2.StylePriority.UseBorders = false;
        this.xrLabel2.StylePriority.UseTextAlignment = false;
        this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel1
        // 
        this.xrLabel1.Borders =
            ((DevExpress.XtraPrinting.BorderSide) ((((DevExpress.XtraPrinting.BorderSide.Left |
                                                      DevExpress.XtraPrinting.BorderSide.Top)
                                                     | DevExpress.XtraPrinting.BorderSide.Right)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Name]")
        });
        this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(6.357829E-05F, 0F);
        this.xrLabel1.Name = "xrLabel1";
        this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel1.SizeF = new System.Drawing.SizeF(191.8434F, 23F);
        this.xrLabel1.StylePriority.UseBorders = false;
        this.xrLabel1.StylePriority.UseTextAlignment = false;
        this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabelDegree
        // 
        this.xrLabelDegree.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabelDegree.LocationFloat = new DevExpress.Utils.PointFloat(282.6452F, 0F);
        this.xrLabelDegree.Name = "xrLabelDegree";
        this.xrLabelDegree.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabelDegree.SizeF = new System.Drawing.SizeF(90.71844F, 23F);
        this.xrLabelDegree.StylePriority.UseBorders = false;
        this.xrLabelDegree.StylePriority.UseTextAlignment = false;
        this.xrLabelDegree.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.xrLabelDegree.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrLabelDegree_BeforePrint);
        // 
        // xrLabelLevel
        // 
        this.xrLabelLevel.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabelLevel.LocationFloat = new DevExpress.Utils.PointFloat(373.3637F, 0F);
        this.xrLabelLevel.Name = "xrLabelLevel";
        this.xrLabelLevel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabelLevel.SizeF = new System.Drawing.SizeF(90.71838F, 23F);
        this.xrLabelLevel.StylePriority.UseBorders = false;
        this.xrLabelLevel.StylePriority.UseTextAlignment = false;
        this.xrLabelLevel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.xrLabelLevel.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrLabelLevel_BeforePrint);
        // 
        // TopMargin
        // 
        this.TopMargin.HeightF = 100F;
        this.TopMargin.Name = "TopMargin";
        this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // BottomMargin
        // 
        this.BottomMargin.HeightF = 100F;
        this.BottomMargin.Name = "BottomMargin";
        this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // sqlDataSource1
        // 
        this.sqlDataSource1.ConnectionName = "HRMadinaEntities (StudentProfile)";
        customStringConnectionParameters1.ConnectionString =
            resources.GetString("customStringConnectionParameters1.ConnectionString");
        this.sqlDataSource1.ConnectionParameters = customStringConnectionParameters1;
        this.sqlDataSource1.Name = "sqlDataSource1";
        customSqlQuery1.Name = "Query";
        customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
        this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[]
        {
            customSqlQuery1
        });
        this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
        // 
        // PageHeader
        // 
        this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[]
        {
            this.xrLabel16,
            this.xrLabel15,
            this.xrLabel14,
            this.xrLabel13,
            this.xrLabel12,
            this.xrLabel11,
            this.xrLabel10,
            this.xrLabel7,
            this.xrLabel8,
            this.xrLabel9
        });
        this.PageHeader.HeightF = 45.99999F;
        this.PageHeader.Name = "PageHeader";
        // 
        // xrLabel16
        // 
        this.xrLabel16.Borders =
            ((DevExpress.XtraPrinting.BorderSide) ((((DevExpress.XtraPrinting.BorderSide.Left |
                                                      DevExpress.XtraPrinting.BorderSide.Top)
                                                     | DevExpress.XtraPrinting.BorderSide.Right)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel16.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(699.3311F, 22.99999F);
        this.xrLabel16.Name = "xrLabel16";
        this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel16.SizeF = new System.Drawing.SizeF(87.58563F, 23F);
        this.xrLabel16.StylePriority.UseBorders = false;
        this.xrLabel16.StylePriority.UseFont = false;
        this.xrLabel16.StylePriority.UseTextAlignment = false;
        this.xrLabel16.Text = "المبلغ";
        this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel15
        // 
        this.xrLabel15.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(464.2487F, 22.99999F);
        this.xrLabel15.Name = "xrLabel15";
        this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel15.SizeF = new System.Drawing.SizeF(90.80176F, 23F);
        this.xrLabel15.StylePriority.UseBorders = false;
        this.xrLabel15.StylePriority.UseFont = false;
        this.xrLabel15.StylePriority.UseTextAlignment = false;
        this.xrLabel15.Text = "نوع الاعانة";
        this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel14
        // 
        this.xrLabel14.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(373.447F, 22.99999F);
        this.xrLabel14.Name = "xrLabel14";
        this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel14.SizeF = new System.Drawing.SizeF(90.80176F, 23F);
        this.xrLabel14.StylePriority.UseBorders = false;
        this.xrLabel14.StylePriority.UseFont = false;
        this.xrLabel14.StylePriority.UseTextAlignment = false;
        this.xrLabel14.Text = "المستوى";
        this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel13
        // 
        this.xrLabel13.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel13.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(282.6452F, 22.99999F);
        this.xrLabel13.Name = "xrLabel13";
        this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel13.SizeF = new System.Drawing.SizeF(90.80176F, 23F);
        this.xrLabel13.StylePriority.UseBorders = false;
        this.xrLabel13.StylePriority.UseFont = false;
        this.xrLabel13.StylePriority.UseTextAlignment = false;
        this.xrLabel13.Text = "الدرجة العلمية";
        this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel12
        // 
        this.xrLabel12.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(191.8435F, 22.99999F);
        this.xrLabel12.Name = "xrLabel12";
        this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel12.SizeF = new System.Drawing.SizeF(90.80176F, 23F);
        this.xrLabel12.StylePriority.UseBorders = false;
        this.xrLabel12.StylePriority.UseFont = false;
        this.xrLabel12.StylePriority.UseTextAlignment = false;
        this.xrLabel12.Text = "الرقم الاكاديمى";
        this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel11
        // 
        this.xrLabel11.Borders =
            ((DevExpress.XtraPrinting.BorderSide) ((((DevExpress.XtraPrinting.BorderSide.Left |
                                                      DevExpress.XtraPrinting.BorderSide.Top)
                                                     | DevExpress.XtraPrinting.BorderSide.Right)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(6.357829E-05F, 22.99999F);
        this.xrLabel11.Name = "xrLabel11";
        this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel11.SizeF = new System.Drawing.SizeF(191.8434F, 23F);
        this.xrLabel11.StylePriority.UseBorders = false;
        this.xrLabel11.StylePriority.UseFont = false;
        this.xrLabel11.StylePriority.UseTextAlignment = false;
        this.xrLabel11.Text = "الاسم";
        this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel10
        // 
        this.xrLabel10.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Right)));
        this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(555.0505F, 0F);
        this.xrLabel10.Name = "xrLabel10";
        this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel10.SizeF = new System.Drawing.SizeF(145.248F, 23F);
        this.xrLabel10.StylePriority.UseBorders = false;
        this.xrLabel10.StylePriority.UseFont = false;
        this.xrLabel10.StylePriority.UseTextAlignment = false;
        this.xrLabel10.Text = "مستوى الموافقة";
        this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel7
        // 
        this.xrLabel7.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(555.0505F, 22.99999F);
        this.xrLabel7.Name = "xrLabel7";
        this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel7.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabel7.StylePriority.UseBorders = false;
        this.xrLabel7.StylePriority.UseFont = false;
        this.xrLabel7.StylePriority.UseTextAlignment = false;
        this.xrLabel7.Text = "الاول";
        this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel8
        // 
        this.xrLabel8.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(603.144F, 22.99999F);
        this.xrLabel8.Name = "xrLabel8";
        this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel8.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabel8.StylePriority.UseBorders = false;
        this.xrLabel8.StylePriority.UseFont = false;
        this.xrLabel8.StylePriority.UseTextAlignment = false;
        this.xrLabel8.Text = "الثانى";
        this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // xrLabel9
        // 
        this.xrLabel9.Borders =
            ((DevExpress.XtraPrinting.BorderSide) (((DevExpress.XtraPrinting.BorderSide.Left |
                                                     DevExpress.XtraPrinting.BorderSide.Top)
                                                    | DevExpress.XtraPrinting.BorderSide.Bottom)));
        this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
        this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(651.2375F, 22.99999F);
        this.xrLabel9.Name = "xrLabel9";
        this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel9.SizeF = new System.Drawing.SizeF(48.09354F, 23F);
        this.xrLabel9.StylePriority.UseBorders = false;
        this.xrLabel9.StylePriority.UseFont = false;
        this.xrLabel9.StylePriority.UseTextAlignment = false;
        this.xrLabel9.Text = "الثالث";
        this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        // 
        // PageFooter
        // 
        this.PageFooter.Expanded = false;
        this.PageFooter.HeightF = 100F;
        this.PageFooter.Name = "PageFooter";
        // 
        // ReportFooter
        // 
        this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[]
        {
            this.xrLabel5
        });
        this.ReportFooter.HeightF = 23F;
        this.ReportFooter.KeepTogether = true;
        this.ReportFooter.Name = "ReportFooter";
        this.ReportFooter.PrintAtBottom = true;
        // 
        // xrLabel5
        // 
        this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([AdvanceValue])")
        });
        this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(698.3572F, 0F);
        this.xrLabel5.Multiline = true;
        this.xrLabel5.Name = "xrLabel5";
        this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel5.SizeF = new System.Drawing.SizeF(88.64282F, 23F);
        xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
        this.xrLabel5.Summary = xrSummary1;
        this.xrLabel5.TextFormatString = "الااجمالى {0}";
        // 
        // GroupFooter1
        // 
        this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[]
        {
            this.xrLabel6
        });
        this.GroupFooter1.HeightF = 23F;
        this.GroupFooter1.KeepTogether = true;
        this.GroupFooter1.Name = "GroupFooter1";
        this.GroupFooter1.RepeatEveryPage = true;
        // 
        // xrLabel6
        // 
        this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[]
        {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([AdvanceValue])")
        });
        this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(699.2087F, 0F);
        this.xrLabel6.Multiline = true;
        this.xrLabel6.Name = "xrLabel6";
        this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel6.SizeF = new System.Drawing.SizeF(87.70801F, 23F);
        xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Page;
        this.xrLabel6.Summary = xrSummary2;
        // 
        // SubsidiesDetailedRPT
        // 
        this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[]
        {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.PageFooter,
            this.ReportFooter,
            this.GroupFooter1
        });
        this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[]
        {
            this.sqlDataSource1
        });
        this.DataMember = "Query";
        this.DataSource = this.sqlDataSource1;
        this.DisplayName = "تقرير مفصل باعانات الطلاب";
        this.Margins = new System.Drawing.Printing.Margins(20, 20, 100, 100);
        this.PageHeight = 1169;
        this.PageWidth = 827;
        this.PaperKind = System.Drawing.Printing.PaperKind.A4;
        this.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.Yes;
        this.RightToLeftLayout = DevExpress.XtraReports.UI.RightToLeftLayout.Yes;
        this.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        this.Version = "17.2";
        ((System.ComponentModel.ISupportInitialize) (this)).EndInit();
    }

    #endregion


    private void xrTableCellLevel_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var cell = (XRTableCell) sender;
        var currentColumnValue = GetCurrentColumnValue("empFileNum");
        if (currentColumnValue == null)
        {
            return;
        }

        var empFileNum = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(empFileNum))
        {
            return;
        }

        var studentId = decimal.Parse(empFileNum);
        var student = db.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);
        if (student != null)
        {
            cell.Value = student.LEVEL_DESC;
        }
    }


    private void xrLabelDegree_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var label = (XRLabel) sender;
        var currentColumnValue = GetCurrentColumnValue("empFileNum");
        if (currentColumnValue == null)
        {
            return;
        }

        var empFileNum = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(empFileNum))
        {
            return;
        }

        var studentId = decimal.Parse(empFileNum);
        var student = db.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);
        if (student != null)
        {
            label.Text = student.DEGREE_DESC;
        }
    }

    private void xrLabelLevel_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var label = (XRLabel) sender;
        var currentColumnValue = GetCurrentColumnValue("empFileNum");
        if (currentColumnValue == null)
        {
            return;
        }

        var empFileNum = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(empFileNum))
        {
            return;
        }

        var studentId = decimal.Parse(empFileNum);
        var student = db.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);
        if (student != null)
        {
            label.Text = student.LEVEL_DESC;
        }
    }

    private void xrLabelFirstLevelAppoval_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var label = (XRLabel) sender;
        var currentColumnValue = GetCurrentColumnValue("FirstApprove");
        if (currentColumnValue == null)
        {
            return;
        }

        var firstApprove = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(firstApprove))
        {
            return;
        }

        var userId = decimal.Parse(firstApprove);
        var user = db.DashBoard_Users.FirstOrDefault(x => x.ID == userId);
        if (user != null)
        {
            label.Text = user.Username;
        }
    }

    private void xrLabelSecondLevelAppoval_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var label = (XRLabel) sender;
        var currentColumnValue = GetCurrentColumnValue("ScondApprove");
        if (currentColumnValue == null)
        {
            return;
        }

        var secondApprove = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(secondApprove))
        {
            return;
        }

        var userId = decimal.Parse(secondApprove);
        var user = db.DashBoard_Users.FirstOrDefault(x => x.ID == userId);
        if (user != null)
        {
            label.Text = user.Username;
        }
    }

    private void xrLabelThirdLevelAppoval_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
    {
        var label = (XRLabel) sender;
        var currentColumnValue = GetCurrentColumnValue("ApprovedBy");
        if (currentColumnValue == null)
        {
            return;
        }
        var thirdApprove = currentColumnValue.ToString();

        if (string.IsNullOrEmpty(thirdApprove))
        {
            return;
        }

        var userId = decimal.Parse(thirdApprove);
        var user = db.DashBoard_Users.FirstOrDefault(x => x.ID == userId);
        if (user != null)
        {
            label.Text = user.Username;
        }
    }
}