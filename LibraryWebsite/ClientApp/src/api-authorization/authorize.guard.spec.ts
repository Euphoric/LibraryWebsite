import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { RouterTestingModule } from "@angular/router/testing";

import { AuthorizeGuard } from './authorize.guard';


describe('AuthorizeGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [AuthorizeGuard, { provide: 'BASE_URL', useValue: 'www.test.com', }]
    });
  });

  it('should ...', inject([AuthorizeGuard], (guard: AuthorizeGuard) => {
    expect(guard).toBeTruthy();
  }));
});
