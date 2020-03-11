namespace QuizHut.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Common;

    public class EventListViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string StartsOn { get; set; }

        public string ResultStatistic { get; set; }

        public IDictionary<string, string> Status { get; set; }/* = new Dictionary<string, string>();*/

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventListViewModel>()
                .ForMember(
                    x => x.StartsOn,
                    opt => opt.MapFrom(x => $"{x.ActivationDateAndTime.Date.ToString("dd/MM/yyyy")} - {x.ActivationDateAndTime.Hour.ToString("D2")}:{x.ActivationDateAndTime.Minute.ToString("D2")}"))
                .ForMember(
                    x => x.ResultStatistic,
                    opt => opt.MapFrom(x => $"{x.Quiz.QuizzesResults.Count}/{x.Group.StudentstGroups.Count}"))
                .ForMember(
                    x => x.Status,
                    opt => opt.MapFrom(x => GetStatus(x.ActivationDateAndTime, x.DurationOfActivity)));
        }

        private static IDictionary<string, string> GetStatus(DateTime activationDateAndTime, TimeSpan duration)
        {
            var now = DateTime.UtcNow;
            var end = activationDateAndTime.Add(duration);

            if (now < activationDateAndTime)
            {
                return new Dictionary<string, string>()
                {
                    [ModelCostants.Status] = ModelCostants.StatusPending,
                    [ModelCostants.Color] = ModelCostants.ColorPending,
                };
            }

            if (now > end)
            {
                return new Dictionary<string, string>()
                {
                    [ModelCostants.Status] = ModelCostants.StatusEnded,
                    [ModelCostants.Color] = ModelCostants.ColorEnded,
                };
            }

            return new Dictionary<string, string>()
            {
                [ModelCostants.Status] = ModelCostants.StatusActive,
                [ModelCostants.Color] = ModelCostants.ColorActive,
            };
        }
    }
}
