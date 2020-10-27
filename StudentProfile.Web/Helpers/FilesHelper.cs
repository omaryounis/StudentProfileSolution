using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Dashboard_StudentProfile.Cls
{
    public static class FilesHelper
    {
        public static UploadControlValidationSettings GetValidatingSettings()
        {
            UploadControlValidationSettings s = new UploadControlValidationSettings();
            s.AllowedFileExtensions = new string[] { ".*" };
            s.NotAllowedFileExtensionErrorText = "غير مسموح بهذا الامتداد";
            s.MaxFileSize = 4194304;
            s.ShowErrors = false;
            return s;
        }


    }



        public class FileManagerDemoHelper
        {

            static HttpContext Context { get { return HttpContext.Current; } }

            public static readonly object FileManagerFolder = "~/Content/UserFiles";
            public static readonly object RootFolder = "~/Content/UserFiles";
            public static readonly object VirtualScrollingRootFolder = "~/Content/UserFiles";
            public static readonly object ImagesRootFolder = "~/Content/UserFiles";
            public static readonly string[] AllowedFileExtensions = new string[] {
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".avi", ".png", ".mp3", ".xml", ".doc", ".pdf"
        };
            public static readonly string[] VirtualScrollingAllowedFileExtensions = new string[] {
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".png", ".doc", ".pdf", ".xls", ".xlsx"
        };
            public static FileManagerFeaturesDemoOptions FeaturesDemoOptions
            {
                get
                {
                    if (Context.Session["FeaturesDemoOptions"] == null)
                        Context.Session["FeaturesDemoOptions"] = new FileManagerFeaturesDemoOptions();
                    return (FileManagerFeaturesDemoOptions)Context.Session["FeaturesDemoOptions"];
                }
                set { Context.Session["FeaturesDemoOptions"] = value; }
            }
            public static FileListView VirtualScrollingViewMode
            {
                get
                {
                    if (Context.Session["VirtualScrollingViewMode"] == null)
                        return FileListView.Thumbnails;
                    return (FileListView)Context.Session["VirtualScrollingViewMode"];
                }
                set { Context.Session["VirtualScrollingViewMode"] = value; }
            }
            public static int VirtualScrollingPageSize
            {
                get
                {
                    if (Context.Session["VirtualScrollingPageSize"] == null)
                        return 300;
                    return (int)Context.Session["VirtualScrollingPageSize"];
                }
                set { Context.Session["VirtualScrollingPageSize"] = value; }
            }
            public static DevExpress.Web.Mvc.FileManagerSettings CreateFileManagerDownloadSettings()
            {
                var settings = new DevExpress.Web.Mvc.FileManagerSettings();
                settings.SettingsEditing.AllowDownload = true;

                settings.Name = "fileManager";
                return settings;
            }
            public static DevExpress.Web.Mvc.FileManagerSettings CreateMultipleFilesSelectionDownloadSettings()
            {
                var settings = new DevExpress.Web.Mvc.FileManagerSettings();
                settings.SettingsEditing.AllowDownload = true;

                settings.Name = "fileManager";
                return settings;
                //return CreateFileManagerDownloadSettingsCore(editingSettings);
            }
            public static DevExpress.Web.Mvc.FileManagerSettings CreateVirtualScrollingDownloadSettings()
            {
                FileManagerSettingsEditing editingSettings = CreateFileManagerEditingSettings();
                return CreateFileManagerDownloadSettingsCore(editingSettings);
            }
            static DevExpress.Web.Mvc.FileManagerSettings CreateFileManagerDownloadSettingsCore(FileManagerSettingsEditing editingSettings)
            {
                var settings = new DevExpress.Web.Mvc.FileManagerSettings();
                settings.SettingsEditing.Assign(editingSettings);
                settings.Name = "fileManager";
                return settings;
            }
            public static FileManagerSettingsEditing CreateFileManagerEditingSettings()
            {
                return new FileManagerSettingsEditing(null)
                {
                    AllowCreate = true,
                    AllowMove = true,
                    AllowDelete = true,
                    AllowRename = true,
                    AllowCopy = true,
                    AllowDownload = true
                };
            }


            public static List<SelectListItem> GetSecurityRoles()
            {
                return new List<SelectListItem>() {
                new SelectListItem() { Text = "Default User", Value = SecurityRole.DefaultUser.ToString(), Selected = true },
                new SelectListItem() { Text = "Document Manager", Value = SecurityRole.DocumentManager.ToString() },
                new SelectListItem() { Text = "Media Moderator", Value = SecurityRole.MediaModerator.ToString() },
                new SelectListItem() { Text = "Administrator", Value = SecurityRole.Administrator.ToString() }
            };
            }
            public static List<SelectListItem> GetFileListViewModes()
            {
                return new List<SelectListItem>() {
                new SelectListItem() { Text = FileListView.Thumbnails.ToString(), Value = FileListView.Thumbnails.ToString(), Selected = true },
                new SelectListItem() { Text = FileListView.Details.ToString(), Value = FileListView.Details.ToString() }
            };
            }
            public static List<SelectListItem> GetFileListPageSizes()
            {
                return new List<SelectListItem>() {
                new SelectListItem() { Text = "50", Value = "50" },
                new SelectListItem() { Text = "100", Value = "100" },
                new SelectListItem() { Text = "300", Value = "300", Selected = true },
                new SelectListItem() { Text = "500", Value = "500" }
            };
            }
        }

    public class FileManagerFeaturesDemoOptions
    {
        FileManagerSettingsEditing settingsEditing;
        FileManagerSettingsToolbar settingsToolbar;
        FileManagerSettingsFolders settingsFolders;
        FileManagerSettingsFileList settingsFileList;
        FileManagerSettingsBreadcrumbs settingsBreadcrumbs;
        MVCxFileManagerSettingsUpload settingsUpload;

        public FileManagerFeaturesDemoOptions()
        {
            this.settingsEditing = new FileManagerSettingsEditing(null)
            {
                AllowCreate = false,
                AllowMove = false,
                AllowDelete = false,
                AllowRename = false,
                AllowCopy = true,
                AllowDownload = true
            };
            this.settingsToolbar = new FileManagerSettingsToolbar(null)
            {
                ShowPath = false,
                ShowFilterBox = true
            };
            this.settingsFolders = new FileManagerSettingsFolders(null)
            {
                Visible = false,
                EnableCallBacks = false,
                ShowFolderIcons = true,
                ShowLockedFolderIcons = true
            };
            this.settingsFileList = new FileManagerSettingsFileList(null)
            {
                ShowFolders = false,
                ShowParentFolder = false
            };
            this.settingsBreadcrumbs = new FileManagerSettingsBreadcrumbs(null)
            {
                Visible = false,
                ShowParentFolderButton = false,
                Position = BreadcrumbsPosition.Top
            };
            this.settingsUpload = new MVCxFileManagerSettingsUpload();
            this.settingsUpload.Enabled = true;
            this.settingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
        }

        [Display(Name = "Settings Editing")]
        public FileManagerSettingsEditing SettingsEditing { get { return settingsEditing; } }
        [Display(Name = "Settings Toolbar")]
        public FileManagerSettingsToolbar SettingsToolbar { get { return settingsToolbar; } }
        [Display(Name = "Settings Folders")]
        public FileManagerSettingsFolders SettingsFolders { get { return settingsFolders; } }
        [Display(Name = "Settings FileList")]
        public FileManagerSettingsFileList SettingsFileList { get { return settingsFileList; } }
        [Display(Name = "Settings Breadcrumbs")]
        public FileManagerSettingsBreadcrumbs SettingsBreadcrumbs { get { return settingsBreadcrumbs; } }
        [Display(Name = "Settings Upload")]
        public MVCxFileManagerSettingsUpload SettingsUpload { get { return settingsUpload; } }
    }
    public enum SecurityRole { DefaultUser, DocumentManager, MediaModerator, Administrator }
    public class CustomValidationHelper
    {
        static Action<TextBoxSettings> textBoxSettingsMethod;
        static Action<MVCxFormLayoutItem> formLayoutItemSettingsMethod;
        public static Action<TextBoxSettings> TextBoxSettingsMethod
        {
            get
            {
                if (textBoxSettingsMethod == null)
                    textBoxSettingsMethod = CreateTextBoxSettingsMethod();
                return textBoxSettingsMethod;
            }
        }
        public static Action<MVCxFormLayoutItem> FormLayoutItemSettingsMethod
        {
            get
            {
                if (formLayoutItemSettingsMethod == null)
                    formLayoutItemSettingsMethod = CreateFormLayoutItemSettingsMethod();
                return formLayoutItemSettingsMethod;
            }
        }
        public static string GetCaptionFromModel(string propertyName, Type modelType)
        {
            var info = modelType.GetMember(propertyName);
            var attributes = info[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            var caption = ((DisplayAttribute)attributes[0]).Name;
            return caption;
        }
        static Action<TextBoxSettings> CreateTextBoxSettingsMethod()
        {
            return settings => {
                settings.Width = Unit.Percentage(100);
                settings.Properties.ValidationSettings.Display = Display.Dynamic;
                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
            };
        }
        static Action<MVCxFormLayoutItem> CreateFormLayoutItemSettingsMethod()
        {
            return itemSettings => {
                itemSettings.Width = Unit.Percentage(100);
                dynamic editorSettings = itemSettings.NestedExtensionSettings;
                editorSettings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                editorSettings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                editorSettings.Properties.ValidationSettings.Display = Display.Dynamic;
                editorSettings.ShowModelErrors = true;
                editorSettings.Width = Unit.Percentage(100);
            };
        }
    }
}
