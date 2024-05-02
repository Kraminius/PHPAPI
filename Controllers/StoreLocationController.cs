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
                return StatusCode(500, "An error occured finding the store");
            }
        }

    }
}