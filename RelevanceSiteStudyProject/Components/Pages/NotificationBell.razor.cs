namespace RelevanceSiteStudyProject.Components.Pages
{
    public partial class NotificationBell
    {

        private bool showPanel = false;

        protected override void OnInitialized()
        {
            NotificationService.OnChange += StateHasChanged;
        }

        private void TogglePanel()
        {
            showPanel = !showPanel;

            // if (showPanel)
            //     NotificationService.MarkAllAsRead();
        }

        private void MarkAllAsRead() => NotificationService.MarkAllAsRead();

        public void Dispose() => NotificationService.OnChange -= StateHasChanged;
    }
}
