import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { YakeenMissingFieldsComponent } from './yakeen-missing-fields.component';

describe('YakeenMissingFieldsComponent', () => {
  let component: YakeenMissingFieldsComponent;
  let fixture: ComponentFixture<YakeenMissingFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [YakeenMissingFieldsComponent]
     // , providers: [InquiryService]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(YakeenMissingFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
