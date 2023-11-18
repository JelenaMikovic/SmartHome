import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavbarService {

  private sideVisible = new BehaviorSubject<Boolean>(false);

  setSideVisible(value: boolean) {
    this.sideVisible.next(value)
  }

  getSideVisible() {
    return this.sideVisible.asObservable();
  }

}
