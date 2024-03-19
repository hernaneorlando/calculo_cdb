import { ComponentFixture, TestBed } from '@angular/core/testing';

import { B3ButtonComponent } from './b3-button.component';

describe('B3ButtonComponent', () => {
  let component: B3ButtonComponent;
  let fixture: ComponentFixture<B3ButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [B3ButtonComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(B3ButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
