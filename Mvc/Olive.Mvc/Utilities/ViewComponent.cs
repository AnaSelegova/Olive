﻿namespace Olive.Mvc
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Olive.Entities;

    public abstract class ViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        protected static IDatabase Database => Entities.Data.Database.Instance;

        /// <summary>
        /// Gets HTTP-specific information about an individual HTTP request.
        /// </summary>
        public new HttpContext HttpContext => base.HttpContext ?? Context.Current.Http();

        protected new HttpRequest Request => HttpContext?.Request;

        public ActionResult Redirect(string url) => new RedirectResult(url);

        protected async Task<TViewModel> Bind<TViewModel>(object settings) where TViewModel : IViewModel, new()
        {
            var result = new TViewModel();
            if (settings != null) await ViewModelServices.CopyData(settings, result);
            return result;
        }
    }
}