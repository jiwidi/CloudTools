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
using Dropbox.Api;
using Nito.AsyncEx;
using System.Windows.Forms;
using Google.Apis.Plus.v1.Data;


using Google.Apis.Plus.v1;





namespace CloudTools
{
    public class CloudManager
    {
        public DropboxClient dbx;
        public DriveService service;
        public PlusService serviceplus;
        // see https://developers.google.com/identity/sign-in/web/ to add google singin to a website so 


        /// <summary>
        /// CloudManager constructor for drive service
        /// </summary>
        /// <param name="user"></param>
        /// <param name="jsonpath"></param>
        public CloudManager(string user,string jsonpath)//Drive
        {
            UserCredential credential = Drive.GetGoogleOAuthCredential( user,@jsonpath);
            service = Drive.getService(credential);
            serviceplus = Drive.getPlusService(credential);
            
        }
        /// <summary>
        /// CloudManager constructor for dropbox, it needs the Uri from getFirstHalfToken
        /// </summary>
        /// <param name="firstHalfToken"></param>
        public CloudManager(string firstHalfToken)//Dropbox auth builder, need the uri from getFirstHalfToken
        {
            dbx = dropbox.secondAuthUser(firstHalfToken);
        }
        /// <summary>
        /// Auxiliar method for the  CloudManager dropbox constructor
        /// </summary>
        /// <returns></returns>
        public static Uri getFirstHalfToken()
        {
            return dropbox.firstAuthUser();
        }
        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="fileNameOrID"></param>
        /// <param name="pathTosave"></param>
        /// <param name="folderDropbox"></param>
        /// <returns></returns>
        public Boolean downloadFile(string fileNameOrID,string pathTosave, string folderDropbox = "")
        {
            try
            {
                if (dbx!=null) { return dropbox.downloadFile(dbx, fileNameOrID, folderDropbox, pathTosave); }
                else if(service!=null) { return Drive.downloadBinaryFile(service, fileNameOrID, pathTosave); }
                return true;
            }
            catch(Exception) { return false; }
        }
        /// <summary>
        /// Test method
        /// </summary>
        public void test()
        {
            Console.WriteLine(service.ApiKey + service.ApplicationName);
        }
        /// <summary>
        /// Update a file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="pathTosaveDBorPARENTID"></param>
        /// <param name="nameFinal"></param>
        /// <param name="mimetype"></param>
        /// <returns></returns>
        public Boolean updateFile(string filepath, string pathTosaveDBorPARENTID, string nameFinal,string mimetype="")
        {
            try
            {
                if (dbx != null) { return dropbox.uploadFile(dbx, filepath, pathTosaveDBorPARENTID, nameFinal); }
                else if (service != null) { return Drive.uploadFile(service, filepath, pathTosaveDBorPARENTID, nameFinal,mimetype); }
                return true;
            }
            catch (Exception) { return false; }
        }
        /// <summary>
        /// Upload a file, it requires id of parent for drive or parentfoldername for dropbox
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nameFinal"></param>
        /// <param name="IDParentorFolderDropbox"></param>
        /// <param name="mimetype"></param>
        /// <returns></returns>
        public Boolean uploadFile(string filePath, string nameFinal, string IDParentorFolderDropbox = "",string mimetype="")
        {
            try
            {
                if (dbx!=null) { dropbox.uploadFile(dbx,filePath,IDParentorFolderDropbox,nameFinal); }
                else if (service!=null) { Drive.uploadFile(service, filePath, IDParentorFolderDropbox, nameFinal,mimetype); }
                return true;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return false; }
        }
        /// <summary>
        /// Create a folder, it requires id of parent for drive or parentfoldername for dropbox
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentIDORdropboxPath"></param>
        /// <returns></returns>
        public Boolean createFolder(string name, string parentIDORdropboxPath)
        {
            try
            {
                if (dbx!=null) { dropbox.createFolder(dbx, name, parentIDORdropboxPath); }
                else if (service!=null) { Drive.createDirectory(service, name, parentIDORdropboxPath); }
                return true;
            }
            catch(Exception) { return false; }
        }
        /// <summary>
        /// Get MimeType from a file (only drive)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string getMimeType(string filename)
        {
            if (service != null)
            {
                return Drive.GetMimeType(filename);
            }
            else return "";
        }
        /// <summary>
        /// Get's all files/folders  under a folder, it requires id of parent for drive or parentfoldername for dropbox
        /// </summary>
        /// <param name="folderDBorPARENTID"></param>
        /// <returns></returns>
        public List<CloudObject> getFilesUnder(string folderDBorPARENTID)
        {
            try
            {
                if (dbx!=null) { return dropbox.retrieveFolder(dbx, folderDBorPARENTID); }
                else if (service!=null) { return Drive.getFilesFromParent(service, folderDBorPARENTID); }
                return null;
            }
            catch(Exception) { return null; }
        }
        /// <summary>
        /// Returns drive files filtered by using the string as parameter, see https://developers.google.com/drive/v3/web/search-parameters for more info
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<CloudObject> getFilesFiltered(string filter)
        {
            try
            {
                if (dbx != null) { return null; }
                else if (service != null) { return Drive.getFilesFiltered(service, filter); }
                return null;
            }
            catch (Exception) { return null; }
        }
        /// <summary>
        /// Get's all files/folders from the root
        /// </summary>
        /// <returns></returns>
        public List<CloudObject> getALLfiles()
        {
            if (dbx!=null) { return dropbox.retrieveAllFilesUnder(dbx, string.Empty); }
            else if (service!=null) { return Drive.getAllFiles(service); }
            throw new System.ArgumentException("Invalid parameters");
        }
        /// <summary>
        /// Delete a folder
        /// </summary>
        /// <param name="folderIDorPATH"></param>
        /// <returns></returns>
        public Boolean deleteFolder(string folderIDorPATH)
        {
            try
            {
                if (dbx!=null) { return dropbox.deleteFolder(dbx, folderIDorPATH); }
                else if (service!=null) { return Drive.permantlyDeleteFolder(service, folderIDorPATH); }
            }
            catch (Exception) { return false; }
            return false;
            
        }
        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="fileIDorPATH"></param>
        /// <returns></returns>
        public Boolean deleteFile( string fileIDorPATH)
        {
            try
            {
                if (dbx != null) { return dropbox.deleteFile(dbx, fileIDorPATH); }
                else if (service != null) { return Drive.permantlyDeleteFolder(service, fileIDorPATH); }
            }
            catch (Exception) { return false; }
            return false;
        }
        /// <summary>
        /// Move a file or folder, it requires id of parent for drive or parentfoldername for dropbox
        /// </summary>
        /// <param name="pathDBFROMorFILEID"></param>
        /// <param name="pathDBTOorNEWPARENT"></param>
        /// <returns></returns>
        public Boolean moveFileOrFolder(string pathDBFROMorFILEID, string pathDBTOorNEWPARENT)
        {
            try
            {
                if (dbx != null) { return dropbox.moveMetadata(dbx, pathDBFROMorFILEID,pathDBTOorNEWPARENT); }
                else if (service != null) { return Drive.moveFile(service, pathDBFROMorFILEID, pathDBTOorNEWPARENT); }
            }
            catch (Exception) { return false; }
            return false;
        }
        /// <summary>
        /// Empty the trash
        /// </summary>
        /// <returns></returns>
        public Boolean emptyTrash()
        {
            try
            {
                if (dbx != null) { return false;  }
                else if (service != null) {Drive.emptyTrash(service) ; }
                return true;
            }
            catch (Exception) { return false; }
            
        }
        /// <summary>
        /// Get's the user name
        /// </summary>
        /// <returns></returns>
        public string getName()
        {
            if (dbx!=null) { return dropbox.getName(dbx); }
            else if (serviceplus!=null) { return Drive.getName(serviceplus); }
            return string.Empty;
        }
        /// <summary>
        /// Get's the user email
        /// </summary>
        /// <returns></returns>
        public string getEmail()
        {
            if (dbx!=null) { return dropbox.getEmail(dbx); }
            else if (serviceplus!=null) { return Drive.getEmail(serviceplus); }
            return string.Empty;
        }
        /// <summary>
        /// Get's the user AccountId
        /// </summary>
        /// <returns></returns>
        public string getAccountID()
        {
            if (dbx!=null) { return dropbox.getAccountID(dbx); }
            else if (serviceplus!=null) { return Drive.getAccountId(serviceplus); }
            return string.Empty;
        }
        /// <summary>
        /// Get's the user photoUrl
        /// </summary>
        /// <returns></returns>
        public string getPhotoUrl()
        {
            if (dbx!=null) { return dropbox.getPhotoUri(dbx); }
            else if (serviceplus!=null) { return Drive.getPhotoUrl(serviceplus); }
            return string.Empty;
        }
        /// <summary>
        /// Check if the user has verified his account
        /// </summary>
        /// <returns></returns>
        public bool? getVerified()
        {
            if (dbx!=null) { return dropbox.getVerifiedAccount(dbx); }
            else if (serviceplus!=null) { return Drive.verfiedAccount(serviceplus); }
            return false;
        }
        /// <summary>
        /// Get's the user location
        /// </summary>
        /// <returns></returns>
        public string getLocation()
        {
            if (dbx!=null) { return dropbox.getCountry(dbx); }
            else if (serviceplus!=null) { return Drive.getCurrentLocation(serviceplus); }
            return string.Empty;

        }
        /// <summary>
        /// Get's the user free space
        /// </summary>
        /// <returns></returns>
        public ulong? getFreeSpace()
        {
            if (dbx != null) { return (dropbox.totalSpace(dbx) - dropbox.usedEspace(dbx)); }
            else if (service != null) { return (ulong) (Drive.totalSpace(service)- Drive.usedSpace(service)); }
            return null;
        }
        /// <summary>
        /// Get's the user used space
        /// </summary>
        /// <returns></returns>
        public ulong? getUsedSpace()
        {
            if (dbx != null) { return (dropbox.usedEspace(dbx)); }
            else if (service != null) { return (ulong)Drive.usedSpace(service); }
            return null;
        }
        /// <summary>
        /// Get's the user total space
        /// </summary>
        /// <returns></returns>
        public ulong? getTotalSpace()
        {
            if (dbx != null) { return (dropbox.totalSpace(dbx)); }
            else if (service != null) { return (ulong)Drive.totalSpace(service); }
            return null;
        }


    }
        

    }
