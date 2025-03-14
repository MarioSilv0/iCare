import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { memoize } from '../../utils/Memoization';
import { normalize } from '../../utils/Normalize';

@Component({
  selector: 'search-bar',
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent implements OnInit {
  @Input() containerStyles: { [key: string]: string } = {};
  @Input() searchBarStyles: { [key: string]: string } = {};
  @Input() id: string = '';
  @Input() placeholder: string = '';
  @Input() normalize: boolean = false;

  @Output() query = new EventEmitter<string>();

  searchControl = new FormControl('');
  private normalizeMemoized = memoize(normalize);

  ngOnInit() {
    this.searchControl.valueChanges.pipe(debounceTime(300)).subscribe((newQuery: string | null) => this.emitChange(newQuery || ''));
  }

  emitChange(newSearchTerm: string) {
    if (this.normalize) newSearchTerm = this.normalizeMemoized(newSearchTerm);
    this.query.emit(newSearchTerm);
  }
}
