import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompletedTaskInfoComponent } from './completed-task-info.component';

describe('CompletedTaskInfoComponent', () => {
  let component: CompletedTaskInfoComponent;
  let fixture: ComponentFixture<CompletedTaskInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompletedTaskInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompletedTaskInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
