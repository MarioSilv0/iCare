export class NotificationService {

  static requestPermission() {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission().then(permission => {
        console.log(`Notification permission: ${permission}`);
      });
    }
  }

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

export const updatedUserNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Perfil Atualizado',
  body: 'As Informações do seu perfil foram atualizadas com sucesso!'
}

export const addedItemNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Itens Adicionados',
  body: 'Os Itens foram adicionados com sucesso!'
}

export const editedItemNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Itens Editados',
  body: 'Os Itens foram editados com sucesso!'
}

export const removedItemNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Itens Removidos',
  body: 'Os Itens foram removidos com sucesso!'
}
