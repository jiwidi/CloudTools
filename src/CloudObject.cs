using System;

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
namespace CloudTools
{
    public class CloudObject
    {
        public string Name;
        public Boolean isDropbox;
        public Boolean isDrive;
        //Aboce this is dropbox attributes
        public Dropbox.Api.Files.DeletedMetadata AsDeleted;
        public Dropbox.Api.Files.FileMetadata AsFile;
        public Dropbox.Api.Files.FolderMetadata AsFolder;
        public bool IsDeleted;
        public bool IsFile;
        public bool IsFolder;
        public string ParentSharedFolderId;
        public string PathDisplay;
        public string PathLower;

        //Above this is drive attributes
        public virtual IDictionary<string, string> AppProperties { get; set; }
        public File.CapabilitiesData Capabilities;
        public string originalFileName;
        public File.ContentHintsData ContentHints;
        public DateTime? CreatedTime;
        public string CreatedTimeRaw;
        public string Description;
        public string ETag;
        public bool? ExplicitlyTrashed;
        public string FileExtension;
        public string FolderColorRgb;
        public string FullFileExtension;
        public string HeadRevisionId;
        public string IconLink;
        public string Id;
        public File.ImageMediaMetadataData ImageMediaMetadata;
        public Boolean? IsAppAuthorized;
        public string Kind;
        public User LastModifyingUser;
        public string Md5Checksum;
        public string MimeType;
        public DateTime? ModifiedByMeTime;
        public string ModifiedByMeTimeRaw;
        public DateTime? ModifiedTime;
        public string ModifiedTimeRaw;
        public string OriginalFilename;
        public Boolean? OwnedByMe;
        public IList<User> Owners;
        public IList<string> Parents;
        public IList<Permission> Permissions;
        public IDictionary<string, string> Properties;
        public long? QuotaBytesUsed;
        public Boolean? Shared;
        public DateTime? SharedWithMeTime;
        public string SharedWithMeTimeRaw;
        public User SharingUser;
        public long? Size;
        public IList<string> Spaces;
        public Boolean? Starred;
        public string ThumbnailLink;
        public Boolean? Trashed;
        public long? Version;
        public File.VideoMediaMetadataData VideoMediaMetadata;
        public Boolean? ViewedByMe;
        public DateTime? ViewedByMeTime;
        public string ViewedByMeTimeRaw;
        public Boolean? ViewersCanCopyContent;
        public string WebContentLink;
        public string WebViewLink;
        public Boolean? WritersCanShare;
        public string Father;
        //

        public CloudObject(Dropbox.Api.Files.Metadata db, string father)
        {
            Name = db.Name;
            isDrive = false;
            isDropbox = true;
            AsDeleted = db.AsDeleted;
            AsFile = db.AsFile;
            AsFolder = db.AsFolder;
            IsDeleted = db.IsDeleted;
            IsFile = db.IsFile;
            IsFolder = db.IsFolder;
            ParentSharedFolderId = db.ParentSharedFolderId;
            PathDisplay = db.PathDisplay;
            PathLower = db.PathLower;
            Father = father;
            
        }
        public CloudObject(Google.Apis.Drive.v3.Data.File drive)
        {
            Name = drive.Name;
            isDrive = true;
            isDropbox = false;
            originalFileName = drive.OriginalFilename;
            Capabilities = drive.Capabilities;
            ContentHints = drive.ContentHints;
            CreatedTime = drive.CreatedTime;
            CreatedTimeRaw = drive.CreatedTimeRaw;
            Description = drive.Description;
            ETag = drive.ETag;
            ExplicitlyTrashed = drive.ExplicitlyTrashed;
            FileExtension = drive.FileExtension;
            FolderColorRgb = drive.FolderColorRgb;
            FullFileExtension = drive.FullFileExtension;
            HeadRevisionId = drive.HeadRevisionId;
            IconLink = drive.IconLink;
            Id = drive.Id;
            ImageMediaMetadata = drive.ImageMediaMetadata;
            IsAppAuthorized = drive.IsAppAuthorized;
            Kind = drive.Kind;
            LastModifyingUser = drive.LastModifyingUser;
            Md5Checksum = drive.Md5Checksum;
            MimeType = drive.MimeType;
            ModifiedByMeTime = drive.ModifiedByMeTime;
            ModifiedByMeTimeRaw=drive.ModifiedByMeTimeRaw;
            ModifiedTime = drive.ModifiedTime;
            ModifiedTimeRaw = drive.ModifiedTimeRaw;
            originalFileName = drive.OriginalFilename;
            OwnedByMe = drive.OwnedByMe;
            Owners = drive.Owners;
            Parents = drive.Parents;
            Permissions = drive.Permissions;
            Properties = drive.Properties;
            QuotaBytesUsed = drive.QuotaBytesUsed;
            Shared = drive.Shared;
            SharedWithMeTime = drive.SharedWithMeTime;
            SharedWithMeTimeRaw = drive.SharedWithMeTimeRaw;
            SharingUser = drive.SharingUser;
            Size = drive.Size;
            Spaces = drive.Spaces;
            Starred = drive.Starred;
            ThumbnailLink = drive.ThumbnailLink;
            Trashed = drive.Trashed;
            Version = drive.Version;
            VideoMediaMetadata = drive.VideoMediaMetadata;
            ViewedByMe = drive.ViewedByMe;
            ViewedByMeTime = drive.ViewedByMeTime;
            ViewedByMeTimeRaw = drive.ViewedByMeTimeRaw;
            ViewersCanCopyContent = drive.ViewersCanCopyContent;
            WebContentLink = drive.WebContentLink;
            WebViewLink = drive.WebViewLink;
            WritersCanShare = drive.WritersCanShare;

        }
    }
}
