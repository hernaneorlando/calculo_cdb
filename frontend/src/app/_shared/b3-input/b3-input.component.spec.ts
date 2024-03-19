import { ComponentFixture, TestBed } from '@angular/core/testing';

import { B3InputComponent } from './b3-input.component';

describe('B3ButtonComponent', () => {
  let component: B3InputComponent;
  let fixture: ComponentFixture<B3InputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [B3InputComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(B3InputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
