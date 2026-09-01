using UnityEngine;

public static class ClientLogger
{
    public static void Network(string message)
    {
        Debug.Log($"[Network] {message}");
    }

    public static void Game(string message)
    {
        Debug.Log($"[Game] {message}");
    }

    public static void Packet(string message)
    {
        Debug.Log($"[Packet] {message}");
    }

    public static void UI(string message)
    {
        Debug.Log($"[UI] {message}");
    }

    public static void Warning(string message)
    {
        Debug.LogWarning($"[Warning] {message}");
    }

    public static void Error(string message)
    {
        Debug.LogError($"[Error] {message}");
    }
}