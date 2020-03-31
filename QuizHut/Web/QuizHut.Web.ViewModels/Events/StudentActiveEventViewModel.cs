namespace QuizHut.Web.ViewModels.Events
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class StudentActiveEventViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string QuizId { get; set; }

        public string QuizName { get; set; }

        public int QuestionsCount { get; set; }

        public string TimeToTake { get; set; }

        public string Duration { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, StudentActiveEventViewModel>()
                .ForMember(
                    x => x.QuestionsCount,
                    opt => opt.MapFrom(
                        x => x.Quiz.Questions.Count))
                .ForMember(
                    x => x.Duration,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.ToLocalTime().Hour.ToString("D2")}:{x.ActivationDateAndTime.ToLocalTime().Minute.ToString("D2")}" +
                        $" - {x.ActivationDateAndTime.ToLocalTime().Add(x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.ToLocalTime().Add(x.DurationOfActivity).Minute.ToString("D2")}"))
                .ForMember(
                    x => x.TimeToTake,
                    opt => opt.MapFrom(x => x.Quiz.Timer != null ? $"{x.Quiz.Timer.ToString()} minutes" : "no time limit"));
        }
    }
}
