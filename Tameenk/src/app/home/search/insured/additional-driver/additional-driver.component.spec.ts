import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdditionalDriverComponent } from './additional-driver.component';

describe('AdditionalDriverComponent', () => {
  let component: AdditionalDriverComponent;
  let fixture: ComponentFixture<AdditionalDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdditionalDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdditionalDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
