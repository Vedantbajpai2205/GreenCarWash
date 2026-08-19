import { enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { AuthInterceptor } from './app/interceptors/auth.interceptor';


bootstrapApplication(AppComponent, {
  providers: [...appConfig
    .providers,
  ],
}).catch(err => console.error(err));
