using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsableFormatted.Model;
using UsableFormatted.Repos;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for UserSettingsView.xaml
    /// </summary>
    public partial class UserSettingsView : UserControl
    {
        private UserProfile _userProfile = new UserProfile();

        public UserSettingsView()
        {
            InitializeComponent();

            FontsComboBox.ItemsSource = DocumentParams.Fonts;
            FontSizeComboBox.ItemsSource = DocumentParams.FontSizes;
            HeadingFontSizeComboBox.ItemsSource = DocumentParams.FontSizes;
            GenderComboBox.ItemsSource = DocumentParams.Genders;
            LanguageComboBox.ItemsSource = DocumentParams.Languages;

            IsVisibleChanged += UserSettingsView_IsVisibleChanged;
        }

        private void UserSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            _userProfile = UserProfileRepo.LoggedInUser;

            BirthYearText.Text = _userProfile.BirthYear > 0 ? _userProfile.BirthYear.ToString() : string.Empty;

            var index = Array.IndexOf(DocumentParams.Fonts, _userProfile.FontName);
            FontsComboBox.SelectedIndex = Math.Max(index, 0);

            index = Array.IndexOf(DocumentParams.FontSizes, _userProfile.FontSize.ToString());
            FontSizeComboBox.SelectedIndex = Math.Max(index, 0);

            index = Array.IndexOf(DocumentParams.FontSizes, _userProfile.HeadingFontSize.ToString());
            HeadingFontSizeComboBox.SelectedIndex = Math.Max(index, 0);

            index = Array.IndexOf(DocumentParams.Genders, _userProfile.Gender);
            GenderComboBox.SelectedIndex = Math.Max(index, 0);

            //index = Array.IndexOf(DocumentParams.Languages, _userProfile.);
            LanguageComboBox.SelectedIndex = 0;
        }



        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var newProfile = new UserProfile
            {
                Id = _userProfile.Id,
                FontName = FontsComboBox.Text,
                Gender = GenderComboBox.Text,
            };

            decimal decVal;
            if (short.TryParse(BirthYearText.Text.Trim(), out var shVal))
                newProfile.BirthYear = shVal;

            if(decimal.TryParse(FontSizeComboBox.Text, out decVal))
                newProfile.FontSize = decVal;

            if(decimal.TryParse(HeadingFontSizeComboBox.Text, out decVal))
                newProfile.HeadingFontSize = decVal;

            var isSaved = UserProfileRepo.UpdateUser(newProfile);
            _M._mainWindow.SetUserProfile(false);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetUserProfile(false);
        }
    }
}
