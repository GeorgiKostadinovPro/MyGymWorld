namespace MyGymWorld.Web.ViewModels.Managers.Gyms
{
    using CloudinaryDotNet.Actions;
    using System.Collections.Generic;

    public class GymLogoAndGalleryImagesInputModel
    {
        public ImageUploadResult LogoResultParams { get; set; } = null!;

        public ICollection<ImageUploadResult> GalleryImagesResultParams { get; set; } = new HashSet<ImageUploadResult>();
    }
}
