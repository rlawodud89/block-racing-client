using System.IO;
using UnityEngine;

[System.Serializable]
public class ServerConfig
{
    public string serverIp;
    public int serverPort;

    public static ServerConfig Load()
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "server_config.json");

        if (!File.Exists(path))
        {
            ClientLogger.Error(
                $"Server config file not found: {path}");

            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ServerConfig>(json);
    }
}