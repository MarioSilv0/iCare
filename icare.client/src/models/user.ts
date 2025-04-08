export { User, Item }

interface User {
  picture: string;
  name: string;
  email: string;
  birthdate: string;
  height: number;
  weight: number;
  gender: string;
  activityLevel: string;
  notifications: boolean;
  preferences: Set<string>;
  restrictions: Set<string>;
  categories: Set<string>;
  genders: string[];
  activityLevels: string[];
}

interface Item {
  name: string;
  quantity: number;
  unit: string;
}
