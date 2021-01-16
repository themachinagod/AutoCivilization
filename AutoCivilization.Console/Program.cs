﻿using AutoCivilization.Abstractions;
using AutoCivilization.Abstractions.ActionSteps;
using AutoCivilization.Abstractions.FocusCardResolvers;
using AutoCivilization.Abstractions.TechnologyResolvers;
using AutoCivilization.ActionSteps;
using AutoCivilization.FocusCardResolvers;
using AutoCivilization.TechnologyResolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoCivilization.Console
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) =>
                {
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AutoCivService>();
                    
                    // state services
                    services.AddSingleton<IBotGameStateService, BotGameStateService>();
                    services.AddScoped<IBotMoveStateService, BotMoveStateService>();
                    
                    // game initialisers
                    services.AddTransient<IFocusCardDeckInitialiser, FocusCardDeckInitialiser>();
                    services.AddTransient<IFocusBarInitialiser, FocusBarInitialiser>();
                    services.AddTransient<ILeaderCardInitialiser, LeaderCardInitialiser>();

                    // culture action steps
                    services.AddTransient<ITokenPlacementCityAdjacentActionRequest, TokenPlacementCityAdjacentActionRequest>();
                    services.AddTransient<ITokenPlacementTerritoryAdjacentActionRequest, TokenPlacementTerritoryAdjacentActionRequest>();
                    services.AddTransient<ITokenPlacementCityAdjacentInformationRequest, TokenPlacementCityAdjacentInformationRequest>();
                    services.AddTransient<ITokenPlacementTerritoryAdjacentInformationRequest, TokenPlacementTerritoryAdjacentInformationRequest>();
                    services.AddTransient<ITokenPlacementNaturalWondersInformationRequest, TokenPlacementNaturalWondersInformationRequest>();
                    services.AddTransient<ITokenPlacementNaturalResourcesInformationRequest, TokenPlacementNaturalResourcesInformationRequest>();
                    services.AddTransient<ITokenFlipEnemyActionRequest, TokenFlipEnemyActionRequest>();

                    // focus card resolvers
                    services.AddTransient<IFocusCardResolverFactory, FocusCardResolverFactory>();
                    services.AddTransient<ICultureLevel1FocusCardResolver, EarlyEmpireFocusCardResolver>();
                    services.AddTransient<ICultureLevel2FocusCardResolver, DramaPoetryFocusCardResolver>();
                    services.AddTransient<ICultureLevel3FocusCardResolver, CivilServiceFocusCardResolver>();
                    services.AddTransient<ICultureLevel4FocusCardResolver, MassMediaFocusCardResolver>();
                    services.AddTransient<IScienceLevel1FocusCardResolver, AstrologyFocusCardResolver>();
                    services.AddTransient<IScienceLevel2FocusCardResolver, MathematicsFocusCardResolver>();
                    services.AddTransient<IScienceLevel3FocusCardResolver, ReplaceablePartsCardResolver>();
                    services.AddTransient<IScienceLevel4FocusCardResolver, NuclearPowerFocusCardResolver>();

                    // technology resolvers
                    services.AddTransient<ITechnologyLevelModifier, TechnologyLevelModifier>();
                    services.AddTransient<IFocusBarTechnologyUpgradeResolver, FocusBarTechnologyUpgradeResolver>();
                    services.AddTransient<ITechnologyBreakthroughResolver, TechnologyBreakthroughResolver>();
                });
    }
}
