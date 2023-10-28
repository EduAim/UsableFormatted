using UsfoModels.Model;

namespace UsfoModels
{
    public class Recommendations
    {
        public static FormatSet GetByAge(int age)
        {
            var recommendations = DefaultRecommendations.Recommentations.Where(x => x.MinAge <= age && x.MaxAge >= age).FirstOrDefault();
            if (recommendations == null)
                recommendations = DefaultRecommendations.DefaultSet;

            return recommendations;
        }
    }
}