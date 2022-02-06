using Xunit;

// We can't run tests in parallel as we need a clean database for each scenario
[assembly: CollectionBehavior(DisableTestParallelization = true)]
