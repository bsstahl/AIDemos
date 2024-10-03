using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var container = new ServiceCollection()
    .AddSingleton<Solutionizer.Agents.Engine>()
    .AddSingleton<IConfiguration>(c => config)
    .BuildServiceProvider();

var prog = container.GetRequiredService<Solutionizer.Agents.Engine>();
await prog.Execute().ConfigureAwait(false);
