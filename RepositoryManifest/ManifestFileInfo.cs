﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

using Utilities;


namespace RepositoryManifest
{
    /// <summary>
    /// Information about a file in the repository
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ManifestFileInfo : ManifestObjectInfo, ISerializable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// The file name
        /// </param>
        /// <param name="parentDirectory">
        /// The parent directory
        /// </param>
        public ManifestFileInfo(
            String name,
            ManifestDirectoryInfo parentDirectory) :
            base(name, parentDirectory)
        {
            FileHash = null;
        }

        /// <summary>
        /// Default constructor needed for json.NET
        /// </summary>
        public ManifestFileInfo() :
            base("", null)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="original">
        /// The original object
        /// </param>
        /// <param name="parentDirectory">
        /// The parent directory
        /// </param>
        public ManifestFileInfo(
            ManifestFileInfo original,
            ManifestDirectoryInfo parentDirectory) :
            base(original.Name, parentDirectory)
        {
            FileLength = original.FileLength;
            LastModifiedUtc = original.LastModifiedUtc;
            RegisteredUtc = original.RegisteredUtc;

            FileHash = original.FileHash;
        }

        /// <summary>
        /// The length of this file in bytes
        /// </summary>
        [JsonProperty]
        public Int64 FileLength { set; get; }

        /// <summary>
        /// The time that this file was last mofified according to the filesystem
        /// </summary>
        [JsonProperty]
        public DateTime LastModifiedUtc { set; get; }

        /// <summary>
        /// The time that this file was first put into the manifest
        /// </summary>
        [JsonProperty]
        public DateTime RegisteredUtc { set; get; }

        /// <summary>
        /// The hash of the file data
        /// </summary>
        [JsonProperty]
        public FileHash FileHash { set; get; }

        // I had to add this for some reason to fix a problem caused by a bug
        // in Mono that was causing serialization to fail.
        protected ManifestFileInfo(
            SerializationInfo info,
            StreamingContext context) :
            base(
                info.GetString("ManifestObjectInfo+<Name>k__BackingField"),
                (ManifestDirectoryInfo)
                    info.GetValue(
                        "ManifestObjectInfo+<ParentDirectory>k__BackingField",
                        typeof(ManifestDirectoryInfo)))
        {
            FileLength = info.GetInt64("<FileLength>k__BackingField");
            LastModifiedUtc = info.GetDateTime("<LastModifiedUtc>k__BackingField");

            try
            {
                RegisteredUtc = info.GetDateTime("<RegisteredUtc>k__BackingField");
            }
            catch (Exception) { }

            try
            {
                FileHash = (FileHash)info.GetValue("<FileHash>k__BackingField", typeof(FileHash));
            }
            catch (Exception) { }
        }

        public virtual void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("ManifestObjectInfo+<Name>k__BackingField", Name);
            info.AddValue("ManifestObjectInfo+<ParentDirectory>k__BackingField", ParentDirectory);
            info.AddValue("<FileLength>k__BackingField", FileLength);
            info.AddValue("<LastModifiedUtc>k__BackingField", LastModifiedUtc);
            info.AddValue("<RegisteredUtc>k__BackingField", RegisteredUtc);
            info.AddValue("<FileHash>k__BackingField", FileHash);
        }
    }
}
