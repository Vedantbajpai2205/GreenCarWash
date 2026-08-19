import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ApplicationModule, mergeApplicationConfig } from '@angular/core';
import { FormsModule } from '@angular/forms';

export const appConfig =  {providers:[
  provideHttpClient(withInterceptorsFromDi()),
  provideRouter(routes),
  provideHttpClient()
],
imports: [
  FormsModule
]
};
