﻿using Radzen;
using Radzen.Blazor.Rendering;
using System.Collections;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenAutoComplete.
    /// Implements the <see cref="Radzen.DataBoundFormComponent{System.String}" />
    /// </summary>
    /// <seealso cref="Radzen.DataBoundFormComponent{System.String}" />
    public partial class RadzenAutoComplete : DataBoundFormComponent<string>
    {
        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>The minimum length.</value>
        [Parameter]
        public int MinLength { get; set; } = 1;

        /// <summary>
        /// Gets or sets the filter delay.
        /// </summary>
        /// <value>The filter delay.</value>
        [Parameter]
        public int FilterDelay { get; set; } = 500;

        /// <summary>
        /// The search
        /// </summary>
        protected ElementReference search;
        /// <summary>
        /// The list
        /// </summary>
        protected ElementReference list;
        /// <summary>
        /// The custom search text
        /// </summary>
        string customSearchText;

        /// <summary>
        /// The selected index
        /// </summary>
        int selectedIndex = -1;

        /// <summary>
        /// Handles the <see cref="E:FilterKeyPress" /> event.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        protected async Task OnFilterKeyPress(KeyboardEventArgs args)
        {
            var items = (LoadData.HasDelegate ? Data != null ? Data : Enumerable.Empty<object>() : (View != null ? View : Enumerable.Empty<object>())).OfType<object>();

            var key = args.Code != null ? args.Code : args.Key;

            if (key == "ArrowDown" || key == "ArrowUp")
            {
                try
                {
                    selectedIndex = await JSRuntime.InvokeAsync<int>("Radzen.focusListItem", search, list, key == "ArrowDown", selectedIndex);
                }
                catch (Exception)
                {
                    //
                }
            }
            else if (key == "Enter")
            {
                if (selectedIndex >= 0 && selectedIndex <= items.Count() - 1)
                {
                    await OnSelectItem(items.ElementAt(selectedIndex));
                    selectedIndex = -1;
                }
            }
            else if (key == "Escape")
            {
                await JSRuntime.InvokeVoidAsync("Radzen.closePopup", PopupID);
            }
            else if(key != "Tab")
            {
                selectedIndex = -1;

                Debounce(DebounceFilter, FilterDelay);
            }
        }

        /// <summary>
        /// Debounces the filter.
        /// </summary>
        async Task DebounceFilter()
        {
            var value = await JSRuntime.InvokeAsync<string>("Radzen.getInputValue", search);

            if (value.Length < MinLength)
                return;

            if (!LoadData.HasDelegate)
            {
                searchText = value;
                await InvokeAsync(() => { StateHasChanged(); });
            }
            else
            {
                customSearchText = value;
                await InvokeAsync(() => { LoadData.InvokeAsync(new Radzen.LoadDataArgs() { Filter = customSearchText }); });
            }
        }

        /// <summary>
        /// Gets the popup identifier.
        /// </summary>
        /// <value>The popup identifier.</value>
        private string PopupID
        {
            get
            {
                return $"popup{UniqueID}";
            }
        }

        /// <summary>
        /// Called when [select item].
        /// </summary>
        /// <param name="item">The item.</param>
        private async Task OnSelectItem(object item)
        {
            await JSRuntime.InvokeVoidAsync("Radzen.closePopup", PopupID);

            await SelectItem(item);
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>The query.</value>
        protected override IQueryable Query
        {
            get
            {
                return Data != null && !string.IsNullOrEmpty(searchText) ? Data.AsQueryable() : null;
            }
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        protected override IEnumerable View
        {
            get
            {
                if (Query != null)
                {
                    string filterCaseSensitivityOperator = FilterCaseSensitivity == FilterCaseSensitivity.CaseInsensitive ? ".ToLower()" : "";

                    return Query.Where($"{TextProperty}{filterCaseSensitivityOperator}.{Enum.GetName(typeof(StringFilterOperator), FilterOperator)}(@0)",
                        FilterCaseSensitivity == FilterCaseSensitivity.CaseInsensitive ? searchText.ToLower() : searchText);
                }

                return null;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:Change" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChangeEventArgs"/> instance containing the event data.</param>
        protected async System.Threading.Tasks.Task OnChange(ChangeEventArgs args)
        {
            Value = args.Value;

            await ValueChanged.InvokeAsync($"{Value}");
            if (FieldIdentifier.FieldName != null) { EditContext?.NotifyFieldChanged(FieldIdentifier); }
            await Change.InvokeAsync(Value);
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item.</param>
        async System.Threading.Tasks.Task SelectItem(object item)
        {
            if (!string.IsNullOrEmpty(TextProperty))
            {
                Value = PropertyAccess.GetItemOrValueFromProperty(item, TextProperty);
            }
            else
            {
                Value = item;
            }

            await ValueChanged.InvokeAsync($"{Value}");
            if (FieldIdentifier.FieldName != null) { EditContext?.NotifyFieldChanged(FieldIdentifier); }
            await Change.InvokeAsync(Value);

            StateHasChanged();
        }

        /// <summary>
        /// Gets the input class list.
        /// </summary>
        /// <value>The input class list.</value>
        ClassList InputClassList => ClassList.Create("rz-inputtext rz-autocomplete-input")
                                             .AddDisabled(Disabled);

        /// <summary>
        /// Opens the script.
        /// </summary>
        /// <returns>System.String.</returns>
        private string OpenScript()
        {
            if (Disabled)
            {
                return string.Empty;
            }

            return $"Radzen.openPopup(this.parentNode, '{PopupID}', true)";
        }

        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return GetClassList("").ToString();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (IsJSRuntimeAvailable)
            {
                JSRuntime.InvokeVoidAsync("Radzen.destroyPopup", PopupID);
            }
        }

        /// <summary>
        /// The first render
        /// </summary>
        private bool firstRender = true;

        /// <summary>
        /// Called when [after render asynchronous].
        /// </summary>
        /// <param name="firstRender">if set to <c>true</c> [first render].</param>
        /// <returns>Task.</returns>
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            this.firstRender = firstRender;

            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Set parameters as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var shouldClose = false;

            if (parameters.DidParameterChange(nameof(Visible), Visible))
            {
                var visible = parameters.GetValueOrDefault<bool>(nameof(Visible));
                shouldClose = !visible;
            }

            await base.SetParametersAsync(parameters);

            if (shouldClose && !firstRender)
            {
                await JSRuntime.InvokeVoidAsync("Radzen.destroyPopup", PopupID);
            }
        }
    }
}
