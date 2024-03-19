import { Component, Input } from '@angular/core';

@Component({
  selector: 'b3-button',
  templateUrl: './b3-button.component.html'
})
export class B3ButtonComponent {

  @Input() texto!: string;
}
