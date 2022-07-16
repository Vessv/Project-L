using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    public string joinCodeString;

    [SerializeField]
    private string _enviroment = "production";

    [SerializeField]
    private int _maxConnections = 10;

    string _relayJoinCode;

    private void Start()
    {
        Instance = this;
    }

    public bool isRelayEnabled => Transport != null && 
        Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay()
    {
        InitializationOptions options = new InitializationOptions().SetEnvironmentName(_enviroment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);

        RelayHostData relayHostData = new RelayHostData
        {
            key = allocation.Key,
            port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            allocationIdBytes = allocation.AllocationIdBytes,
            ipv4address = allocation.RelayServer.IpV4,
            connectionData = allocation.ConnectionData,

        };

        relayHostData.joinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

        Debug.Log("join code:" + relayHostData.joinCode);
        joinCodeString = relayHostData.joinCode;

        Transport.SetRelayServerData(relayHostData.ipv4address, relayHostData.port, relayHostData.allocationIdBytes, relayHostData.key, relayHostData.connectionData);

        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        InitializationOptions options = new InitializationOptions().SetEnvironmentName(_enviroment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData relayJoinData = new RelayJoinData
        {
            key = allocation.Key,
            port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            allocationIdBytes = allocation.AllocationIdBytes,
            ipv4address = allocation.RelayServer.IpV4,
            hostConnectionData = allocation.HostConnectionData,
            connectionData = allocation.ConnectionData,
            joinCode = joinCode

        };

        Transport.SetRelayServerData(relayJoinData.ipv4address, relayJoinData.port, relayJoinData.allocationIdBytes, relayJoinData.key, relayJoinData.connectionData, relayJoinData.hostConnectionData);
        
        //NetworkLog.LogInfoServer("joined code:" + relayJoinData.joinCode);

        return relayJoinData;
    }

    public struct RelayHostData
    {
        public string joinCode;
        public string ipv4address;
        public ushort port;
        public Guid AllocationID;
        public byte[] allocationIdBytes;
        public byte[] connectionData;
        public byte[] key;

    }
    public struct RelayJoinData
    {
        public string joinCode;
        public string ipv4address;
        public ushort port;
        public Guid AllocationID;
        public byte[] allocationIdBytes;
        public byte[] hostConnectionData;
        public byte[] connectionData;
        public byte[] key;

    }

}
