﻿using System.Text.Json.Serialization;
namespace CodeHelper.API.LinkedIn.Common
{
    public class SpecificContent
    {
        #region Properties
        [JsonPropertyName("com.linkedin.ugc.ShareContent")] public ShareContent ShareContent { get; set; } = new();
        #endregion
    }

}
