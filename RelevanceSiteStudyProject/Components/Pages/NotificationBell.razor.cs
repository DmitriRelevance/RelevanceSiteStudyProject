using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RelevanceSiteStudyProject.Components.Pages
{
    public partial class NotificationBell
    {

        private bool showPanel = false;

        private bool pendingElementReferenteRegistration = false;
        private ElementReference panelRef;
        private DotNetObjectReference<NotificationBell>? objRef;
        protected override void OnInitialized()
        {
            NotificationService.OnChange += StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(pendingElementReferenteRegistration && showPanel)
            {
                pendingElementReferenteRegistration = false;
                objRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("notificationUtils.registerOutsideClickById", "notificationPanel", objRef);
            }
        }

        private async Task TogglePanel()
        {
            // Toggle the visibility of the notification panel
            showPanel = !showPanel;

            // if (showPanel)
            //     NotificationService.MarkAllAsRead();

            if (showPanel)
            {
                pendingElementReferenteRegistration = true;
            }
        }

        [JSInvokable]
        public void HidePanel()
        {
            showPanel = false;
            StateHasChanged();
        }

        private void MarkAllAsRead() => NotificationService.MarkAllAsRead();

        public void Dispose()
        {
            NotificationService.OnChange -= StateHasChanged;
            objRef?.Dispose();
        }
    }
}
