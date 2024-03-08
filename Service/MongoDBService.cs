using MongoDB.Driver;
using System.Threading.Tasks;

public class MongoDBService
{
	private readonly IMongoCollection<UserGeolocation> _geolocations;

	public MongoDBService(string connectionString, string databaseName, string collectionName)
	{
		var client = new MongoClient(connectionString);
		var database = client.GetDatabase(databaseName);
		_geolocations = database.GetCollection<UserGeolocation>(collectionName);
	}

	public async Task InsertGeolocationAsync(UserGeolocation geolocation)
	{
		await _geolocations.InsertOneAsync(geolocation);
	}

}