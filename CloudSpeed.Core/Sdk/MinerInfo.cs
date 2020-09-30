using Newtonsoft.Json;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class MinerInfo
    {
        public string Owner { get; set; }

        public string Worker { get; set; }
        
        public string NewWorker { get; set; }
        
        public string[] ControlAddresses { get; set; }
        
        public long WorkerChangeEpoch { get; set; }
        
        public string PeerId { get; set; }
        
        public string[] Multiaddrs { get; set; }
        
        public long RegisteredSealProof { get; set; }
        
        public ulong SectorSize { get; set; }

        public ulong WindowPoStPartitionSectors { get; set; }
    }
}
