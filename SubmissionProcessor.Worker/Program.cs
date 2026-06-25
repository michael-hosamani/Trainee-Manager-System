using SubmissionProcessor.Worker.Consumer;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<SubmissionConsumer>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
 
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registration
builder.Services.AddHttpClient("TraineeDirectory.Api", client =>
   {
   client.BaseAddress = new Uri("http://localhost:5190/");
   });

var host = builder.Build();
host.Run();
