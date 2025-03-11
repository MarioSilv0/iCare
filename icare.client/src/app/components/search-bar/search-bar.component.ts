import { Component, Input, Output, EventEmitter } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'search-bar',
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent {
  public searchTerm: string = '';
  public searchSubject = new Subject<string>();

  @Input() containerStyles: { [key: string]: string } = {};
  @Input() searchBarStyles: { [key: string]: string } = {};
  @Input() id: string = '';
  @Input() placeholder: string = '';
  @Input() normalize: boolean = false;
  @Output() query = new EventEmitter<string>();

  constructor() {
    this.searchSubject.pipe(debounceTime(300)).subscribe((newSearchTerm) => this.emitChange(newSearchTerm));
  }

  onSearchTermChange(newSearchTerm: string | number) {
    let tmp: string = '';
    if (typeof newSearchTerm === 'number') tmp = newSearchTerm.toString();

    this.searchSubject.next(tmp);
  }

  emitChange(newSearchTerm: string) {
    this.searchTerm = newSearchTerm
    let result = this.searchTerm;
    if (this.normalize) result = this.searchTerm.normalize('NFD')
                                                .replace(/[\u0300-\u036f]/g, '')
                                                .replace(/[^a-zA-Z0-9]/g, '')
                                                .toLowerCase();

    this.query.emit(result);
  }
}
