using System.Reflection;
using AqLife.Application.Account;
using Category;
using Corpus;
using Favorite;
using File;
using Microsoft.Extensions.DependencyInjection;
using Plan;
using Tag;

namespace AqLife.Application;

public static class Application
{
    /// <summary>
    /// Assemblies containing application handlers, validators and module registrations.
    /// The host uses this list to register MediatR and application validators consistently.
    /// </summary>
    public static IReadOnlyCollection<Assembly> Assemblies { get; } =
    [
        typeof(Application).Assembly,
        typeof(AqLife.Application.Account.Account).Assembly,
        typeof(global::Category.Category).Assembly,
        typeof(global::Corpus.Corpus).Assembly,
        typeof(global::Favorite.Favorite).Assembly,
        typeof(global::File.File).Assembly,
        typeof(global::Plan.Plan).Assembly,
        typeof(global::Tag.Tag).Assembly
    ];

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddFile()
            .AddAccount()
            .AddTag()
            .AddCategory()
            .AddFavorite()
            .AddCorpus()
            .AddPlan();

        return services;
    }
}