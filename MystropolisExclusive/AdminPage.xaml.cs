using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Data;
using System.Collections.ObjectModel;
using MystropolisExclusive.DataAccess;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MystropolisExclusive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        public MysticlusiveCode editingCode = null;

        public ObservableCollection<MysticlusiveCode> collection = new ObservableCollection<MysticlusiveCode>();

        public AdminPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSettings();
            LoadData();
        }

        private void InitializeSettings()
        {
            CountdownDuration.Text = Settings.CountdownDuration.ToString();
            VideoFolderName.Text = Settings.VideoFolderName;
        }

        private void LoadData()
        {
            var allCodes = DataAccess.DataAccess.GetAllCodes();

            collection.Clear();
            foreach (var row in allCodes)
            {
                collection.Add(row);
            }

        }

        private void CodesGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var currentSortDirection = e.Column.SortDirection;

            foreach (var column in CodesGrid.Columns)
            {
                column.SortDirection = null;
            }

            var sortOrder = "ASC";

            if ((currentSortDirection == null || currentSortDirection == DataGridSortDirection.Descending))
            {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                sortOrder = "DESC";
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            IOrderedEnumerable<MysticlusiveCode> orderedCodes = null;
            switch (e.Column.Header.ToString())
            {
                case nameof(MysticlusiveCode.Code) when sortOrder == "ASC":
                    orderedCodes = collection.ToArray().OrderBy(x => x.Code);
                    break;
                case nameof(MysticlusiveCode.Code) when sortOrder == "DESC":
                    orderedCodes = collection.ToArray().OrderByDescending(x => x.Code);
                    break;
                case nameof(MysticlusiveCode.Video) when sortOrder == "ASC":
                    orderedCodes = collection.ToArray().OrderBy(x => x.Video);
                    break;
                case nameof(MysticlusiveCode.Video) when sortOrder == "DESC":
                    orderedCodes = collection.ToArray().OrderByDescending(x => x.Video);
                    break;
                case nameof(MysticlusiveCode.Used) when sortOrder == "ASC":
                    orderedCodes = collection.ToArray().OrderBy(x => x.Used);
                    break;
                case nameof(MysticlusiveCode.Used) when sortOrder == "DESC":
                    orderedCodes = collection.ToArray().OrderByDescending(x => x.Used);
                    break;
            }

            if (orderedCodes != null)
            {
                collection.Clear();
                foreach (var code in orderedCodes)
                {
                    collection.Add(code);
                }
            }

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            editingCode = button.DataContext as MysticlusiveCode;
            if (editingCode != null)
            {
                CancelButton.Visibility = Visibility.Visible;
                Used.Visibility = Visibility.Visible;

                Code.Text = editingCode.Code;
                Video.Text = editingCode.Video;
                Used.IsChecked = editingCode.Used;
                OneTimeUse.IsChecked = editingCode.OneTimeUse;
                MinimumDuration.Text = editingCode.MinimumDuration?.ToString() ?? string.Empty;
                Remarks.Text = editingCode.Remarks ?? string.Empty;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelEdit();
        }

        private void CancelEdit()
        {
            if (editingCode != null)
            {
                ClearForm();

                editingCode = null;
                CodesGrid.SelectedIndex = -1;
                CancelButton.Visibility = Visibility.Collapsed;
                Used.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearForm()
        {
            Code.Text = string.Empty;
            Video.Text = string.Empty;
            Used.IsChecked = false;
            OneTimeUse.IsChecked = true;
            MinimumDuration.Text = string.Empty;
            Remarks.Text = string.Empty;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Code.Text))
            {
                ShowError("Code cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(Video.Text))
            {
                ShowError("Video cannot be empty");
                return;
            }

            if (editingCode != null)
            {
                editingCode.Code = Code.Text;
                editingCode.Video = Video.Text;
                editingCode.Used = Used.IsChecked ?? false;
                editingCode.OneTimeUse = OneTimeUse.IsChecked ?? false;
                editingCode.MinimumDuration = !string.IsNullOrEmpty(MinimumDuration.Text) ? int.Parse(MinimumDuration.Text) : (int?)null;
                editingCode.Remarks = Remarks.Text;

                DataAccess.DataAccess.SaveCode(editingCode);
                LoadData();
                CancelEdit();
                ShowToastNotification("Saved", "Code was successfully saved!");
            }
            else
            {

                var code = new MysticlusiveCode
                {
                    Code = Code.Text,
                    Video = Video.Text,
                    Used = false,
                    OneTimeUse = OneTimeUse.IsChecked ?? false,
                    MinimumDuration = !string.IsNullOrEmpty(MinimumDuration.Text) ? int.Parse(MinimumDuration.Text) : (int?)null,
                    Remarks = Remarks.Text,
                };

                DataAccess.DataAccess.InsertCode(code);
                LoadData();
                ClearForm();
                ShowToastNotification("Created", "Code was successfully created!");
            }

            HideError();
        }

        private void ShowError(string message)
        {
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Text = message;
        }

        private void HideError()
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            ErrorMessage.Text = string.Empty;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            editingCode = button.DataContext as MysticlusiveCode;
            if (editingCode != null)
            {

                ContentDialog deletionPrompt = new ContentDialog
                {
                    Title = "Are you sure you want to delete?",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Yes"
                };

                ContentDialogResult result = await deletionPrompt.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    DataAccess.DataAccess.DeleteCode(editingCode.Id);
                    LoadData();
                    CancelEdit();
                    ShowToastNotification("Deleted", "Code was successfully deleted!");
                }
            }
        }

        private void MystiSettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (!string.IsNullOrEmpty(CountdownDuration.Text) && int.TryParse(CountdownDuration.Text, out var duration))
            {
                localSettings.Values[Settings.Keys.CountdownDuration] = duration;
            }

            if (!string.IsNullOrEmpty(VideoFolderName.Text))
            {
                localSettings.Values[Settings.Keys.VideoFolderName] = VideoFolderName.Text;
            }

            ShowToastNotification("Save", "Settings succesfully saved!");
        }

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
