import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchResultOptionsComponent } from './search-result-options.component';

describe('SearchResultOptionsComponent', () => {
  let component: SearchResultOptionsComponent;
  let fixture: ComponentFixture<SearchResultOptionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchResultOptionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchResultOptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
