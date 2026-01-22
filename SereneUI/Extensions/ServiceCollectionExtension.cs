using Microsoft.Extensions.DependencyInjection;
using SereneUI.Builders;

namespace SereneUI.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSereneUi(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SereneUiSystem>();
        serviceCollection.AddSingleton<BuildService>();
        serviceCollection.AddSingleton<ButtonBuilder>();
        serviceCollection.AddSingleton<PageBuilder>();
        serviceCollection.AddSingleton<PanelBuilder>();
        serviceCollection.AddSingleton<StackPanelBuilder>();
        serviceCollection.AddSingleton<TextBlockBuilder>();
        serviceCollection.AddSingleton<LineEditBuilder>();
        
        return serviceCollection;
    }
}