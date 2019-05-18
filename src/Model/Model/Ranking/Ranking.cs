using Model.SurveyCompletion;

namespace Model.Ranking
{
    public class Ranking
    {
        public int Id { get; set; }
        public SurveyCompletionParent SurveyCompletionParentByDemanda { get; set; }
        public SurveyCompletionParent SurveyCompletionParentByOferta { get; set; }
        public int Rank { get; set; }
    }
}
