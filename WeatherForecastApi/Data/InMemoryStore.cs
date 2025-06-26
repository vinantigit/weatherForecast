namespace WeatherForecastApi.Data
{
    using System.Collections.Generic;
    using WeatherForecastApi.Models;

    //Temporary in-memory database. We will switch with Azure SQL DB OR Azure Cosmos DB
    public static class InMemoryStore
    {
        public static List<Location> Locations { get; set; } = new();
    }
}
