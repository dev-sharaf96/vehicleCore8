import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdditonalDriverComponent } from './additonal-driver.component';

describe('AdditonalDriverComponent', () => {
  let component: AdditonalDriverComponent;
  let fixture: ComponentFixture<AdditonalDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdditonalDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdditonalDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
