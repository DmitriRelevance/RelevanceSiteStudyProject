var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.RelevanceSiteStudyProject_API>("relevancesitestudyproject-api");

builder.AddProject<Projects.RelevanceSiteStudyProject>("relevancesitestudyproject");

builder.Build().Run();
