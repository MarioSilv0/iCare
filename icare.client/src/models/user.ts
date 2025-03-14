export { User, Item }

interface User {
  picture: string;
  name: string;
  email: string;
  birthdate: string;
  notifications: boolean;
  height: number;
  weight: number;
  preferences: Set<string>;
  restrictions: Set<string>;
  categories: Set<string>;
}

interface Item {
  name: string;
  quantity: number;
  unit: string;
}
