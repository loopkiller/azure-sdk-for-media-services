﻿//-----------------------------------------------------------------------
// <copyright file="CloudMediaContext.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
// <license>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </license>

using System;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.MediaServices.Client.OAuth;
using Microsoft.WindowsAzure.MediaServices.Client.Versioning;

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// Describes the context from which all entities in the Microsoft WindowsAzure Media Services platform can be accessed.
    /// </summary>
    public partial class CloudMediaContext : MediaContextBase
    {
        /// <summary>
        /// The certificate thumbprint for Nimbus services.
        /// </summary>
        internal const string NimbusRestApiCertificateThumbprint = "AC24B49ADEF9D6AA17195E041D3F8D07C88EC145";

        /// <summary>
        /// The certificate subject for Nimbus services.
        /// </summary>
        internal const string NimbusRestApiCertificateSubject = "CN=NimbusRestApi";

        private static readonly Uri _mediaServicesUri = new Uri("https://media.windows.net/");

        private  AssetCollection _assets;
        private  AssetFileCollection _files;
        private  AccessPolicyBaseCollection _accessPolicies;
        private  ContentKeyCollection _contentKeys;
        private  JobBaseCollection _jobs;
        private  JobTemplateBaseCollection _jobTemplates;
        private  NotificationEndPointCollection _notificationEndPoints;
        private  MediaProcessorBaseCollection _mediaProcessors;
        private  LocatorBaseCollection _locators;
        private  IngestManifestCollection _ingestManifests;
        private  IngestManifestAssetCollection _ingestManifestAssets;
        private  IngestManifestFileCollection _ingestManifestFiles;
        private  StorageAccountBaseCollection _storageAccounts;
        private MediaServicesClassFactory _classFactory;
        private OAuthDataServiceAdapter dataServiceAdapter;
        private ServiceVersionAdapter versionAdapter;
        private Uri apiServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        public CloudMediaContext(string accountName, string accountKey)
            : this(CloudMediaContext._mediaServicesUri, new MediaServicesCredentials(accountName, accountKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="apiServer">A <see cref="Uri"/> representing a the API endpoint.</param>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        public CloudMediaContext(Uri apiServer, string accountName, string accountKey)
            : this(apiServer, new MediaServicesCredentials(accountName, accountKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="apiServer">A <see cref="Uri"/> representing a the API endpoint.</param>
        /// <param name="accountName">The Microsoft WindowsAzure Media Services account name to authenticate with.</param>
        /// <param name="accountKey">The Microsoft WindowsAzure Media Services account key to authenticate with.</param>
        /// <param name="scope">The scope of authorization.</param>
        /// <param name="acsBaseAddress">The access control endpoint to authenticate against.</param>
        public CloudMediaContext(Uri apiServer, string accountName, string accountKey, string scope, string acsBaseAddress)
            : this(apiServer, new MediaServicesCredentials(accountName, accountKey, scope, acsBaseAddress))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="credentials">Microsoft WindowsAzure Media Services credentials.</param>
        public CloudMediaContext(MediaServicesCredentials credentials)
            : this(_mediaServicesUri, credentials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMediaContext"/> class.
        /// </summary>
        /// <param name="apiServer">A <see cref="Uri"/> representing the API endpoint.</param>
        /// <param name="credentials">Microsoft WindowsAzure Media Services credentials.</param>
        public CloudMediaContext(Uri apiServer, MediaServicesCredentials credentials)
        {
            this.apiServer = apiServer;
            this.ParallelTransferThreadCount = 10;
            this.NumberOfConcurrentTransfers = 2;
            this.Credentials = credentials;
            dataServiceAdapter = new OAuthDataServiceAdapter(credentials, NimbusRestApiCertificateThumbprint, NimbusRestApiCertificateSubject);
            versionAdapter = new ServiceVersionAdapter(KnownApiVersions.Current);

        }

        public override MediaServicesClassFactory MediaServicesClassFactory
        {
            get
            {
                if (_classFactory == null)
                {
                    Interlocked.CompareExchange(ref _classFactory, new AzureMediaServicesClassFactory(apiServer, dataServiceAdapter, versionAdapter, this), null);
                }
                return _classFactory;
            }
            set
            {
                _classFactory = value;
            }
        } 

        /// <summary>
        /// Gets the collection of assets in the system.
        /// </summary>
        public override AssetBaseCollection Assets
        {
            get
            {
                if (_assets == null)
                {
                    Interlocked.CompareExchange(ref _assets, new AssetCollection(this), null);
                }
                return this._assets;
                
            }
        }

        /// <summary>
        /// Gets the collection of files in the system.
        /// </summary>
        public override AssetFileBaseCollection Files
        {
            get
            {
                if (_files == null)
                {
                    Interlocked.CompareExchange(ref _files, new AssetFileCollection(this), null);
                }
                return this._files;
                
            }
        }

        /// <summary>
        /// Gets the collection of access policies in the system.
        /// </summary>
        public override AccessPolicyBaseCollection AccessPolicies
        {
            get
            {
                if (_accessPolicies == null)
                {
                    Interlocked.CompareExchange(ref _accessPolicies, new AccessPolicyBaseCollection(this), null);
                }
                return this._accessPolicies;
               
            }
        }

        /// <summary>
        /// Gets the collection of content keys in the system.
        /// </summary>
        public override ContentKeyBaseCollection ContentKeys
        {
            get
            {
                if (_contentKeys == null)
                {
                    Interlocked.CompareExchange(ref _contentKeys, new ContentKeyCollection(this), null);
                }
                return this._contentKeys;
                
            }
        }

        /// <summary>
        /// Gets the collection of jobs available in the system.
        /// </summary>
        public override JobBaseCollection Jobs
        {
            get
            {
                if (_jobs == null)
                {
                    Interlocked.CompareExchange(ref _jobs, new JobBaseCollection(this), null);
                }
                return this._jobs;
            }
        }

        /// <summary>
        /// Gets the collection of job templates available in the system.
        /// </summary>
        public override JobTemplateBaseCollection JobTemplates
        {
            get
            {
                if (_jobTemplates == null)
                {
                    Interlocked.CompareExchange(ref _jobTemplates, new JobTemplateBaseCollection(this), null);
                } 
                return this._jobTemplates;
            }
        }

        /// <summary>
        /// Gets the collection of media processors available in the system.
        /// </summary>
        public override MediaProcessorBaseCollection MediaProcessors
        {
            get
            {
                if (_mediaProcessors == null)
                {
                    Interlocked.CompareExchange(ref _mediaProcessors, new MediaProcessorBaseCollection(this), null);
                }    
                return this._mediaProcessors;
                
            }
        }

        /// <summary>
        /// Gets a collection to operate on StorageAccounts.
        /// </summary>
        /// <seealso cref="StorageAccountBaseCollection" />
        ///   <seealso cref="IStorageAccount" />
        public override StorageAccountBaseCollection StorageAccounts
        {
            get
            {
                if (_storageAccounts == null)
                {
                    Interlocked.CompareExchange(ref _storageAccounts, new StorageAccountBaseCollection(this), null);
                }
                return this._storageAccounts;
               
            }
        }

        /// <summary>
        /// Returns default storage account
        /// </summary>
        public override IStorageAccount DefaultStorageAccount
        {
            get 
            { 
                return this.StorageAccounts.Where(c=>c.IsDefault == true).FirstOrDefault(); 
            }
        }

        /// <summary>
        /// Gets the collection of notification endpoints available in the system.
        /// </summary>
        public override NotificationEndPointCollection NotificationEndPoints
        {
            get
            {
                if (_notificationEndPoints == null)
                {
                    Interlocked.CompareExchange(ref _notificationEndPoints, new NotificationEndPointCollection(this), null);
                }
                return this._notificationEndPoints;
                
            }
        }

        /// <summary>
        /// Gets the collection of locators in the system.
        /// </summary>
        public override LocatorBaseCollection Locators
        {
            get
            {
                if (_locators == null)
                {
                    Interlocked.CompareExchange(ref _locators, new LocatorBaseCollection(this), null);
                }    
                return this._locators;
                
            }
        }

        /// <summary>
        /// Gets the collection of bulk ingest manifests in the system.
        /// </summary>
        public override IngestManifestCollection IngestManifests
        {
            get
            {
                if (_ingestManifests == null)
                {
                    Interlocked.CompareExchange(ref _ingestManifests, new IngestManifestCollection(this), null);
                }   
                return this._ingestManifests;
                
            }
        }

        /// <summary>
        /// Gets the collection of manifest asset files in the system
        /// </summary>
        public  override IngestManifestFileCollection IngestManifestFiles
        {
            get
            {
                if (_ingestManifestFiles == null)
                {
                    Interlocked.CompareExchange(ref _ingestManifestFiles, new IngestManifestFileCollection(this, null), null);
                }
                return this._ingestManifestFiles;
                
            }
        }

        /// <summary>
        /// Gets the collection of manifest assets in the system
        /// </summary>
        public override IngestManifestAssetCollection IngestManifestAssets
        {
            get
            {
                if (_ingestManifestAssets == null)
                {
                    Interlocked.CompareExchange(ref _ingestManifestAssets, new IngestManifestAssetCollection(this,null), null);
                }
                return this._ingestManifestAssets;
            }
        }
    }
}
