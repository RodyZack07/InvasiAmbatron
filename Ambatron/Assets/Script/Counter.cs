using Firebase.Firestore;


[FirestoreData]
public struct Counter
{
    [FirestoreProperty]
    public int count { get; set; }

    [FirestoreProperty]
    public string UpdateBy { get; set; }

    [FirestoreProperty]
    public Timestamp lastUpdated { get; set; }
}
