namespace API_Simulacao.Util
{
    using API_Simulacao.DTOs;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using System.Text;
    using System.Text.Json;

    public class EventHubSDK
    {
        static EventHubProducerClient _producerClient;
        
        public EventHubSDK(IConfiguration configuration)
        {
            _producerClient = new EventHubProducerClient(configuration.GetConnectionString("EhSimulacoes"));
        }
        public async Task EnviaEvento(byte[] json)
        {
            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();

            if (!eventBatch.TryAdd(new EventData(json)))
            {
                throw new Exception($"Evento muito grande");
            }

            await _producerClient.SendAsync(eventBatch);
        }

        public async Task EnviaEventoProc(EventHubDTO evento, string cloudRoleName)
        {
            evento.Aplicacao = cloudRoleName;
            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));

            using var eventBatch = await _producerClient.CreateBatchAsync();
            if (!eventBatch.TryAdd(new EventData(message)))
            {
                throw new ArgumentException($"Evento muito grande");
            }

            await _producerClient.SendAsync(eventBatch);
        }
    }
}
