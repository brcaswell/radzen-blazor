﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenSSRSViewer.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public partial class RadzenSSRSViewer : RadzenComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether [use proxy].
        /// </summary>
        /// <value><c>true</c> if [use proxy]; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool UseProxy { get; set; } = false;

        /// <summary>
        /// Gets or sets the report server.
        /// </summary>
        /// <value>The report server.</value>
        [Parameter]
        public string ReportServer { get; set; }

        /// <summary>
        /// Gets or sets the local server.
        /// </summary>
        /// <value>The local server.</value>
        [Parameter]
        public string LocalServer { get; set; }

        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        /// <value>The name of the report.</value>
        [Parameter]
        public string ReportName { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [Parameter]
        public RenderFragment Parameters { get; set; }

        /// <summary>
        /// Gets the report URL.
        /// </summary>
        /// <value>The report URL.</value>
        public string ReportUrl
        {
            get
            {
                var urlParams = string.Join("&", parameters.Where(p => !string.IsNullOrEmpty(p.ParameterName)).Select(p => $"{p.ParameterName}={p.Value}"));
                var urlParamString = parameters.Count > 0 ? $"&{urlParams}" : urlParams;

                var url = $"{ReportServer}/Pages/ReportViewer.aspx?%2f{ReportName}&rs:Command=Render&rs:Embed=true{urlParamString}";

                if (UseProxy)
                {
                    if (!string.IsNullOrEmpty(LocalServer))
                    {
                        url = $"{LocalServer}/__ssrsreport?url={System.Net.WebUtility.UrlEncode(url)}";
                    }
                    else
                    {
                        url = $"{uriHelper.BaseUri}__ssrsreport?url={System.Net.WebUtility.UrlEncode(url)}";
                    }
                }

                return url;
            }
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// The parameters
        /// </summary>
        List<RadzenSSRSViewerParameter> parameters = new List<RadzenSSRSViewerParameter>();

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void AddParameter(RadzenSSRSViewerParameter parameter)
        {
            if (parameters.IndexOf(parameter) == -1)
            {
                parameters.Add(parameter);
                StateHasChanged();
            }
        }

        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return "ssrsviewer";
        }
    }
}