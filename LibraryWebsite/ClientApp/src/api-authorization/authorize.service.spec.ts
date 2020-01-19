import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { AuthorizeService } from './authorize.service';

describe('AuthorizeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthorizeService]
    });
  });

  it('should be created', inject([AuthorizeService], (service: AuthorizeService) => {
    expect(service).toBeTruthy();
  }));
});
