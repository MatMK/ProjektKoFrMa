import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExceptionInfoComponent } from './exception-info.component';

describe('ExceptionInfoComponent', () => {
  let component: ExceptionInfoComponent;
  let fixture: ComponentFixture<ExceptionInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExceptionInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExceptionInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
