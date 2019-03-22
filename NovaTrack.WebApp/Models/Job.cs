using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NovaTrack.WebApp.Models
{
    public partial class Job
    {
        public virtual Guid Id { get; set; }
        [DisplayName("Job Type")]
        public JobType JobType { get; set; }

        [DisplayName("Create Date")]
        public DateTime CreateDate { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        public string Premise { get; set; }
        public string OtherPremise { get; set; }

        [DisplayName("Company Address")]
        public string CompanyAddress { get; set; }

        [DisplayName("Company Phone Number")]
        public string CompanyPhoneNumber { get; set; }

        [DisplayName("Company Name")]
        public string CustomerName { get; set; }

        [DisplayName("Asset Barcode")]
        public string AssetBarCode { get; set; }

        [DisplayName("Device Barcode")]
        public string DeviceBarCode { get; set; }

        [DisplayName("Asset Type")]
        public string AssetType { get; set; }
        //public string LocalAssetPhoto { get; set; }
        //public string LocalDevicePhoto { get; set; }
        [DisplayName("Device Id")]
        public string DeviceId { get; set; }

        [DisplayName("Asset Id")]
        public string AssetId { get; set; }
        //public bool AssetPhotoUploaded { get; set; }
        //public bool DevicePhotoUploaded { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string LocalAssetPhoto { get; set; }
        public string LocalDevicePhoto { get; set; }

        public ApplicationUser User { get; set; }

    }

    public enum JobType
    {
        Created = 0,
        Uploaded = 1
    }
}
