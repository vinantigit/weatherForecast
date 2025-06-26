# weatherForecast

DuploCloud Take Home Assignment 

Azure-based Weather forecast RESTful API 

Git Repo: https://github.com/vinantigit/weatherForecast 

Tech Stack: 
• .NET Core (.NET 8) Web API 
• Entity Framework Core with Azure Cosmos DB 
• Integration with Open-Meteo API 
• Swagger for API documentation 
• xUnit for test coverage 

Divided Assignment into 3 stages: 
Stage 1: Basic Working API 
• Integrate Open-Meteo API 
• Use memory storage 
• Add endPoint to achieve listed features 
Stage 2: Persistence and structured DB access 
• SQLite DB Storage 
• Entity Framework 
• Validations, Exception handling, Logging 
Stage 3: Cloud base access 
• Add API Authentication 
• Migrate from SQLite to Azure CosmosDB 
• Deploy API to Azure cloud 
• Catching 
• Azure Monitors 

Features: 
1. Add new coordinate pair (latitude/longitude) → fetch & persist forecast 
2. Get current forecast for a coordinate 
3. Delete a stored coordinate 
4. List all stored coordinates 
5. Refresh and return the latest forecast for a selected coordinate 

Architecture (Layered): 

Detailed Architecture (Draft later) 

Endpoints: 
Method Route 
POST 
/api/coordinates 
Description 
Add lat/lon, fetch & 
store forecast 
GET 
/api/forecast?latitude=<latitude>&longitude=<longitude> Get forecast for 
coordinates 
DELETE /api/coordinates/{id} 
Delete coordinate 
from DB 
GET 
/api/coordinates 
List stored 
coordinates 
PUT 
/api/forecast/refresh/{id} 
Refresh forecast for 
saved coordinate 