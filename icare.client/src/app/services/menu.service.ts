import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MenuService {
  private menuState = new BehaviorSubject<boolean>(false);
  showNavMenu$ = this.menuState.asObservable();
  constructor() { }

  toggle() {
    this.menuState.next(!this.menuState.value);
  }

  setMenuState(state: boolean) {
    this.menuState.next(state)
  }
}
