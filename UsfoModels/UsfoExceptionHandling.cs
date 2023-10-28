using System.Diagnostics;

namespace UsfoModels
{
    public static class UsfoExceptionHandling
    {
        public static void TraceEx(this Exception ex)
        {
            try
            {
                var message = $"Exception: {ex.Message}{Environment.NewLine}{ex.Source}{Environment.NewLine}{ex.StackTrace}";
                Debug.WriteLine(message);
                var path = Path.Combine(Path.GetTempPath(), "errorlog.txt");
                _ = File.WriteAllLinesAsync(path, new List<string> { message });
            }
            catch (Exception e)
            {
                var message = $"Exception: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}";
                Debug.WriteLine(message);
            }
        }
    }
}