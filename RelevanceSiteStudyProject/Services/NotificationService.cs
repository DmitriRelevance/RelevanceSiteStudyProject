using System.Collections.ObjectModel;
using static RelevanceSiteStudyProject.Components.Pages.Notification;

namespace RelevanceSiteStudyProject.Services
{

    public class NotificationService
    {
        public event Action? OnChange;

        private readonly ObservableCollection<NotificationInfo> _notifications = new();

        public IReadOnlyCollection<NotificationInfo> Notifications => _notifications;

        public void AddNotification(string message, NotificationInfoType type)
        {
            _notifications.Insert(0, new NotificationInfo
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now,
                IsRead = false
            });

            NotifyStateChanged();
        }

        public void MarkAllAsRead()
        {
            foreach (var note in _notifications)
                note.IsRead = true;

            NotifyStateChanged();
        }

        public int UnreadCount => _notifications.Count(n => !n.IsRead);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}
