import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MissingFieldsComponent } from './missing-fields.component';

describe('MissingFieldsComponent', () => {
  let component: MissingFieldsComponent;
  let fixture: ComponentFixture<MissingFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MissingFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MissingFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
