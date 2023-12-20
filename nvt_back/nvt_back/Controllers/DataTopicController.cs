using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Mqtt;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataTopicController : Controller
    {
        private readonly IMqttClientService _mqttClientService;

        public DataTopicController(IMqttClientService mqttService)
        {
            _mqttClientService = mqttService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MessageDTO>> SubscribeToDataTopic(int id)
        {
            Console.WriteLine("tuuuuuuuuuuuuuuuuu");
            try
            {
                await _mqttClientService.SubscribeToDataTopic(id);
                return Ok(new MessageDTO("Successfully subscribed to data topic."));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR!");
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
