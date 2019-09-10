using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextbinChannel.Services;
using Textchannel.Models;

namespace Textchannel.Services
{
    public class AppState
    {
        /// <summary>
        /// Array of all Textbin categories
        /// </summary>
        public Category[] Categories { get; private set; }
        /// <summary>
        /// Array of all Textbin Bulletins
        /// </summary>
        public Bulletin[] Bulletins { get; set; }
        /// <summary>
        /// The currently selected category
        /// </summary>
        public Category CurrentCategory { get; private set; }
        /// <summary>
        /// Posts of the currently selected category
        /// </summary>
        public Post[] CurrentCategoryPosts { get; private set; }
        /// <summary>
        /// The currently selected post
        /// </summary>
        public Post CurrentPost { get; private set; }
        /// <summary>
        /// Comments of the currently selected post
        /// </summary>
        public Comment[] CurrentPostComments { get; private set; }
        /// <summary>
        /// The Captcha for the current Comment/Post form
        /// </summary>
        public Captcha CurrentCaptcha { get; private set; }
        /// <summary>
        /// Event invoked when the state has changed
        /// </summary>
        public event Action OnChange;
        /// <summary>
        /// Toast spawn-er, yum!
        /// </summary>
        private readonly ToastService _toastService;
        /// <summary>
        /// Api service
        /// </summary>
        private readonly Api _api;

        public AppState(ToastService toastService, Api api)
        {
            _toastService = toastService;
            _api = api;
        }

        #region Creation Methods
        public async Task<Post> CreatePostAsync(string title, string text)
        {
            var createdPost = await _api.CreatePostAsync(title, text, CurrentCategory.Id);
            if (createdPost != null)
                _toastService.ShowToast("The post was created!", ToastLevel.Success);

            return createdPost;
        }

        public async Task CreateCommentAsync(string captchaSolution, string postId, string comment, string name, string email)
        {
            if (await _api.CreateCommentAsync(captchaSolution, postId, comment, name, email))
            {
                _toastService.ShowToast("Comment was successfully created!", ToastLevel.Success);
                // update current Post's comments
                await SetCurrentPostAsync(postId);
            }
            else
            {
                // reset captcha
                await GetCaptchaAsync();
            }
        }
        #endregion

        #region Get Methods 
        public async Task GetCaptchaAsync()
        {
            CurrentCaptcha = null;
            CurrentCaptcha = await _api.GetCaptchaAsync();
            if (CurrentCaptcha == null)
                _toastService.ShowToast("Unable to fetch captcha!", ToastLevel.Error);

            NotifyStateChanged();
        }

        public async Task GetCategoriesAsync()
        {
            Categories = null;
            Categories = await _api.GetCategoriesAsync();
            if (Categories == null)
                _toastService.ShowToast("Unable to fetch categories!", ToastLevel.Error);

            NotifyStateChanged();
        }

        public async Task GetBulletinsAsync()
        {
            Bulletins = null;
            Bulletins = await _api.GetBulletinsAsync();
            if (Bulletins == null)
                _toastService.ShowToast("Unable to fetch bulletiens!", ToastLevel.Error);

            NotifyStateChanged();
        }
        #endregion

        #region Set Methods
        public async Task SetCurrentCategoryAsync(string code)
        {
            // reset state
            ResetCurrentStates();

            if (Categories == null)
                await GetCategoriesAsync();

            var targetCategory = new List<Category>(Categories).FirstOrDefault(c => c.Code == code);
            if (targetCategory == null)
                _toastService.ShowToast("The specified category couldn't be found!", ToastLevel.Error);

            CurrentCategory = targetCategory;

            // get posts of the category
            await SetCurrentCategoryPostsAsync();

            NotifyStateChanged();
        }

        public async Task SetCurrentPostAsync(string postId)
        {
            // hold on to category before reseting state
            var category = CurrentCategory;
            // reset state
            ResetCurrentStates();
            CurrentCategory = category;

            if (CurrentCategoryPosts == null)
                await SetCurrentCategoryPostsAsync();

            CurrentPost = new List<Post>(CurrentCategoryPosts).FirstOrDefault(p => p.Id == postId);
            if (CurrentPost == null)
                _toastService.ShowToast("The specified post couldn't be found!", ToastLevel.Error);

            // get comments of the post
            await SetCurrentPostCommentsAsync();
            // prepend comments with OP comment
            var prependedList = new List<Comment>(CurrentPostComments);
            // insert into list post grabbed from api (the get_post gives more detailed data than posts filtered through latest_posts)
            prependedList.Insert(0, Comment.PostToComment(await _api.GetPostAsync(postId)));
            CurrentPostComments = prependedList.ToArray();

            NotifyStateChanged();
        }

        public async Task SetCurrentPostAsync(Post post)
        {
            // hold on to category before reseting state
            var category = CurrentCategory;
            // reset state
            ResetCurrentStates();
            CurrentCategory = category;
            CurrentPost = post;

            // get comments of the post
            await SetCurrentPostCommentsAsync();
            // prepend comments with OP comment
            var prependedList = new List<Comment>(CurrentPostComments);
            // insert into list the op post grabbed from api (the /get_post endpoint gives more detailed data than posts through latest_posts)
            prependedList.Insert(0, Comment.PostToComment(post));
            CurrentPostComments = prependedList.ToArray();

            NotifyStateChanged();
        }

        private async Task SetCurrentCategoryPostsAsync()
        {
            CurrentCategoryPosts = await _api.GetPostsAsync(CurrentCategory);

            NotifyStateChanged();
        }

        private async Task SetCurrentPostCommentsAsync()
        {
            CurrentPostComments = await _api.GetCommentsAsync(CurrentPost);

            NotifyStateChanged();
        }
        #endregion

        private void ResetCurrentStates()
        {
            //Categories = null;
            CurrentCategory = null;
            CurrentCategoryPosts = null;
            CurrentPost = null;
            CurrentPostComments = null;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
