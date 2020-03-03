namespace QuizHut.Web.ViewModels.Teachers
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class TeacherViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, TeacherViewModel>()
               .ForMember(
                   x => x.FullName,
                   opt => opt.MapFrom(x => $"{x.FirstName} {x.LastName}"));
        }
    }
}
