import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { benefitsComponent } from './benefits.component';

describe('benefitsComponent', () => {
  let component: benefitsComponent;
  let fixture: ComponentFixture<benefitsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ benefitsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(benefitsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
