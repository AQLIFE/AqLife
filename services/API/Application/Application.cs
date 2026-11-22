using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Account;
using AqLife.Shared.Contracts;
using Category;
using Core.Mappers;
using Core.Pipeline.Behaviors;
using Core.Validators;
using Corpus;
using Favorite;
using File;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Plan;
using Tag;

namespace AqLife.Application;

public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddFile().AddAccount().AddTag().AddCategory().AddFavorite().AddCorpus().AddPlan();

        return services;
    }
}
