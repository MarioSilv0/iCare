export {
  Notification,
  updatedUserNotification,
  failedToEditEmailUserNotification,
  addedItemNotification,
  editedItemNotification,
  removedItemNotification
}

interface Notification {
  icon: string;
  title: string;
  body: string;
}

/**
 * Notification displayed when a user's profile is successfully updated.
 */
const updatedUserNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Perfil Atualizado',
  body: 'As Informações do seu perfil foram atualizadas com sucesso!'
}

/**
 * Notification displayed when an email update fails due to an invalid or duplicate email.
 */
const failedToEditEmailUserNotification: Notification = {
  icon: '/assets/svgs/user.svg',
  title: 'Email Inválido ou Duplicado',
  body: 'Não foi possível atualizar o seu email! Isto pode ter ocorrido devido ao email ser inválido ou duplicado.'
}

/**
 * Notification displayed when items are successfully added to the inventory.
 */
const addedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Adicionados',
  body: 'Os Itens foram adicionados com sucesso!'
}

/**
 * Notification displayed when inventory items are successfully edited.
 */
const editedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Editados',
  body: 'Os Itens foram editados com sucesso!'
}

/**
 * Notification displayed when inventory items are successfully removed.
 */
const removedItemNotification: Notification = {
  icon: '/assets/svgs/inventory.svg',
  title: 'Itens Removidos',
  body: 'Os Itens foram removidos com sucesso!'
}
