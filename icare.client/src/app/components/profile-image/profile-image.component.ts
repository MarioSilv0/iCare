import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'profile-image',
  templateUrl: './profile-image.component.html',
  styleUrl: './profile-image.component.css'
})
export class ProfileImageComponent {
  @Input() containerStyles: { [key: string]: string } = {};
  @Input() imageStyles: { [key: string]: string } = {};
  @Input() imageUrl: string = '';
  @Input() editable: boolean = false;
  @Output() imageChange = new EventEmitter<File>();

  defaultImage: string = 'https://www.w3schools.com/howto/img_avatar.png';

  /**
   * Handles profile picture selection and updates the `user.picture` property.
   * Ensures only image files are accepted.
   * @param {Event} event - The file input change event.
   */
  onSelectFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || !input.files[0]) return;

    const file = input.files[0];
    if (!file.type.startsWith('image/')) return;

    this.imageChange.emit(file);
  }
}
