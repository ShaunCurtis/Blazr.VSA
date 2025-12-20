var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blazr_App_Server>("blazr-app-server");

builder.AddProject<Projects.Blazr_App_WASM_Server>("blazr-app-wasm-server");

builder.Build().Run();
