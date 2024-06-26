﻿using Application.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
            .AddAutoMapper(typeof(MappingProfiles))
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
    }
}

/*
Assembly, .NET platformunda bir uygulamanın derlenmiş kodunu içeren bir birimdir. 
Assembly.GetExecutingAssembly() metodu, mevcut çalışan uygulamanın (yani bu kodun derlendiği) derlenmiş kodunu temsil eden Assembly nesnesini döndürür.
 
AddMediatR metodu ile MediatR kütüphanesinin kullanımını kolaylaştırmaktır. 
AddMediatR metodu, MediatR kütüphanesini ve bağlılıklarını IServiceCollection'a ekler. 
Bu, özellikle MediatR tarafından tanınan ve otomatik olarak kaydedilen hizmetleri içerir. 
Bu nedenle Assembly.GetExecutingAssembly() kullanılarak mevcut uygulamanın derlenmiş kodunun bulunduğu assembly alınır ve bu assembly içindeki hizmetler AddMediatR metoduna kaydedilir.
 */