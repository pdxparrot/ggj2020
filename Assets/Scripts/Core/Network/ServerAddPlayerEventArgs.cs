#pragma warning disable 0618    // disable obsolete warning for now

using System;

#if USE_NETWORKING
using UnityEngine.Networking;
#endif

namespace pdxpartyparrot.Core.Network
{
    public sealed class ServerAddPlayerEventArgs : EventArgs
    {
        public NetworkConnection NetworkConnection { get; }

        public short PlayerControllerId { get; }

        public ServerAddPlayerEventArgs(NetworkConnection conn, short playerControllerId)
        {
            NetworkConnection = conn;
            PlayerControllerId = playerControllerId;
        }
    }
}
