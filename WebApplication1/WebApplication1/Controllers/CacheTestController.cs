using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
#if DEBUG
    [ApiController]
    [Route("api/[controller]")]
    public class CacheTestController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheTestController> _logger;
        private readonly IWebHostEnvironment _environment;

        public CacheTestController(ICacheService cacheService, ILogger<CacheTestController> logger, IWebHostEnvironment environment)
        {
            _cacheService = cacheService;
            _logger = logger;
            _environment = environment;
        }

        private bool IsDevelopmentOrDebug()
        {
            return _environment.IsDevelopment() || _environment.IsEnvironment("Debug");
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetCache([FromBody] CacheTestRequest request)
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                await _cacheService.SetAsync(request.Key, request.Value, TimeSpan.FromMinutes(request.ExpirationMinutes));
                _logger.LogInformation("Cache set successfully for key: {Key}", request.Key);
                return Ok(new { message = "Cache set successfully", key = request.Key });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", request.Key);
                return StatusCode(500, new { error = "Failed to set cache" });
            }
        }

        [HttpGet("get/{key}")]
        public async Task<IActionResult> GetCache(string key)
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var value = await _cacheService.GetAsync<string>(key);
                if (value == null)
                {
                    return NotFound(new { message = "Cache key not found", key });
                }
                
                _logger.LogInformation("Cache retrieved successfully for key: {Key}", key);
                return Ok(new { key, value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return StatusCode(500, new { error = "Failed to get cache" });
            }
        }

        [HttpDelete("remove/{key}")]
        public async Task<IActionResult> RemoveCache(string key)
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                await _cacheService.RemoveAsync(key);
                _logger.LogInformation("Cache removed successfully for key: {Key}", key);
                return Ok(new { message = "Cache removed successfully", key });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
                return StatusCode(500, new { error = "Failed to remove cache" });
            }
        }

        [HttpGet("exists/{key}")]
        public async Task<IActionResult> ExistsCache(string key)
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var exists = await _cacheService.ExistsAsync(key);
                _logger.LogInformation("Cache exists check for key: {Key}, result: {Exists}", key, exists);
                return Ok(new { key, exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return StatusCode(500, new { error = "Failed to check cache existence" });
            }
        }

        [HttpGet("info")]
        public IActionResult GetCacheInfo()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            var cacheType = _cacheService.GetType().Name;
            return Ok(new { 
                cacheProvider = cacheType,
                message = $"Currently using {cacheType}",
                timestamp = DateTime.UtcNow
            });
        }
    }

    public class CacheTestRequest
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 30;
    }
#endif
}
