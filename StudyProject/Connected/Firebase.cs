using Google.Cloud.Firestore;
using StudyProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyProject
{
    public class Firebase
    {
        private static FirestoreDb db;
        public Firebase()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"test-66d74-firebase-adminsdk-jz0nu-f863f22421.json";//json файл с api коннектом с базой firebase, вставляем сюда свой
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);//оставляем без изменений
            db = FirestoreDb.Create("test-66d74");//наименование базы firebase
        }
        public static void UploadPictires(Model.Pictures pictures)//загрузка картинки в firebase
        {
            DocumentReference docRef = db.Collection("Files").Document($"File_{pictures.IdGood}");
            //Файл больее 650 кб не помещается в одну строку в Firebase, по этому если он больше то его необходимо разделить
            if (pictures.FileBase64.Length < 610000)
            {
                Dictionary<string, object> files = new Dictionary<string, object>();
                files.Add("file_name", pictures.FileName);
                files.Add("good_id", pictures.IdGood);
                files.Add("size", 1);
                docRef.SetAsync(files);
                Dictionary<string, object> files1 = new Dictionary<string, object>();
                DocumentReference docRef1 = docRef.Collection("file_part").Document("1");
                files1.Add("1", pictures.FileBase64);
                docRef1.SetAsync(files1);
            }
            else
            {
                var files_split = Split(pictures.FileBase64, 600000);
                Dictionary<string, object> files = new Dictionary<string, object>();
                files.Add("file_name", pictures.FileName);
                files.Add("good_id", pictures.IdGood);
                files.Add("size", files_split.Count());
                docRef.SetAsync(files);
                int i = 1;
                foreach (var str in files_split)
                {
                    Dictionary<string, object> files1 = new Dictionary<string, object>();
                    DocumentReference docRef1 = docRef.Collection("file_part").Document(i.ToString());
                    files1.Add(i.ToString(), str);
                    docRef1.SetAsync(files1);
                    i++;
                }
                
            }
        }
        public static void UploadQr(int _id, string file)//загрузка qr в firebase
        {
            DocumentReference docRef = db.Collection("QRFiles").Document($"File_{_id}");
            //Файл больее 650 кб не помещается в одну строку в Firebase, по этому если он больше то его необходимо разделить
            if (file.Length < 610000)
            {
                Dictionary<string, object> files = new Dictionary<string, object>();
                files.Add("file_name", "qr_"+ _id);
                files.Add("basket_id", _id);
                files.Add("size", 1);
                docRef.SetAsync(files);
                Dictionary<string, object> files1 = new Dictionary<string, object>();
                DocumentReference docRef1 = docRef.Collection("file_part").Document("1");
                files1.Add("1", file);
                docRef1.SetAsync(files1);
            }
            else
            {
                var files_split = Split(file, 600000);
                Dictionary<string, object> files = new Dictionary<string, object>();
                files.Add("file_name", "qr_" + _id);
                files.Add("basket_id", _id);
                files.Add("size", files_split.Count());
                docRef.SetAsync(files);
                int i = 1;
                foreach (var str in files_split)
                {
                    Dictionary<string, object> files1 = new Dictionary<string, object>();
                    DocumentReference docRef1 = docRef.Collection("file_part").Document(i.ToString());
                    files1.Add(i.ToString(), str);
                    docRef1.SetAsync(files1);
                    i++;
                }

            }
        }
        private static IEnumerable<string> Split(string s, int length)//разделение строки
        {
            int i;
            for (i = 0; i + length < s.Length; i += length)
                yield return s.Substring(i, length);
            if (i != s.Length)
                yield return s.Substring(i, s.Length - i);
        }
        private Pictures pic;
        private async Task DownPicAsync(int IdGood)//асинхронный метот выгрузкы картинки из firebase
        {
            await Task.Run(async () =>
           {
               pic = null;
               DocumentReference docRef = db.Collection("Files").Document($"File_{IdGood}");
               DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
               int count = 0;
               string pic_string = "";
               string file_name = "";

               if (snapshot.Exists)
               {
                   Console.WriteLine("Document data for {0} document:", snapshot.Id);
                   Dictionary<string, object> file = snapshot.ToDictionary();
                   count = Convert.ToInt32(file.Where(p => p.Key == "size").Select(p => p.Value).FirstOrDefault());
                   file_name = file.Where(p => p.Key == "file_name").Select(p => p.Value).FirstOrDefault().ToString();

               }
               IAsyncEnumerable<CollectionReference> subcollections = docRef.ListCollectionsAsync();
               IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = subcollections.GetAsyncEnumerator(default);
               for (int i = 1; i <= count; i++)
               {
                   DocumentReference docRef1 = db.Collection("Files").Document($"File_{IdGood}").Collection("file_part").Document(i.ToString());
                   DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                   if (snapshot1.Exists)
                   {
                       Dictionary<string, object> file = snapshot1.ToDictionary();
                       pic_string += file.Where(p => p.Key == i.ToString()).Select(p => p.Value).FirstOrDefault().ToString();
                   }
               }
               pic = new Model.Pictures {FileName= file_name, IdGood= IdGood, FileBase64= pic_string };
           });
        }
        public Pictures DownloadPictires(int IdGood) //сохранение картинки
        {
            _ = DownPicAsync(IdGood);
            while (pic == null)
            {

            }
            Pictures picture = new Pictures(pic.FileName, pic.IdGood, pic.FileBase64);
            return picture;
        }
    }
}