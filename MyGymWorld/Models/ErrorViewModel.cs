namespace MyGymWorld.Models
{
    using Microsoft.AspNetCore.Authentication;

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}