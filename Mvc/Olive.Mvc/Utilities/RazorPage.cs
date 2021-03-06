﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Olive.Mvc
{
    public abstract class RazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        /// <summary>
        /// Gets the View Model instance to provide a consistent API to gain access to the ViewModel object from controller and View.
        /// </summary>
        protected virtual TModel info => Model;

        public HttpRequest Request => Context.Request;

        /// <summary>
        /// Gets a file from its relative path.
        /// </summary>
        public FileInfo MapPath(string relativePath)
        {
            var fileProvider = Environment.ContentRootFileProvider;
            var path = fileProvider.GetFileInfo(relativePath)?.PhysicalPath;
            if (path.IsEmpty()) return null;
            return path.AsFile();
        }

        public virtual IHostingEnvironment Environment => Olive.Context.Current.GetService<IHostingEnvironment>();
    }
}