export class NotificationService {

  static requestPermission() {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission().then(permission => {
        console.log(`Notification permission: ${permission}`);
      });
    }
  }

  static showNotification(permission: Boolean, icon: string, title: string, body: string) {
    if (Notification.permission === 'default') NotificationService.requestPermission();

    if (permission && Notification.permission === 'granted') {
      new Notification(title, { body, icon });
    }
  }
}
