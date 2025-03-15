using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos
{
    /// <summary>
    /// Data transfer object for privacy setting
    /// </summary>
    public class PrivacySettingDto
    {
        /// <summary>
        /// User's id
        /// </summary>
        /// 
        [Required]
        public string Field { get; set; } = "post";

        /// <summary>
        /// User's id
        /// </summary>
        /// 
        [Required]
        public string PrivacyLevel { get; set; } = "public";
    }
}
