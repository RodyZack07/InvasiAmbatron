using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class FirestoreManager : MonoBehaviour
{
    [SerializeField] Button updateBtn;
    [SerializeField] Text countUi;

    FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        updateBtn.onClick.AddListener(OnHandleClick);
        FetchAndUpdateUI(); // Memuat nilai awal dari Firestore
    }

    void FetchAndUpdateUI()
    {
        // Mendapatkan nilai terbaru dari Firestore
        DocumentReference countRef = db.Collection("counters").Document("counter");
        countRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                Counter counter = task.Result.ConvertTo<Counter>();
                countUi.text = counter.count.ToString(); // Update UI dengan nilai terbaru
            }
            else
            {
                Debug.LogWarning("Document tidak ditemukan!");
            }
        });
    }

    void OnHandleClick()
    {
        // Mendapatkan nilai terbaru dari Firestore sebelum update
        DocumentReference countRef = db.Collection("counters").Document("counter");
        countRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                Counter counter = task.Result.ConvertTo<Counter>();
                int newCount = counter.count + 1;

                // Update nilai baru ke Firestore
                Counter updatedCounter = new Counter
                {
                    count = newCount,
                    UpdateBy = "Ambatron",
                    lastUpdated = Timestamp.GetCurrentTimestamp() // Tambahkan timestamp saat ini
                };

                countRef.SetAsync(updatedCounter).ContinueWithOnMainThread(setTask =>
                {
                    if (setTask.IsCompleted)
                    {
                        Debug.Log("Count updated successfully");
                        countUi.text = newCount.ToString(); // Update UI dengan nilai baru
                    }
                    else
                    {
                        Debug.LogError("Gagal memperbarui count: " + setTask.Exception);
                    }
                });
            }
            else
            {
                Debug.LogError("Gagal membaca document: " + task.Exception);
            }
        });
    }

}
