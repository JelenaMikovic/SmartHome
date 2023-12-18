using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace nvt_back.InfluxDB
{
    public class InfluxDBService
    {
        private readonly string _token;
        private readonly string _adminToken;
        private readonly string _org;
        private readonly string _orgId;
        private readonly string _bucket;

        public InfluxDBService(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("InfluxDB:Token");
            _adminToken = configuration.GetValue<string>("InfluxDB:AdminToken");
            _org = configuration.GetValue<string>("InfluxDB:Org");
            _orgId = configuration.GetValue<string>("InfluxDB:OrgId");
            _bucket = configuration.GetValue<string>("InfluxDB:Bucket");
        }


        public async Task<IEnumerable<FluxRecord>> QueryAsync(string query)
        {
            using var client = InfluxDBClientFactory.Create("http://localhost:8086", _adminToken.ToCharArray());
            Console.WriteLine(_adminToken);
            Console.WriteLine(_orgId);
            var fluxTables = await client.GetQueryApi().QueryAsync(query, _orgId);
            Console.WriteLine("tri");
            var records = new List<FluxRecord>();

            foreach (var fluxTable in fluxTables)
            {
                records.AddRange(fluxTable.Records);
            }

            return records;
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
