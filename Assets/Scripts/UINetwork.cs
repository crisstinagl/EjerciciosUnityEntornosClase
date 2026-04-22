using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

    /// <summary>
    /// Add this component to the same GameObject as
    /// the NetworkManager component.
    /// </summary>
    public class HelloWorldManager : MonoBehaviour
    {
    private NetworkManager m_NetworkManager;
    private string joinCode = "Enter join code...";

    private void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) StartHost();
        if (GUILayout.Button("Client")) StartClient();
        //if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
        joinCode = GUILayout.TextArea(joinCode);
    }

    async void StartClient()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
        m_NetworkManager.StartClient();
    }

    private async void StartHost()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var allocation = await RelayService.Instance.CreateAllocationAsync(50);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        m_NetworkManager.StartHost();
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Roomcode: " + joinCode);
    }

}

    



