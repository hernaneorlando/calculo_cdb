import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { B3InputComponent } from './b3-input/b3-input.component';
import { B3ButtonComponent } from './b3-button/b3-button.component';

@NgModule({
  declarations: [
    B3InputComponent,
    B3ButtonComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    B3InputComponent,
    B3ButtonComponent
  ]
})
export class SharedModule { }
