using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cvapp.DAL.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.IntegrationTests.config;

using System.Data.Common;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


public class CustomWebApplicationFactoryWithMockAuth<TProgram>
: WebApplicationFactory<TProgram> where TProgram : class 
{

    protected override void ConfigureWebHost(IWebHostBuilder builder) 
    {
        builder.ConfigureServices(services =>
        {

        var dbContextDescriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<CvAppDbContext>));

        services.Remove(dbContextDescriptor);
        var dbConnectionDescriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbConnection));

        services.Remove(dbConnectionDescriptor);
        // Create open SqliteConnection so EF won't automatically close it.

        services.AddSingleton<DbConnection>(_ => {
            var connection = new SqliteConnection("DataSource=:memory:"); connection.Open();
            return connection;
        });
        services.AddDbContext<CvAppDbContext>((container, options) =>
        {
        var connection = container.GetRequiredService<DbConnection>();
        options.UseSqlite(connection); });
        });
        builder.UseEnvironment("Development");
    }
    
    public WebApplicationFactory<TProgram> AuthenticatedInstance(params Claim[] claimSeed)
    {
        return WithWebHostBuilder(builder =>
        { 
            builder.ConfigureTestServices(services =>
        
            {
                services.AddSingleton<IAuthenticationSchemeProvider, MockSchemeProvider>();
                services.AddSingleton<MockClaimSeed>(_ => new(claimSeed)); 
            }); 
        });
    }
    public class MockSchemeProvider(IOptions<AuthenticationOptions> options) : AuthenticationSchemeProvider(options)
    {
        public override Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
        
            AuthenticationScheme mockScheme = new(
        IdentityConstants.ApplicationScheme,
        IdentityConstants.ApplicationScheme, typeof(MockAuthenticationHandler));
        
            return Task.FromResult(mockScheme);
        }
    }
    [Obsolete("Obsolete")]
    public class MockAuthenticationHandler(
        MockClaimSeed claimSeed,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claimsIdentity = new ClaimsIdentity(claimSeed.GetSeeds(),
        IdentityConstants.ApplicationScheme);
        
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
            var ticket = new AuthenticationTicket(claimsPrincipal, IdentityConstants.ApplicationScheme);
        
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
    public class MockClaimSeed(IEnumerable<Claim> seed)
    {
        public IEnumerable<Claim> GetSeeds() => seed;
    } 
}