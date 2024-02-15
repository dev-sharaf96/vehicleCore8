import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdditonalInfoComponent } from './additonal-info.component';

describe('AdditonalInfoComponent', () => {
  let component: AdditonalInfoComponent;
  let fixture: ComponentFixture<AdditonalInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdditonalInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdditonalInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
