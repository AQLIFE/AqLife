using System.Reflection;
using AqLife.Application.Account;
using AqLife.Application.Category;
using AqLife.Application.Corpus;
using AqLife.Application.Favorite;
using AqLife.Application.File;
using AqLife.Application.Plan;
using AqLife.Application.Tag;
using Microsoft.Extensions.DependencyInjection;

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
        typeof(global::AqLife.Application.Category.Category).Assembly,
        typeof(global::AqLife.Application.Corpus.Corpus).Assembly,
        typeof(global::AqLife.Application.Favorite.Favorite).Assembly,
        typeof(global::AqLife.Application.File.File).Assembly,
        typeof(global::AqLife.Application.Plan.Plan).Assembly,
        typeof(global::AqLife.Application.Tag.Tag).Assembly
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