using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H3.Model;
using H3;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreLocationController: ControllerBase
    {

        private readonly MongoDBService DBService;

        public StoreLocationController(MongoDBService service)
        {
            DBService = service;
        }

        [HttpPost("insertStore")]
        public async Task<IActionResult> InserStore([FromBody] Store store)
        {
            await DBService.InsertStoreAsync(store);
            return Ok("Store inserted successfully");
        }

        [HttpGet("findNearestStore")]
        public async Task<ActionResult<List<Store>>> FindNearestStores(double lattitude, double longitude, int meters, string brandName)
        {
            try
            {
                var nearestStore = await DBService.FindNearestStoreAsync(lattitude, longitude, meters, brandName);
                if (nearestStore != null && nearestStore.Count > 0)
                    return Ok(nearestStore);
                else
                    return NotFound("No nearby stores found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured finding the store, erro:" + ex.Message);
            }
        }

        // New API endpoint to insert Store mock data
        [HttpPost("insertMockStore")]
        public async Task<IActionResult> InsertMockData()
        {
            try
            {
                await DBService.InsertMockStoresAsync();
                return Ok("Mock stores inserted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while inserting mock stores: " + ex.Message);
            }
        }

        // Endpoint to generate and insert mock data for Copenhagen
        [HttpPost("generateMockStore/Copenhagen")]
        public async Task<IActionResult> GenerateMockDataCopenhagen(int numberOfEntries)
        {
            var mockData = MockDataGeneratorStore.GenerateMockStoreCopenhagen(numberOfEntries);
            await DBService.InsertManyStoresAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Copenhagen generated and inserted successfully.");
        }



        // Endpoint to generate and insert mock data for Aarhus
        [HttpPost("generateMockStore/Aarhus")]
        public async Task<IActionResult> GenerateMockDataAarhus(int numberOfEntries)
        {
            var mockData = MockDataGeneratorStore.GenerateMockDataForAarhus(numberOfEntries);
            await DBService.InsertManyStoresAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Aarhus generated and inserted successfully.");
        }

        // Endpoint to generate and insert mock data for Møn
        [HttpPost("generateMockStore/Mon")]
        public async Task<IActionResult> GenerateMockDataMon(int numberOfEntries)
        {
            var mockData = MockDataGeneratorStore.GenerateMockDataForMon(numberOfEntries);
            await DBService.InsertManyStoresAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Møn generated and inserted successfully.");
        }

        [HttpGet("findStoreByH3Index")]
        public async Task<ActionResult<List<Store>>> FindByH3Index(string h3Index, string brandName)
        {
            try
            {
                var matchingStorelocations = await DBService.FindStoreByH3IndexAsync(h3Index, brandName);
                if (matchingStorelocations != null && matchingStorelocations.Count > 0)
                {
                    return Ok(matchingStorelocations);
                }
                return NotFound("No stores found with the given H3 index.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving stores: " + ex.Message);
            }
        }

    }
}