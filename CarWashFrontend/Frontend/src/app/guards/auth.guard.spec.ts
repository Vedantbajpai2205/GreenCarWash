import { TestBed } from '@angular/core/testing';
import { CanActivateFn, Router } from '@angular/router';

import { AuthGuard } from './auth.guard';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (route, state) => 
      TestBed.runInInjectionContext(() => new AuthGuard(TestBed.inject(Router)).canActivate(route, state));

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }]
    });
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
