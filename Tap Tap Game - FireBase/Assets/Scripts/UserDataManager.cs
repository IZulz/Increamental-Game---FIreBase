using Firebase.Storage;
using System.Threading.Tasks;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void LoadFromLocal()
    {
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Progress = new UserProgressData();
            Save();

            //used for step 2
            //Save(true);
        }
        else
        {
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }

    public static void Load()
    {
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }

    }

    //used for step 2

    //public static IEnumerator LoadFromCloud(System.Action onComplete)
    //{
    //    Debug.Log("is starting");
    //    StorageReference targetStorage = GetTargetCloudStorage();

    //    bool isCompleted = false;
    //    bool isSuccessfull = false;
    //    const long maxAllowedSize = 1024 * 1024; // 1 MB
    //    targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
    //    {
    //        if (!task.IsFaulted)
    //        {
    //            Debug.Log("success");

    //            string json = Encoding.Default.GetString(task.Result);
    //            Progress = JsonUtility.FromJson<UserProgressData>(json);
    //            isSuccessfull = true;
    //        }

    //        isCompleted = true;

    //        Debug.Log("iscolplit true");
    //    });

    //    while (!isCompleted)
    //    {
    //        yield return null;
    //    }

    //    if (isSuccessfull)
    //    {
    //        Debug.Log("cloud data");

    //        Save();
    //    }
    //    else
    //    {
    //        Debug.Log("local data");

    //        LoadFromLocal();
    //    }

    //    onComplete?.Invoke();
    //}


    //public static void Save(bool uploadToClaud = false)
    //{
    //    string json = JsonUtility.ToJson(Progress);
    //    PlayerPrefs.SetString(PROGRESS_KEY, json);

    //    if (uploadToClaud)
    //    {
    //        AnalyticsManager.SetUserProperties("gold", Progress.Gold.ToString());

    //        byte[] data = Encoding.Default.GetBytes(json);
    //        StorageReference targetStorage = GetTargetCloudStorage();
    //        targetStorage.PutBytesAsync(data);
    //    }
    //}

    //private static StorageReference GetTargetCloudStorage()
    //{
    //    string deviceID = SystemInfo.deviceUniqueIdentifier;
    //    FirebaseStorage storage = FirebaseStorage.DefaultInstance;

    //    return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
    //}
}
