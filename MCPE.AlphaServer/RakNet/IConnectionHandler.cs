using System;

namespace SpoongePE.Core.RakNet;

public interface IConnectionHandler {
    public void OnOpen(RakNetClient address);
    public void OnClose(RakNetClient address, string reason);
    public void OnData(RakNetClient address, ReadOnlyMemory<byte> data);
    public void OnUpdate();
}
