using UsableFormatted.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UsableFormatted.Controller
{
    internal static class SurveyController
    {
        private const string _token = "RIi6ZzMvivmU63m6dZYS1feQC4nvv/Lo2RV2v9!v3v1PFyHR5NdQw5Xr3SIs/IhP";
        private const string _url = "https://www.eduaim.eu/api/docproc_survey";

        internal static async Task SendSurvey(SurveyAnswers answers)
        {
            HttpClient myHttpClient = new HttpClient();
            myHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("font_size", answers.FontSize.ToString()),
                new KeyValuePair<string, string>("heading_size", answers.HeadingSize.ToString()),
                new KeyValuePair<string, string>("survey_1", answers.Survey1.ToString()),
                new KeyValuePair<string, string>("survey_2", answers.Survey2.ToString()),
                new KeyValuePair<string, string>("survey_3", answers.Survey3.ToString()),
                new KeyValuePair<string, string>("font_name", answers.FontName),
                new KeyValuePair<string, string>("user_name", answers.UserName),
            });

            var response = await myHttpClient.PostAsync(_url, formContent);
            var stringContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(stringContent);
        }

    }
}
