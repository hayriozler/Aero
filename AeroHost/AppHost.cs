var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5050)
    .WithDataVolume("aeroflight-postgres-data");

// Only the catalog (tenant registry) DB is provisioned here; tenant DBs (including each tenant's
// own FIDS database) are created by Migrations.
var tenantDb = postgres.AddDatabase("tenant");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("aeroflight-rabbitmq-data")
    .WithManagementPlugin();

//var migrations = builder.AddProject<Projects.AeroFlight_Migrations>("migrations")
//    .WithReference(tenantDb)
//    .WaitFor(tenantDb);

//var api = builder.AddProject<Projects.AeroFlight_Api>("api")
//    .WithReference(tenantDb)
//    .WithReference(rabbitmq)
//    .WaitForCompletion(migrations);

//builder.AddProject<Projects.AeroFlight_Web>("web")
//    .WithReference(rabbitmq)
//    .WithReference(api)
//    .WaitForCompletion(migrations);

//builder.AddProject<Projects.AeroFids_Service>("aerofids")
//    .WithReference(rabbitmq)
//    .WithReference(api)
//    .WithReference(tenantDb)
//    .WaitFor(rabbitmq)
//    .WaitForCompletion(migrations);

builder.Build().Run();
