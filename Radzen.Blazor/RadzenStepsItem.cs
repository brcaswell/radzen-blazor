﻿using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenStepsItem.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public class RadzenStepsItem : RadzenComponent
    {
        /// <summary>
        /// The text
        /// </summary>
        private string _text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Parameter]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    if (Steps != null)
                    {
                        Steps.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenStepsItem"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Selected { get; set; }

        /// <summary>
        /// The visible
        /// </summary>
        bool _visible = true;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenComponent" /> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [Parameter]
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    if (Steps != null)
                    {
                        Steps.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// The disabled
        /// </summary>
        bool _disabled;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenStepsItem"/> is disabled.
        /// </summary>
        /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Disabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    if (Steps != null)
                    {
                        Steps.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the content of the child.
        /// </summary>
        /// <value>The content of the child.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The steps
        /// </summary>
        RadzenSteps _steps;

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>The steps.</value>
        [CascadingParameter]
        public RadzenSteps Steps
        {
            get
            {
                return _steps;
            }
            set
            {
                if (_steps != value)
                {
                    _steps = value;
                    _steps.AddStep(this);
                }
            }
        }

        /// <summary>
        /// Set parameters as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.DidParameterChange(nameof(Selected), Selected))
            {
                var selected = parameters.GetValueOrDefault<bool>(nameof(Selected));
                if (!selected)
                {
                    Steps?.SelectFirst();
                }
                else
                {
                    Steps?.SelectStep(this);
                }
            }

            await base.SetParametersAsync(parameters);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Steps?.RemoveStep(this);
        }
    }
}