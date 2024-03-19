import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'b3-input',
  templateUrl: './b3-input.component.html'
})
export class B3InputComponent {
  
  @Input() idDoCampo!: string;
  @Input() placeholder!: string;
  @Input() label!: string;
  @Input() mostrarLabel!: boolean;

  @Output() valorFormatado = new EventEmitter();
  @Output() valor = new EventEmitter<string>();

  transformaValor(event: Event) {
    this.valorFormatado.emit(event);
  }

  registraValor(event: EventTarget | null) {
    const input = event as HTMLInputElement;
    this.valor.emit(input.value);
  }
}
