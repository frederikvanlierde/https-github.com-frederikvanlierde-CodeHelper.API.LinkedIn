﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Net.Http;
namespace CodeHelper.API.LinkedIn
{
    /// <summary>
    /// Documentation: https://docs.microsoft.com/en-us/linkedin/consumer/integrations/self-serve/share-on-linkedin?context=linkedin%2Fconsumer%2Fcontext
    /// </summary>
    public class LinkedInHelper
    {
        #region Properties
        private readonly HttpClient _httpClient = new();
        private string _AuthorID = "";
        public string AccessToken { get; set; } = "";        
        public string AuthorID { get { return "urn:li:person:" + _AuthorID; } 
                                 set { _AuthorID = value; } }
        #endregion

        #region Constructo
        public LinkedInHelper() { }
        #endregion

        #region Public Methods
        public async Task ShareTextMessage(string textMessage, string visibility = CodeHelper.API.LinkedIn.VisibilityTypes.Public)
        {
            Post _post = new() { Author = this.AuthorID };
            _post.Visibility.VisibiltyTpe = visibility;
            _post.SpecificContent.ShareContent = new Common.ShareContent() { ShareCommentary = new() {Text=textMessage } };

            await PostJson(Constants.APIURL_POST, _post.GetJsonString());
        }
        /// <summary>
        /// Create an Article or URL Share
        /// </summary>
        /// <param name="textMessage">Mandatory: Provides the primary content for the share.</param>
        /// <param name="url">Mandatory: Provide the URL of the article you would like to share here.</param>
        /// <param name="visibility">Mandatory
        /// Defines any visibility restrictions for the share. Possible values include:
        /// ● CONNECTIONS - The share will be viewable by 1st-degree connections only.
        /// ● PUBLIC - The share will be viewable by anyone on LinkedIn.
        /// </param>
        /// <param name="articleTitle">Optional: Customize the title of your image or article.</param>
        /// <param name="articleDescription">Optional: Provide a short description for your image or article.</param>
        /// <returns></returns>
        public async Task ShareUrl(string textMessage, string eUrl, string visibility = CodeHelper.API.LinkedIn.VisibilityTypes.Public,  string articleTitle=null, string articleDescription=null)
        {
            Post _post = new() { Author = this.AuthorID };
            _post.Visibility.VisibiltyTpe = visibility;
            _post.SpecificContent.ShareContent = new Common.ShareContent() { ShareCommentary = new() { Text = textMessage }, ShareMediaCategory = ShareMediaCategoryTypes.Article };
            _post.SpecificContent.ShareContent.Media = new();
            _post.SpecificContent.ShareContent.Media.Add(new() { OriginalUrl = eUrl, Title = new() { Text = articleTitle }, Description = new() { Text =  articleDescription } });
            await PostJson(Constants.APIURL_POST, _post.GetJsonString());
        }

        public async Task GetMe()
        {
            SetAuthorizationHeader();
            await GetJson(Constants.APIURL_ME);
        
        }
        #endregion

        #region Private Methods        
        protected async Task<string> PostJson(string apiURL, HttpContent data)
        {
            SetAuthorizationHeader();
            var _task = await _httpClient.PostAsync(apiURL, data);
            var _r  = await _task.Content.ReadAsStringAsync();
            return _r;
        }
        protected async Task<string> GetJson(string apiURL)
        {
            SetAuthorizationHeader();            
            var _task = await _httpClient.GetAsync(apiURL);
            var _r = await _task.Content.ReadAsStringAsync();
            return _r;
        }
        private void SetAuthorizationHeader()
        {
            if (_httpClient.DefaultRequestHeaders.Contains("X-Restli-Protocol-Version"))
                _httpClient.DefaultRequestHeaders.Remove("X-Restli-Protocol-Version");
            _httpClient.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);
        }
        #endregion
    }
}