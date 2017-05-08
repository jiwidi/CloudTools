using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Nito.AsyncEx;
using System.Windows.Forms;

namespace CloudTools
{
     public class dropbox //: CloudDocInterface
    {
        //public DropboxClient dbx; //DropboxClient needed to work in almost every method, instanciated by secondAuthUser(token got from the url of firstAuthUser)
        static string ClientId = ""; //App key
        static string ClientSecret = ""; //App secret key




        public dropbox() { }
        /// <summary>
        /// Starts the procces to authorize the app on OAuth2, after this one secoindAuthUser must be called with the token recieved on the Uri
        /// </summary>
        /// <returns>Uri</returns>
        public static Uri firstAuthUser()
        {
            var authUri = Dropbox.Api.DropboxOAuth2Helper.GetAuthorizeUri(ClientId, false);
            return authUri;
        }
        /// <summary>
        /// Computes the full access token for a user given the first half got on firstAuthUser
        /// </summary>
        /// <param name="firstHalfToken"></param>
        /// <returns>boolean(succes,fail)</returns>
        public static DropboxClient getServerUser(string token)
        {
            var client = new DropboxClient(token);
            return client;
        }
        public static DropboxClient secondAuthUser(string firstHalfToken)
        {
            try
            {
                var dbx = new DropboxClient(secondAuthUserAux(firstHalfToken).Result);
                return dbx;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

        }
        /// <summary>
        /// Async auxiliar method to call async methods on dropbox
        /// </summary>
        /// <param name="firstHalfToken"></param>
        /// <returns>Task<string></returns>
        private static async Task<string> secondAuthUserAux(string firstHalfToken)
        {
            OAuth2Response authUriComplete = await Dropbox.Api.DropboxOAuth2Helper.ProcessCodeFlowAsync(firstHalfToken, ClientId, ClientSecret);
            return authUriComplete.AccessToken;
        }
        /// <summary>
        /// gets a Ilist<metadata>, folder must be in format /example/example, empty for root
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>Ilist<metadata></returns>
        public static List<CloudObject> retrieveFolder(DropboxClient dbx,string folder)
        {
            if (!folder.StartsWith("/") && !(folder == string.Empty)) { throw new System.InvalidOperationException("Path of the folder must start with / or null"); }
            //Task list = dbx.Files.ListFolderAsync(folder);
            List<Dropbox.Api.Files.Metadata> hijos = new List<Dropbox.Api.Files.Metadata>( getFolderAux(dbx,folder).Result.Entries);
            var list = hijos;
            List<CloudObject> result = new List<CloudObject>();
            foreach (var item in list)
            {
                result.Add(new CloudObject(item,folder));
            }
            return result;

        }
        public static List<CloudObject> retrieveAllFilesUnder(DropboxClient dbx, string folder)
        {
            List<CloudObject> list = retrieveFolder(dbx, folder);
            foreach(var item in list.Where(i => i.IsFolder))
            {
                retrieveAllfilesUnderaux(dbx, folder, ref list);
            }
            return list;
        }
        private static void retrieveAllfilesUnderaux(DropboxClient dbx, string folder,ref List<CloudObject> list)
        {
            List<CloudObject> lista = retrieveFolder(dbx, folder);
            list.AddRange(lista);//we don't need to call retrieveallfilesunderaux for the objects in lista as the main method will call them.
            
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>Task<IList></returns>
        private static  async Task<Dropbox.Api.Files.ListFolderResult> getFolderAux(DropboxClient dbx,string folderPath)
        {
            var list = await dbx.Files.ListFolderAsync(folderPath);
            return list;

        }
        /// <summary>
        /// Downloads a file from your dropbox into a specific folder on your pc,filename must have extension, folderDropbox must be like /example/example and folderPC be like C:\Users\Downloads
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderDropbox"></param>
        /// <param name="folderPC"></param>
        /// <returns>boolean(succes,fail)</returns>
        public static Boolean downloadFile(DropboxClient dbx, string fileName, string folderDropbox, string folderPC)
        {
            folderDropbox = folderDropbox.Replace(@"\", "/");
            folderPC = folderPC.Replace("/", @"\");
            if (!Path.HasExtension(fileName)) { throw new System.InvalidOperationException("Invalid filename, no file extension found"); }
            if (!folderDropbox.StartsWith("/")) { throw new System.InvalidOperationException("Invalid folderDB path"); }
            if (!Path.IsPathRooted(folderPC)) { throw new System.InvalidOperationException("Invalid path for FolderPC"); }

            return downloadFileAsyncAux(dbx,fileName, folderDropbox, folderPC).Result;
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox to download a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderDropbox"></param>
        /// <param name="folderPC"></param>
        /// <returns></returns>
        private static async Task<Boolean> downloadFileAsyncAux(DropboxClient dbx, string fileName, string folderDropbox, string folderPC)
        {
            try
            {
                using (var response = await dbx.Files.DownloadAsync(folderDropbox + "/" + fileName))
                {
                    File.WriteAllBytes(@folderPC + @"\" + fileName, await response.GetContentAsByteArrayAsync());
                }
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Uploads a file from your pc to your dropbox, filePath=complete path of the file foñderDB=target folder on dropbox nameFinal=name of the file be saved with 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="folderDB"></param>
        /// <param name="nameFinal"></param>
        /// <returns>boolean(succes,fail)</returns>
        public static Boolean uploadFile(DropboxClient dbx, string filePath, string folderDB, string nameFinal)
        {
            filePath = filePath.Replace("/", @"\");
            folderDB = folderDB.Replace(@"\", "/");
            if (!Path.IsPathRooted(filePath)) { throw new System.InvalidOperationException("Invalid path for filePath"); }
            if (!Path.HasExtension(filePath)) { throw new System.InvalidOperationException("Invalid path for filePath, no extension found"); }
            if (!folderDB.StartsWith("/")) {    throw new System.InvalidOperationException("Invalid folderDB path"); }
            if (!Path.HasExtension(nameFinal)) { throw new System.InvalidOperationException("Invalid nameFinal, no extension"); }
            return uploadFileAsyncAux(dbx,filePath, folderDB, nameFinal).Result;
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox to upload a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderDB"></param>
        /// <param name="nameFinal"></param>
        /// <returns>Task<Boolean></returns>
        private static async Task<Boolean> uploadFileAsyncAux(DropboxClient dbx, string fileName, string folderDB, string nameFinal)
        {
            try
            {
                byte[] arrayFile = File.ReadAllBytes(fileName);

                using (var mem = new MemoryStream(arrayFile))
                {

                    var updated = await dbx.Files.UploadAsync(folderDB + "/" + nameFinal, Dropbox.Api.Files.WriteMode.Overwrite.Instance, body: mem);

                }
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Create a folder by the full path of the folder you want to create
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Boolean(succes,fail)</returns>
        public static Boolean createFolder(DropboxClient dbx, string name, string path)
        {
            path = path.Replace(@"\", "/");
            if (!path.StartsWith("/") && path!=string.Empty) { throw new System.InvalidOperationException("Invalid path"); }
            return createFolderAsyncAux(dbx,path+"/"+name).Result;
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox to create a folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Task<Boolean></returns>
        private static async Task<Boolean> createFolderAsyncAux(DropboxClient dbx, string path)
        {
            try
            {
                var folder = await dbx.Files.CreateFolderAsync(path);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Move metadata from a specific path(full path) to another, doesn't matter if it's file or folder
        /// </summary>
        /// <param name="pathFrom"></param>
        /// <param name="pathTo"></param>
        /// <returns>Boolean(succes,fail)</returns>
        public static Boolean moveMetadata(DropboxClient dbx, string pathFrom, string pathTo)
        {
            pathFrom = pathFrom.Replace(@"\", "/");
            pathTo = pathTo.Replace(@"\", "/");
            if (!pathFrom.StartsWith("/")) { throw new System.InvalidOperationException("Invalid pathFrom"); }
            if (!pathTo.StartsWith("/")) { throw new System.InvalidOperationException("Invalid pathTo"); }
            return moveMetadataAsyncAux(dbx,pathFrom, pathTo).Result;
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox to move metadata
        /// </summary>
        /// <param name="pathFrom"></param>
        /// <param name="pathTo"></param>
        /// <returns>Task<Boolean></returns>
        private static async Task<Boolean> moveMetadataAsyncAux(DropboxClient dbx, string pathFrom, string pathTo)
        {
            try
            {
                var move = await dbx.Files.MoveAsync(new Dropbox.Api.Files.RelocationArg(pathFrom, pathTo));
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Deletes a folder given his full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Boolean(succes,fail)</returns>
        public static Boolean deleteFolder(DropboxClient dbx, string path)
        {
            path = path.Replace(@"\", "/");
            if (!path.StartsWith("/")) { throw new System.InvalidOperationException("Invalid path"); }
            return deleteMetadataAsyncAux(dbx,path).Result;
        }
        /// <summary>
        /// Deletes a file given his full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Boolean(succes,fail)</returns>
        public static Boolean deleteFile(DropboxClient dbx, string path)
        {
            path = path.Replace(@"\", "/");
            if (!path.StartsWith("/")) { throw new System.InvalidOperationException("Invalid path"); }
            if (!Path.HasExtension(path)) { throw new System.InvalidOperationException("Invalid path, no file extension found"); }
            return deleteMetadataAsyncAux(dbx,path).Result;
        }
        /// <summary>
        /// Async aux method to call async methods on dropbox to delete metadata
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Task<Boolean></returns>
        private static async Task<Boolean> deleteMetadataAsyncAux(DropboxClient dbx, string path)
        {
            try
            {
                await dbx.Files.DeleteAsync(new Dropbox.Api.Files.DeleteArg(path));
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Get user name
        /// </summary>
        /// <returns>string</returns>
        public static string getName(DropboxClient dbx)
        {
            return getAccount(dbx).Result.Name.DisplayName;
        }
        /// <summary>
        /// Get user email
        /// </summary>
        /// <returns>string</returns>
        public static string getEmail(DropboxClient dbx)
        {
            return getAccount(dbx).Result.Email.ToString();
        }
        /// <summary>
        /// Get user accountID
        /// </summary>
        /// <returns>string</returns>
        public static string getAccountID(DropboxClient dbx)
        {
            return getAccount(dbx).Result.AccountId.ToString();
        }
        /// <summary>
        /// Get user photo uri,returns empty if the user has no photo
        /// </summary>
        /// <returns>string</returns>
        public static string getPhotoUri(DropboxClient dbx)
        {
            if (getAccount(dbx).Result.ProfilePhotoUrl == null) { return string.Empty; }
            return getAccount(dbx).Result.ProfilePhotoUrl.ToString();
        }
        /// <summary>
        /// Indicates if the user has a verified email
        /// </summary>
        /// <returns>boolean</returns>
        public static Boolean getVerifiedAccount(DropboxClient dbx)
        {
            return getAccount(dbx).Result.EmailVerified;
        }
        /// <summary>
        /// Get user locale
        /// </summary>
        /// <returns>string</returns>
        public static string getUserLocale(DropboxClient dbx)
        {
            return getAccount(dbx).Result.Locale;
        }
        /// <summary>
        /// Get user referal link
        /// </summary>
        /// <returns>string</returns>
        public static string getReferalLink(DropboxClient dbx)
        {
            return getAccount(dbx).Result.ReferralLink;
        }
        /// <summary>
        /// Get user country
        /// </summary>
        /// <returns>string</returns>
        public static string getCountry(DropboxClient dbx)
        {
            return getAccount(dbx).Result.Country;
        }
        /// <summary>
        /// Async aux method to get a full account object used on other methods to retrieve info from the user
        /// </summary>
        /// <returns>Full account object</returns>
        private static async Task<Dropbox.Api.Users.FullAccount> getAccount(DropboxClient dbx)
        {
            Dropbox.Api.Users.FullAccount cuenta = await dbx.Users.GetCurrentAccountAsync();
            return cuenta;
        }
        /// <summary>
        /// Gets a temporaly link that last 4 hours from a given full path of a file, you can download this file from there.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>String</returns>
        public static string GetTemporalyLinkFromFile(DropboxClient dbx, string path)
        {
            path = path.Replace(@"\", "/");
            if (!path.StartsWith("/")) { throw new System.InvalidOperationException("Invalid path"); }
            if (!Path.HasExtension(path)) { throw new System.InvalidOperationException("Invalid path, no file extension found"); }
            return getTemporalyLinkFromFileAux(dbx,path).Result;
        }
        /// <summary>
        /// Async aux method to call async dropbox methods to get a temporaly link
        /// </summary>
        /// <param name="path"></param>
        /// <returns>String</returns>
        private static async Task<string> getTemporalyLinkFromFileAux(DropboxClient dbx, string path)
        {
            var link = await dbx.Files.GetTemporaryLinkAsync(new Dropbox.Api.Files.GetTemporaryLinkArg(path));
            return link.Link;

        }
        /// <summary>
        /// Get the currently used space of the user
        /// </summary>
        /// <param name="dbx"></param>
        /// <returns></returns>
        public static ulong usedEspace(DropboxClient dbx)
        {
            var esp = dbx.Users.GetSpaceUsageAsync();
            return esp.Result.Used;

        }
        /// <summary>
        /// Get the current totalSpace of the user
        /// </summary>
        /// <param name="dbx"></param>
        /// <returns></returns>
        public static ulong totalSpace(DropboxClient dbx)
        {
            var esp = dbx.Users.GetSpaceUsageAsync();
            return esp.Result.Allocation.AsIndividual.Value.Allocated;
        }
 }

}
