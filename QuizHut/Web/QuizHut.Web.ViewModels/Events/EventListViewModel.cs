﻿namespace QuizHut.Web.ViewModels.Events
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

        public string CreatedOn { get; set; }

        public IDictionary<string, string> Status { get; set; }/* = new Dictionary<string, string>();*/

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventListViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    opt => opt.MapFrom(x => x.CreatedOn.ToString("dd/MM/yyyy")))
                .ForMember(
                    x => x.Status,
                    opt => opt.MapFrom(x => GetStatus(x.ActivationDateAndTime, x.DurationOfActivity)));
        }

        private static IDictionary<string, string> GetStatus(DateTime? activationDateAndTime, TimeSpan? duration)
        {
            if (activationDateAndTime == null)
            {
                return new Dictionary<string, string>()
                {
                    [ModelCostants.Status] = ModelCostants.StatusActive,
                    [ModelCostants.Color] = ModelCostants.ColorActive,
                };
            }

            var now = DateTime.UtcNow;
            if (duration != null)
            {
                var end = activationDateAndTime.Value.Add((TimeSpan)duration);

                if (now < activationDateAndTime)
                {
                    return new Dictionary<string, string>()
                    {
                        [ModelCostants.Status] = ModelCostants.StatusPending,
                        [ModelCostants.Color] = ModelCostants.ColorPending,
                    };
                }
                else if (now > end)
                {
                    return new Dictionary<string, string>()
                    {
                        [ModelCostants.Status] = ModelCostants.StatusEnded,
                        [ModelCostants.Color] = ModelCostants.ColorInActive,
                    };
                }
            }
            else
            {
                if (now.Date < activationDateAndTime.Value.Date)
                {
                    return new Dictionary<string, string>()
                    {
                        [ModelCostants.Status] = ModelCostants.StatusPending,
                        [ModelCostants.Color] = ModelCostants.ColorPending,
                    };
                }
                else if (now.Date > activationDateAndTime.Value.Date)
                {
                    return new Dictionary<string, string>()
                    {
                        [ModelCostants.Status] = ModelCostants.StatusEnded,
                        [ModelCostants.Color] = ModelCostants.ColorInActive,
                    };
                }
            }

            return new Dictionary<string, string>()
            {
                [ModelCostants.Status] = ModelCostants.StatusActive,
                [ModelCostants.Color] = ModelCostants.ColorActive,
            };
        }
    }
}