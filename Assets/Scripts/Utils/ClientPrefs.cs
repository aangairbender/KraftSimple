using UnityEngine;

public static class ClientPrefs
{
    const string k_ClientGuidKey = "client_guid";

    public static string GetGuid()
    {
        if (PlayerPrefs.HasKey(k_ClientGuidKey))
        {
            return PlayerPrefs.GetString(k_ClientGuidKey);
        }

        var guid = System.Guid.NewGuid();
        var guidString  = guid.ToString();

        PlayerPrefs.SetString(k_ClientGuidKey, guidString);
        return guidString;
    }
}
