import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MissingFieldComponent } from './missing-field.component';

describe('MissingFieldComponent', () => {
  let component: MissingFieldComponent;
  let fixture: ComponentFixture<MissingFieldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MissingFieldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MissingFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
