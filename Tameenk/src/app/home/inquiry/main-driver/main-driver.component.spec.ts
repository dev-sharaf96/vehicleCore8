import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainDriverComponent } from './main-driver.component';

describe('MainDriverComponent', () => {
  let component: MainDriverComponent;
  let fixture: ComponentFixture<MainDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
