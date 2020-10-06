namespace CloudSpeed.Powergate
{
    public class PowergateClient
    {
        private readonly PowergateSetting _powergateSetting;

        public PowergateClient(PowergateSetting powergateSetting,
            Index.Ask.Rpc.RPCService.RPCServiceClient indexAsk,
            Index.Faults.Rpc.RPCService.RPCServiceClient indexFaults,
            Index.Miner.Rpc.RPCService.RPCServiceClient indexMiner,
            Ffs.Rpc.RPCService.RPCServiceClient ffs,
            Net.Rpc.RPCService.RPCServiceClient net,
            Wallet.Rpc.RPCService.RPCServiceClient wallet)
        {
            _powergateSetting = powergateSetting;
            IndexAsk = indexAsk;
            IndexFaults = indexFaults;
            IndexMiner = indexMiner;
            Ffs = ffs;
            Net = net;
            Wallet = wallet;
        }

        public Index.Ask.Rpc.RPCService.RPCServiceClient IndexAsk { get; }

        public Index.Faults.Rpc.RPCService.RPCServiceClient IndexFaults { get; }

        public Index.Miner.Rpc.RPCService.RPCServiceClient IndexMiner { get; }

        public Ffs.Rpc.RPCService.RPCServiceClient Ffs { get; }

        public Net.Rpc.RPCService.RPCServiceClient Net { get; }

        public Wallet.Rpc.RPCService.RPCServiceClient Wallet { get; }

        public Grpc.Core.Metadata BotXFfsToken
        {
            get
            {
                var headers = new Grpc.Core.Metadata();
                headers.Add("X-ffs-Token", _powergateSetting.BotToken);
                return headers;
            }
        }
    }
}
