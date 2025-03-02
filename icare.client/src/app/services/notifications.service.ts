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

interface Notification {
  icon: string;
  title: string;
  body: string;
}

/**
 * Notification displayed when a user's profile is successfully updated.
 */
export const updatedUserNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Perfil Atualizado',
  body: 'As Informações do seu perfil foram atualizadas com sucesso!'
}

/**
 * Notification displayed when an email update fails due to an invalid or duplicate email.
 */
export const failedToEditEmailUserNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Email Inválido ou Duplicado',
  body: 'Não foi possível atualizar o seu email! Isto pode ter ocorrido devido ao email ser inválido ou duplicado.'
}

/**
 * Notification displayed when items are successfully added to the inventory.
 */
export const addedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Adicionados',
  body: 'Os Itens foram adicionados com sucesso!'
}

/**
 * Notification displayed when inventory items are successfully edited.
 */
export const editedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Editados',
  body: 'Os Itens foram editados com sucesso!'
}

/**
 * Notification displayed when inventory items are successfully removed.
 */
export const removedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Removidos',
  body: 'Os Itens foram removidos com sucesso!'
}
