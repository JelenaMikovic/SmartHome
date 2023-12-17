using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace nvt_back.InfluxDB
{
    public class InfluxDBService
    {
        private readonly string _token;
        private readonly string _org;
        private readonly string _bucket;

        public InfluxDBService(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("InfluxDB:Token");
            _org = configuration.GetValue<string>("InfluxDB:Org");
            _bucket = configuration.GetValue<string>("InfluxDB:Bucket");
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            using var client = new InfluxDBClient("http://localhost:8086", _token);
            var query = client.GetQueryApi();
            return await action(query);
        }

        public Task WriteHeartbeatToInfluxDBForDevice(int deviceId, int status)
        {
            var client = InfluxDBClientFactory.Create("http://localhost:8086", _token.ToCharArray());

            string statusToString = status.ToString();

            string data = "online,device_id=id" + deviceId.ToString() + " status=" + statusToString;

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteRecord(data, WritePrecision.Ns, _bucket, _org);
                Console.WriteLine("Writing to InfluxDB, device id: " + deviceId.ToString());
            }

            return Task.CompletedTask;
        }

        public Task WriteStateToInfluxDBForDevice(int deviceId, string state, int userId)
        {
            var client = InfluxDBClientFactory.Create("http://localhost:8086", _token.ToCharArray());

            string data = "state,device_id=id" + deviceId.ToString() + " status=" + state + ",userId=" + userId.ToString();

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteRecord(data, WritePrecision.Ns, _bucket, _org);
                Console.WriteLine("Writing to InfluxDB, device id: " + deviceId.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
