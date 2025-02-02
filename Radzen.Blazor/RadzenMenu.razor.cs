using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenMenu.
    /// Implements the <see cref="Radzen.RadzenComponentWithChildren" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponentWithChildren" />
    public partial class RadzenMenu : RadzenComponentWithChildren
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenMenu"/> is responsive.
        /// </summary>
        /// <value><c>true</c> if responsive; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Responsive { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        private bool IsOpen { get; set; } = false;

        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            var classList = new List<string>();

            classList.Add("rz-menu");

            if (Responsive)
            {
                if (IsOpen)
                {
                    classList.Add("rz-menu-open");
                }
                else
                {
                    classList.Add("rz-menu-closed");
                }
            }

            return string.Join(" ", classList);
        }

        /// <summary>
        /// Called when [toggle].
        /// </summary>
        void OnToggle()
        {
            IsOpen = !IsOpen;
        }

        /// <summary>
        /// Gets or sets the click.
        /// </summary>
        /// <value>The click.</value>
        [Parameter]
        public EventCallback<MenuItemEventArgs> Click { get; set; }
    }
}