namespace QuizHut.Web.ViewModels.Quizzes
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EditDetailsViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ActivationDate { get; set; }

        public int? Duration { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, EditDetailsViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty));
        }
    }
}
