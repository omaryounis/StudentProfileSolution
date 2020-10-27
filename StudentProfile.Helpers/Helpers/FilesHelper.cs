using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace StudentProfile.Components
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
