import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { RouterTestingModule } from '@angular/router/testing';

import { LoginMenuComponent } from './login-menu.component';

describe('LoginMenuComponent', () => {
  let component: LoginMenuComponent;
  let fixture: ComponentFixture<LoginMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: 'BASE_URL', useValue: 'www.test.com', },],
      declarations: [ LoginMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
