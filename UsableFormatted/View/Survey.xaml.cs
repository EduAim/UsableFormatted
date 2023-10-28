using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsableFormatted.Repos;
using UsfoModels;
using DocumentFormat.OpenXml.Office2013.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for Survey.xaml
    /// </summary>
    public partial class Survey : UserControl
    {
        public Survey()
        {
            InitializeComponent();
            IsVisibleChanged += Survey_IsVisibleChanged;
        }

        private void Survey_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            foreach (var rb in SurveyGroup1.Children.OfType<RadioButton>())
                rb.IsChecked = false;
            
            foreach (var rb in SurveyGroup2.Children.OfType<RadioButton>())
                rb.IsChecked = false;
            
            foreach (var rb in SurveyGroup3.Children.OfType<RadioButton>())
                rb.IsChecked = false;
        }

        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (bool isSet, byte survey1, byte survey2, byte survey3) = CollectValues();

                var profile = UserProfileRepo.LoggedInUser ?? new UserProfile();

                var surveyAnswers = new SurveyAnswers
                {
                    Survey1 = survey1,
                    Survey2 = survey2,
                    Survey3 = survey3,
                    FontSize = (int)_M._currentFormatSet.FontSize,
                    HeadingSize = (int)_M._currentFormatSet.HeadingFontSize,
                    FontName = _M._currentFormatSet.Font,
                    UserName = profile?.Email,
                };

                _ = SurveyController.SendSurvey(surveyAnswers);
            }
            catch(Exception ex)
            {
                ex.TraceEx();
            }

            _M._mainWindow.SetPage(EPages.FileUpload);
        }

        private (bool isSet, byte survey1, byte survey2, byte survey3) CollectValues()
        {
            var isSet = true;
            byte s1 = 0, s2 = 0, s3 = 0;
            string? survey_1 = SurveyGroup1.Children.OfType<RadioButton>().FirstOrDefault(x => x.IsChecked == true)?.Tag as string;
            if (survey_1 == null || !byte.TryParse(survey_1, out s1))
                isSet = false;

            string? survey_2 = SurveyGroup2.Children.OfType<RadioButton>().FirstOrDefault(x => x.IsChecked == true)?.Tag as string;
            if (string.IsNullOrEmpty(survey_2) || !byte.TryParse(survey_2, out s2))
                isSet = false;

            string? survey_3 = SurveyGroup2.Children.OfType<RadioButton>().FirstOrDefault(x => x.IsChecked == true)?.Tag as string;
            if (string.IsNullOrEmpty(survey_3) || !byte.TryParse(survey_3, out s3))
                isSet = false;

            return (isSet, s1, s2, s3);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetPage(EPages.FileUpload);
        }
    }
}
