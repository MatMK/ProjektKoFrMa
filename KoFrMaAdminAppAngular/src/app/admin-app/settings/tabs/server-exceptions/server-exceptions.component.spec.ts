import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServerExceptionsComponent } from './server-exceptions.component';

describe('ServerExceptionsComponent', () => {
  let component: ServerExceptionsComponent;
  let fixture: ComponentFixture<ServerExceptionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServerExceptionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServerExceptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
