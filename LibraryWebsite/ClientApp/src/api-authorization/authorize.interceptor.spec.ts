import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { RouterTestingModule } from "@angular/router/testing";

import { AuthorizeInterceptor } from './authorize.interceptor';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [AuthorizeInterceptor, { provide: 'BASE_URL', useValue: 'www.test.com', }],
    });
  });

  it('should be created', inject([AuthorizeInterceptor], (service: AuthorizeInterceptor) => {
    expect(service).toBeTruthy();
  }));
});
