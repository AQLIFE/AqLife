using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Infrastructure.Configuration
{
    public static class JwtOptionsSetup
    {
        /// <summary>
        /// 测试环境
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            services.AddOptions<JwtOption>().Bind(jwtSection).ValidateOnStart();
            return services;
        }
    }
}
