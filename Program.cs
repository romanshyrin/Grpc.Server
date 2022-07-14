using GrpcServer.Data;
using GrpcServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();
builder.Services.AddDbContext<DataContext>(X =>
    X.UseNpgsql(builder.Configuration.GetConnectionString("StudentsDb") ?? string.Empty));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ServicesStudent>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();