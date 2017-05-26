using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Plus.v1;






namespace CloudTools
{
    public class Drive  //CloudDocInterface
    {
        static string ClientId = "919235768362-6c75ihpgnu3r86c2ihuv99j65odg1ll5.apps.googleusercontent.com";
        static string ClientSecret = "iq35Y8PmRBhnGGD4HH81cxtk";
        static String APP_USER_AGENT = "Drive API Quickstart";
        static String[] SCOPES = new[] { DriveService.Scope.Drive };
        static UserCredential crede;

        public Drive() { }

        /// <summary>
        /// The user authorizes the app and his credentials are saved on a .json file
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserCredential GetGoogleOAuthCredential(string user, string jsonpath)
        {

            String JsonFilelocation = jsonpath;
            Google.Apis.Auth.OAuth2.UserCredential credential = null;
            using (var stream = new FileStream(JsonFilelocation, FileMode.Open,
                            FileAccess.Read))
            {
                Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker.Folder = "Tasks.Auth.Store";
                credential = Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker.AuthorizeAsync(
                Google.Apis.Auth.OAuth2.GoogleClientSecrets.Load(stream).Secrets,
                new[] { DriveService.Scope.Drive, PlusService.Scope.UserinfoProfile, PlusService.Scope.UserinfoEmail },
                user,
                System.Threading.CancellationToken.None,
                new FileDataStore("Drive.Auth.Store")).Result;
            }
            crede = credential;
            return credential;

        }
        public static DriveService getService(UserCredential credential)
        {
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });
            return service;
        }
        public static PlusService getPlusService(UserCredential credential)
        {
            PlusService service = new PlusService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Plus Sample",
            });
            return service;
        }
        public static bool IsBase64Encoded(String str)
        {
            try
            {
                // If no exception is caught, then it is possibly a base64 encoded string
                byte[] data = Convert.FromBase64String(str);
                // The part that checks if the string was properly padded to the
                // correct length was borrowed from d@anish's solution
                return (str.Replace(" ", "").Length % 4 == 0);
            }
            catch
            {
                // If exception is caught, then it is not a base64 encoded string
                return false;
            }
        }
        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="nameFile">name to be saved with</param>
        /// <param name="filePath">local file path</param>
        /// <param name="fileIDparent">parent id, empty for root</param>
        public static Boolean uploadFile(DriveService service, string filePath, string fileIDparent, string nameFile, string mimetype)
        {
            try
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = nameFile;
                body.Description = "flexygo";
                if (mimetype != null && mimetype != string.Empty) { body.MimeType = mimetype; }
                else { body.MimeType = GetMimeType(nameFile); }
                List<string> parents = new List<string>();
                if (fileIDparent != null && fileIDparent != string.Empty) { parents.Add(fileIDparent); }
                body.Parents = parents;
                byte[] byteArray = Convert.FromBase64String(filePath);
                if (!IsBase64Encoded(filePath))
                {
                    byteArray = System.IO.File.ReadAllBytes(@filePath);
                }
                
                System.IO.MemoryStream stream2 = new System.IO.MemoryStream(byteArray);

                FilesResource.CreateMediaUpload request = service.Files.Create(body, stream2, GetMimeType(nameFile));
                request.Upload();
                return true;
            }
            catch (Exception e) { Console.Write(e.ToString()); return false; }

            //Google.Apis.Drive.v3.Data.File file = request.ResponseBody;
        }
        /// <summary>
        /// Updates a file with news params
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="newTitle"></param>
        /// <param name="newDescription"></param>
        /// <param name="newMimeType"></param>
        /// <param name="newFilename"></param>
        /// <param name="newRevision"></param>
        /// <returns></returns>
        private static Boolean updateFile(DriveService service, String fileId, String newTitle, String newDescription, String newMimeType, String newFilename, bool newRevision)
        {
            try
            {
                // First retrieve the file from the API.
                Google.Apis.Drive.v3.Data.File file = service.Files.Get(fileId).Execute();

                // File's new metadata.
                file.Name = newTitle;
                file.Description = newDescription;
                file.MimeType = newMimeType;

                // File's new content.
                byte[] byteArray = System.IO.File.ReadAllBytes(newFilename);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                // Send the request to the API.
                FilesResource.UpdateMediaUpload request = service.Files.Update(file, fileId, stream, newMimeType);
                request.Upload();

                //Google.Apis.Drive.v3.Data.File updatedFile = request.ResponseBody;
                //return updatedFile;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return false;
            }
        }
        /// <summary>
        /// Creates a folder 
        /// </summary>
        /// <param name="_title">name to create it</param>
        /// <param name="_description">description</param>
        /// <param name="_parent">parent id, empty for root</param>
        /// <returns></returns>
        public static Google.Apis.Drive.v3.Data.File createDirectory(DriveService service, string _title, string _parentID)
        {

            Google.Apis.Drive.v3.Data.File NewDirectory = null;

            // Create metaData for a new Directory
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
            body.Name = _title;
            body.Description = "";
            body.MimeType = "application/vnd.google-apps.folder";
            List<string> parents = new List<string>();
            if (!_parentID.Equals(string.Empty)) { parents.Add(_parentID); }
            body.Parents = parents;
            try
            {
                FilesResource.CreateRequest request = service.Files.Create(body);
                NewDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return NewDirectory;
        }
        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        /// <summary>
        /// Aux method to getfiles methods
        /// </summary>
        /// <returns></returns>
        private static FilesResource.ListRequest getFilesRE(DriveService service)
        {
            return service.Files.List();
        }
        /// <summary>
        /// Deletes ALL files on trash
        /// </summary>
        public static Boolean emptyTrash(DriveService service)
        {
            try
            {
                FilesResource.EmptyTrashRequest limpia = new FilesResource.EmptyTrashRequest(service);
                limpia.Execute();
                return true;
            }
            catch (Exception e) { Console.Write(e.ToString()); return false; }
        }
        /// <summary>
        /// Downloads a file in your pc
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="pathToSave"></param>
        public static Boolean downloadBinaryFile(DriveService service, string fileID, string pathToSave)
        {
            try
            {
                var request = service.Files.Get(fileID);
                var file = request.Execute();
                var stream = new System.IO.MemoryStream();
                FileStream filesave = new FileStream(@pathToSave + file.Name + file.FileExtension, FileMode.Create, FileAccess.Write);
                request.Download(stream);
                stream.WriteTo(filesave);
                filesave.Close();
                stream.Close();
                return true;
            }
            catch (Exception e) { Console.Write(e.ToString()); return false; }
        }
        /// <summary>
        /// Get the Uri of a file's icon
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public static string getUriFileIcon(DriveService service, string fileID)
        {
            var file = service.Files.Get(fileID).Execute();
            return file.IconLink;
        }
        /// <summary>
        /// Returns a list with ALL of your files on drive, trash included
        /// </summary>
        /// <returns></returns>
        public static List<CloudObject> getAllFiles(DriveService service)
        {
            var list = getFilesRE(service).Execute().Files;
            List<CloudObject> result = new List<CloudObject>();
            foreach (var item in list)
            {
                result.Add(new CloudObject(item));
            }
            return result;
        }
        /// <summary>
        /// Get a list of files and their ids that has a specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<CloudObject> getIDbyname(DriveService service, string name)
        {
            var files = getFilesRE(service);
            files.Q = "name='" + name + "'";
            var list = files.Execute().Files;
            List<CloudObject> result = new List<CloudObject>();
            foreach (var item in list)
            {
                result.Add(new CloudObject(item));
            }
            return result;
        }
        /// <summary>
        /// Get ALL files and folders on your drive except the trashed
        /// </summary>
        /// <returns></returns>
        public static List<CloudObject> getNormalFiles(DriveService service)
        {
            return getFilesFiltered(service, "trashed = false");
        }
        /// <summary>
        /// Get all the trashed files
        /// </summary>
        /// <returns></returns>
        public static List<CloudObject> getTrashedFiles(DriveService service)
        {
            return getFilesFiltered(service, "trashed = true");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentID">see https://developers.google.com/drive/v3/web/search-parameters to get info about filters</param>
        /// <returns></returns>
        public static List<CloudObject> getFilesFiltered(DriveService service, string filter)
        {
            var files = getFilesRE(service);
            files.Q = filter;
            var list = files.Execute().Files;
            List<CloudObject> result = new List<CloudObject>();
            foreach (var item in list)
            {
                result.Add(new CloudObject(item));
            }
            return result;
        }
        /// <summary>
        /// Get ALL files and folders that has that parent,on all levels
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public static List<CloudObject> getFilesFromParent(DriveService service, string parentID)
        {
            return getFilesFiltered(service, "'" + parentID + "' in parents");

        }
        /// <summary>
        /// Returns the first level of files and folders under the parent
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private static List<CloudObject> getFilesFromParentFirstFloor(DriveService service, string parentID)//shit method bru, not using it as it works the same way that getFilesFromParent but i keep it here for the lulz
        {
            var lista = getFilesFromParent(service, parentID);
            var parent = service.Files.Get(parentID).Execute();
            List<CloudObject> resultado = new List<CloudObject>();
            foreach (var item in lista)
            {
                if (item.Parents.Count == ((parent.Parents.Count) + 1)) { resultado.Add(item); }
            }

            return resultado;
        }
        /// <summary>
        /// Permantly deletes a file
        /// </summary>
        /// <param name="fileID"></param>
        public static Boolean permantylyDeleteFile(DriveService service, string fileID)
        {
            try
            {
                var request = service.Files.Delete(fileID).Execute();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Permantly deletes a folder
        /// </summary>
        /// <param name="folderID"></param>
        public static Boolean permantlyDeleteFolder(DriveService service, string folderID)
        {
            try { var request = service.Files.Delete(folderID).Execute(); return true; }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Moves a file by updating his parent list
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="newParent"></param>
        public static Boolean moveFile(DriveService service, string fileID, string newParent)
        { try
            {


                // First retrieve the file from the API.
                Google.Apis.Drive.v3.Data.File body = service.Files.Get(fileID).Execute();
                //Get Parents from the new parent
                Google.Apis.Drive.v3.Data.File fileParent = service.Files.Get(newParent).Execute();
                //update parents from the file to newparent+newparent.parents
                var parents = fileParent.Parents;
                parents.Add(newParent);
                body.Parents = parents;
                //update file
                service.Files.Update(body, fileID);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

        }
        public static string getToken(DriveService service)
        {
            return service.Revisions.List("").OauthToken;

        }
        public static Person getPlusUser(PlusService serviceplus)
        {
            PeopleResource.GetRequest personRequest = serviceplus.People.Get("me");
            Person user = personRequest.Execute();
            return user;
        }
        public static string getName(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            return user.DisplayName;
        }
        public static string getEmail(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            String emails = "";
            foreach (var u in user.Emails) {
                emails += u.Value + " ";
            }
            return emails;
        }
    
        public static string getAccountId(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            return user.Id;
        }
        public static string getPhotoUrl(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            return user.Image.Url;
        }
        public static bool? verfiedAccount(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            return user.Verified.Value;
        }
        public static string getCurrentLocation(PlusService serviceplus)
        {
            Person user = getPlusUser(serviceplus);
            return user.CurrentLocation;
        }
        public static long? usedSpace(DriveService service)
        {
            try
            {
                About about = service.About.Get().Execute();

                return about.StorageQuota.Usage;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static long? totalSpace(DriveService service)
        {
            try
            {
                About about = service.About.Get().Execute();

                return about.StorageQuota.Limit;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static long? SpaceIntrash(DriveService service)
        {
            try
            {
                About about = service.About.Get().Execute();

                return about.StorageQuota.UsageInDriveTrash;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static long? SpaceUsedWithoutTrash(DriveService service)
        {
            try
            {
                About about = service.About.Get().Execute();

                return about.StorageQuota.UsageInDrive;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }
}
