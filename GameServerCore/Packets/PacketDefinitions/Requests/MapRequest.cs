namespace GameServerCore.Packets.PacketDefinitions.Requests
{
    public class MapRequest : ICoreRequest
    {
        public int ClientID { get; }
        public uint NetTeamID { get; }

        public MapRequest(int clientID, uint netTeamID)
        {
            ClientID = clientID;
            NetTeamID = netTeamID;
        }
    }
}
