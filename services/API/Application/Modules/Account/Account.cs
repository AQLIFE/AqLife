using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Account.Search;
using AqLife.Application.Account.Services;
using AqLife.Application.Business.Account;
using AqLife.Domain.Entities;
using Core.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Application.Account
{
    public static class Account
    {
        public static IServiceCollection AddAccount(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, DefaultAccount>();
            services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, ValidAccount>();
            services.AddScoped<ISearch<AccountQuery, AccountDto, AccountEntity>, AccountSearch>();

            services.AddSingleton<SubscriptionMapper>();
            services.AddSingleton<IViewMapper<SubscriptionEntity, SubscriptionDto>>(sp => sp.GetRequiredService<SubscriptionMapper>());
            services.AddSingleton<ICreateMapper<SubscriptionEntity, ISubscription>>(sp => sp.GetRequiredService<SubscriptionMapper>());

            services.AddSingleton<AccountMapper>();
            services.AddSingleton<IViewMapper<AccountEntity, AccountDto>>(sp => sp.GetRequiredService<AccountMapper>());
            services.AddSingleton<ICreateMapper<AccountEntity, CreateAccountCommand>>(sp => sp.GetRequiredService<AccountMapper>());
            services.AddSingleton<ISearchMapper<AccountQuery, AccountDto, EntitySearchCriteria>>(sp => sp.GetRequiredService<AccountMapper>());

            services.AddScoped<IFileReferenceProvider, AccountFileReferenceProvider>();
            services.AddScoped<SystemInitializationService>();


            return services;
        }
    }
}
