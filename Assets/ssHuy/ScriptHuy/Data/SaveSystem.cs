using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SaveFolder = Application.persistentDataPath + "/Saves/";

    // Đảm bảo thư mục tồn tại khi game chạy
    static SaveSystem()
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);
    }

    /// <summary>
    /// Lưu dữ liệu người chơi vào file .json
    /// </summary>
    public static void SavePlayer(PlayerData data, string saveName = null)
    {
        try
        {
            if (string.IsNullOrEmpty(saveName))
                saveName = data.saveName;

            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogWarning("⚠️ Không có tên file để lưu!");
                return;
            }

            string path = SaveFolder + saveName + ".json";
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            Debug.Log($"✅ Lưu thành công: {saveName} tại {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi lưu dữ liệu: {e.Message}");
        }
    }

    /// <summary>
    /// Load dữ liệu người chơi từ file
    /// </summary>
    public static PlayerData LoadPlayer(string saveName)
    {
        try
        {
            string path = SaveFolder + saveName + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                return data;
            }
            else
            {
                Debug.LogWarning($"⚠️ Không tìm thấy file {saveName}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi load dữ liệu {saveName}: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Lấy toàn bộ danh sách file save hiện có
    /// </summary>
    public static List<string> GetAllSaveFiles()
    {
        List<string> saveFiles = new List<string>();

        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string[] files = Directory.GetFiles(SaveFolder, "*.json");
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            saveFiles.Add(fileName);
        }

        return saveFiles;
    }

    /// <summary>
    /// Xóa file lưu cụ thể
    /// </summary>
    public static void DeleteSave(string saveName)
    {
        try
        {
            string path = SaveFolder + saveName + ".json";
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"🗑️ Đã xóa file lưu: {saveName}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Không tìm thấy file để xóa: {saveName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi xóa file {saveName}: {e.Message}");
        }
    }
}
