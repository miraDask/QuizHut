namespace QuizHut.Web.ViewModels.Categories
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class CategoryViewModel : IMapFrom<Category>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string QuizzesCount { get; set; }

        public string CreatedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Category, CategoryViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    opt => opt.MapFrom(x => x.CreatedOn.ToString("dd/MM/yyyy")))
                .ForMember(
                    x => x.QuizzesCount,
                    opt => opt.MapFrom(x => x.Quizzes.Count));
        }
    }
}
