import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InsuredInfoComponent } from './insured-info.component';

describe('InsuredInfoComponent', () => {
  let component: InsuredInfoComponent;
  let fixture: ComponentFixture<InsuredInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InsuredInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InsuredInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
