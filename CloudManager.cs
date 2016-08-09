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
using Google.Apis.Util.Store;

using Google.Apis.Plus.v1;





namespace CloudTools
{
    public class CloudManager
    {
        public DropboxClient dbx;
        public DriveService service;
        public PlusService serviceplus;
        // see https://developers.google.com/identity/sign-in/web/ to add google singin to a website so 



        public CloudManager(string user,string jsonpath)//Drive
        {
            UserCredential credential = Drive.GetGoogleOAuthCredential( user,@jsonpath);
            service = Drive.getService(credential);
            serviceplus = Drive.getPlusService(credential);
            
        }
        public CloudManager(string firstHalfToken)//Dropbox auth builder, need the uri from getFirstHalfToken
        {
            dbx = dropbox.secondAuthUser(firstHalfToken);
        }
        public static Uri getFirstHalfToken()
        {
            return dropbox.firstAuthUser();
        }
        public Boolean downloadFile(string fileNameOrID,string pathTosave, string folderDropbox = "")
        {
            try
            {
                if (dbx!=null) { return dropbox.downloadFile(dbx, fileNameOrID, folderDropbox, pathTosave); }
                else if(service!=null) { return Drive.downloadBinaryFile(service, fileNameOrID, pathTosave); }
                return true;
            }
            catch(Exception e) { return false; }
        }
        public void test()
        {
            Console.WriteLine(service.ApiKey + service.ApplicationName);
        }
        public Boolean updateFile(string filepath, string pathTosaveDBorPARENTID, string nameFinal)
        {
            try
            {
                if (dbx != null) { return dropbox.uploadFile(dbx, filepath, pathTosaveDBorPARENTID, nameFinal); }
                else if (service != null) { return Drive.uploadFile(service, filepath, pathTosaveDBorPARENTID, nameFinal); }
                return true;
            }
            catch (Exception e) { return false; }
        }
        public Boolean uploadFile(string filePath, string nameFinal, string IDParentorFolderDropbox = "")
        {
            try
            {
                if (dbx!=null) { dropbox.uploadFile(dbx,filePath,IDParentorFolderDropbox,nameFinal); }
                else if (service!=null) { Drive.uploadFile(service, filePath, IDParentorFolderDropbox, nameFinal); }
                return true;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return false; }
        }

        public Boolean createFolder(string name, string parentIDORdropboxPath)
        {
            try
            {
                if (dbx!=null) { dropbox.createFolder(dbx, name, parentIDORdropboxPath); }
                else if (service!=null) { Drive.createDirectory(service, name, parentIDORdropboxPath); }
                return true;
            }
            catch(Exception e) { return false; }
        }
        public string getMimeType(string filename)
        {
           return  Drive.GetMimeType(filename);
        }
        public List<CloudObject> getFilesUnder(string folderDBorPARENTID)
        {
            try
            {
                if (dbx!=null) { return dropbox.retrieveFolder(dbx, folderDBorPARENTID); }
                else if (service!=null) { return Drive.getFilesFromParent(service, folderDBorPARENTID); }
                return null;
            }
            catch(Exception e) { return null; }
        }
        public List<CloudObject> getALLfiles()
        {
            if (dbx!=null) { return dropbox.retrieveAllFilesUnder(dbx, string.Empty); }
            else if (service!=null) { return Drive.getAllFiles(service); }
            throw new System.ArgumentException("Invalid parameters");
        }
      
        public Boolean deleteFolder(string folderIDorPATH)
        {
            try
            {
                if (dbx!=null) { return dropbox.deleteFolder(dbx, folderIDorPATH); }
                else if (service!=null) { return Drive.permantlyDeleteFolder(service, folderIDorPATH); }
            }
            catch (Exception e) { return false; }
            return false;
            
        }
        public Boolean deleteFile( string fileIDorPATH)
        {
            try
            {
                if (dbx != null) { return dropbox.deleteFile(dbx, fileIDorPATH); }
                else if (service != null) { return Drive.permantlyDeleteFolder(service, fileIDorPATH); }
            }
            catch (Exception e) { return false; }
            return false;
        }
        public Boolean moveFileOrFolder(string pathDBFROMorFILEID, string pathDBTOorNEWPARENT)
        {
            try
            {
                if (dbx != null) { return dropbox.moveMetadata(dbx, pathDBFROMorFILEID,pathDBTOorNEWPARENT); }
                else if (service != null) { return Drive.moveFile(service, pathDBFROMorFILEID, pathDBTOorNEWPARENT); }
            }
            catch (Exception e) { return false; }
            return false;
        }
        public Boolean emptyTrash()
        {
            try
            {
                if (dbx != null) { return false;  }
                else if (service != null) {Drive.emptyTrash(service) ; }
                return true;
            }
            catch (Exception e) { return false; }
            return false;
        }
        public string getName()
        {
            if (dbx!=null) { return dropbox.getName(dbx); }
            else if (serviceplus!=null) { return Drive.getName(serviceplus); }
            return string.Empty;
        }
        public string getEmail()
        {
            if (dbx!=null) { return dropbox.getEmail(dbx); }
            else if (serviceplus!=null) { return Drive.getEmail(serviceplus); }
            return string.Empty;
        }
        public string getAccountID()
        {
            if (dbx!=null) { return dropbox.getAccountID(dbx); }
            else if (serviceplus!=null) { return Drive.getAccountId(serviceplus); }
            return string.Empty;
        }
        public string getPhotoUrl()
        {
            if (dbx!=null) { return dropbox.getPhotoUri(dbx); }
            else if (serviceplus!=null) { return Drive.getPhotoUrl(serviceplus); }
            return string.Empty;
        }
        public bool? getVerified()
        {
            if (dbx!=null) { return dropbox.getVerifiedAccount(dbx); }
            else if (serviceplus!=null) { return Drive.verfiedAccount(serviceplus); }
            return false;
        }
        public string getLocation()
        {
            if (dbx!=null) { return dropbox.getCountry(dbx); }
            else if (serviceplus!=null) { return Drive.getCurrentLocation(serviceplus); }
            return string.Empty;
        }

    }
        

    }
