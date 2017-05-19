using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTools
{
    public interface CloudDocInterface
    {
        Boolean downloadFile(string fileName, string folderCLOUD, string folderPC);
        Boolean uploadFile(string filePath, string folderCLOUD, string nameFinal);
        Boolean createFolder(string folderPath, string folderName);
        Boolean moveMetadata(string pathFrom,string pathTo);
        Boolean deleteFolder(string folderPath);
        Boolean deleteFile(string filePath);
        string getName();
        string getEmail();
        string getAccountID();
        string getPhotoUri();
        Boolean getVerifiedEmail();
        string getUserLocale();
        string getReferalLink();
        string getCountry();
        string GetTemporalyLinkFromFile(string path);
    }
}
