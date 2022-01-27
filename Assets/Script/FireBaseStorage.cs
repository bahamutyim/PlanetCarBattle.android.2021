using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;
using Firebase.Unity;

namespace com.ahyim.planet
{
    public class FireBaseStorage : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Firebase.Storage.StorageReference storage_root_ref = Firebase.Storage.FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://planetcar-dd4e6.appspot.com");
            Firebase.Storage.StorageReference text_ref = storage_root_ref.Child("test.txt");
            text_ref.PutBytesAsync(Encoding.ASCII.GetBytes("Test storage")).ContinueWith((Task<StorageMetadata> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    Firebase.Storage.StorageMetadata metadata = task.Result;
                    string download_url = metadata.DownloadUrl.ToString();
                    Debug.Log("Finished uploading...");
                    Debug.Log("download url = " + download_url);
                }
            });
        }

        public static void WriteUserFile(string path, byte[] data)
        {
            string uid = PlayerAuthInfo.UserUID;
            Debug.LogFormat("User {0} write file", uid);
            if (uid != null && uid.Trim().Length > 0)
            {
                Firebase.Storage.StorageReference storage_user_path_ref = Firebase.Storage.FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://planetcar-dd4e6.appspot.com" + "/" +  uid + "/" + path);
                storage_user_path_ref.PutBytesAsync(data).ContinueWith((Task<StorageMetadata> task) => {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        // Uh-oh, an error occurred!
                    } 
                    else 
                    {
                        // Metadata contains file metadata such as size, content-type, and download URL.
                        Firebase.Storage.StorageMetadata metadata = task.Result;
                        string download_url = metadata.DownloadUrl.ToString();
                        Debug.Log("Finished uploading...");
                        Debug.Log("download url = " + download_url);
                    }
                });
            }
        }

        // Update is called once per frame
    }

}