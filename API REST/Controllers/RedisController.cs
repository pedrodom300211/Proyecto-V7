using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API_REST.Controllers
{   //Cambia nombre
    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisController(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        // Start (POST): Crear un nuevo script
        [HttpPost("start/{scriptType}/{scriptId}")]
        public IActionResult Start(string scriptType, string scriptId)
        {
            if (string.IsNullOrEmpty(scriptType) || string.IsNullOrEmpty(scriptId))
            {
                return BadRequest(new { message = "scriptType and scriptId are required." });
            }

            // Crear el objeto ScriptData
            var data = new ScriptData
            {
                ProcessId = Guid.NewGuid().ToString(),
                ScriptType = scriptType,
                ScriptId = scriptId,
                CurrentSection = 1,
                TotalSections = 4
            };

            string key = $"script:{data.ProcessId}";

            // Guardar en Redis
            var db = _redisConnection.GetDatabase();
            string jsonData = JsonConvert.SerializeObject(data);
            db.StringSet(key, jsonData);

            return Ok(new { message = "Script created successfully", key, data });
        }

        // GET: Obtener información de un script
        [HttpGet("get-json/{processId}")]
        public IActionResult GetJson(string processId)
        {
            string key = $"script:{processId}";
            var db = _redisConnection.GetDatabase();
            var jsonData = db.StringGet(key);

            if (jsonData.IsNullOrEmpty)
            {
                return NotFound(new { message = "Process ID not found in Redis.", processId });
            }

            var data = JsonConvert.DeserializeObject<ScriptData>(jsonData);
            return Ok(data);
        }

        // Next (POST): Incrementar currentSection
        [HttpPost("next/{processId}")]
        public IActionResult Next(string processId)
        {
            string key = $"script:{processId}";
            var db = _redisConnection.GetDatabase();
            var jsonData = db.StringGet(key);

            if (jsonData.IsNullOrEmpty)
            {
                return NotFound(new { message = "Process ID not found.", processId });
            }

            var data = JsonConvert.DeserializeObject<ScriptData>(jsonData);

            // Incrementar currentSection
            if (data.CurrentSection < data.TotalSections)
            {
                data.CurrentSection++;
                jsonData = JsonConvert.SerializeObject(data);
                db.StringSet(key, jsonData);

                return Ok(new { message = "Current section incremented", data });
            }
            else
            {
                return BadRequest(new { message = "Current section already at the maximum.", processId });
            }
        }
    }
}