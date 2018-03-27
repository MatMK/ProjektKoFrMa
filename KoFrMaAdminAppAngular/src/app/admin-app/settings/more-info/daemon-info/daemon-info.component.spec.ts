import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DaemonInfoComponent } from './daemon-info.component';

describe('DaemonInfoComponent', () => {
  let component: DaemonInfoComponent;
  let fixture: ComponentFixture<DaemonInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DaemonInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DaemonInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
