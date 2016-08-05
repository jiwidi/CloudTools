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



namespace FLEXYGO
{
    class CloudManager
    {
        private DropboxClient dbx;
        private DriveService service;
        

        public CloudManager(string user,string jsonpath)//Drive
        {
            service = Drive.GetGoogleOAuthCredential(user,jsonpath);
        }
        public CloudManager(string firstHalfToken)//Dropbox auth builder, need the uri from getFirstHalfToken
        {
            dbx = dropbox.secondAuthUser(firstHalfToken);
        }
        public static Uri getFirstHalfToken()
        {
            return dropbox.firstAuthUser();
        }
        public Boolean downloadFile(Object autenti,string fileNameOrID,string pathTosave, string folderDropbox = "")
        {
            try
            {
                if (autenti == dbx) { dropbox.downloadFile(dbx, fileNameOrID, folderDropbox, pathTosave); }
                else if(autenti == service) { Drive.downloadBinaryFile(service, fileNameOrID, pathTosave); }
                return true;
            }
            catch(Exception e) { return false; }
        }
        public Boolean updateFile(Object autenti, string filePath, string nameFinal, string IDParentorFolderDropbox = "")
        {
            try
            {
                if (autenti == dbx) { dropbox.uploadFile(dbx,filePath,IDParentorFolderDropbox,nameFinal); }
                else if (autenti == service) { Drive.uploadFile(service, filePath, IDParentorFolderDropbox, nameFinal); }
                return true;
            }
            catch (Exception e) { return false; }
        }
        public Boolean createFolder(Object autenti,string name, string parentIDORdropboxPath)
        {
            try
            {
                if (autenti == dbx) { dropbox.createFolder(dbx, name, parentIDORdropboxPath); }
                else if (autenti == service) { Drive.createDirectory(service, name, parentIDORdropboxPath); }
                return true;
            }
            catch(Exception e) { return false; }
        }
        public string getMimeType(string filename)
        {
           return  Drive.GetMimeType(filename);
        }
        public List<CloudObject> getFiles(Object autenti, string folderDB)
        {
            try
            {
                if (autenti == dbx) { return dropbox.retrieveFolder(dbx, folderDB); }
                else if (autenti == service) { return Drive.getAllFiles(service); }
                return null;
            }
            catch(Exception e) { return null; }
        }
        public Boolean deleteFolder(Object auntenti, string folderIDorPATH)
        {
            try
            {
                if (auntenti == dbx) { return dropbox.deleteFolder(dbx, folderIDorPATH); }
                else if (auntenti == service) { return Drive.permantlyDeleteFolder(service, folderIDorPATH); }
            }
            catch (Exception e) { return false; }
            return false;
            
        }
        public Boolean deleteFile(Object auntenti, string fileIDorPATH)
        {
            try
            {
                if (auntenti == dbx) { return dropbox.deleteFile(dbx, fileIDorPATH); }
                else if (auntenti == service) { return Drive.permantlyDeleteFolder(service, fileIDorPATH); }
            }
            catch (Exception e) { return false; }
            return false;
        }
        public Boolean moveFileOrFolder(Object auntenti, string pathDBFROMorFILEID, string pathDBTOorNEWPARENT)
        {
            try
            {
                if (auntenti == dbx) { return dropbox.moveMetadata(dbx, pathDBFROMorFILEID,pathDBTOorNEWPARENT); }
                else if (auntenti == service) { return Drive.moveFile(service, pathDBFROMorFILEID, pathDBTOorNEWPARENT); }
            }
            catch (Exception e) { return false; }
            return false;
        }

    }
}
