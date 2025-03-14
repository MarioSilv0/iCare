/**
 * @file Defines the `NotificationService` class, which handles browser notifications.
 * It provides methods for requesting notification permissions and displaying notifications.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Norification } from '../../models'

/**
 * The `NotificationService` class provides static methods for handling browser notifications.
 * It includes functionality to request notification permissions and display notifications if allowed.
 */
export class NotificationService {

  /**
   * Requests permission from the user to display browser notifications.
   * If the permission is still in the default state, it prompts the user to allow or deny notifications.
   */
  static requestPermission() {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission().then(permission => {
        console.log(`Notification permission: ${permission}`);
      });
    }
  }

  /**
   * Displays a browser notification if the user has granted permission.
   * If permission is still in the default state, it will request permission first.
   * 
   * @param {boolean} permission - Whether the user has enabled notifications.
   * @param {NotificationData} notification - The notification details (icon, title, body).
   */
  static showNotification(permission: Boolean, notification: Notification) {
    const { icon, title, body } = notification;

    if (Notification.permission === 'default') NotificationService.requestPermission();

    if (permission && Notification.permission === 'granted') {
      new Notification(title, { body, icon });
    }
  }
}
