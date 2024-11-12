using ConsoleAppJobNotificacaoHttp.Models;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

logger.Information("Iniciando a execucao do Job...");

try
{
    var infoExecucao = new InfoExecucao()
    {
        Instancia = Environment.MachineName,
        Kernel = Environment.OSVersion.VersionString,
        Framework = RuntimeInformation.FrameworkDescription,
        Horario = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    };
    using var httpClient = new HttpClient();
    var response = await httpClient.PostAsJsonAsync<InfoExecucao>(
        configuration["EndpointNotificacao"], infoExecucao);
    response.EnsureSuccessStatusCode();
    logger.Information("Notificacao enviada com sucesso!");
    logger.Information($"Dados transmitidos = {JsonSerializer.Serialize(infoExecucao)}");
    logger.Information("Job executado com sucesso!");
}
catch (Exception ex)
{
    logger.Error($"Erro durante a execucao do Job: {ex.Message}");
    Environment.ExitCode = 1;
}