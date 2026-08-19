import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { appConfig } from './app.config';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    ...appConfig.providers // Ensure providers from appConfig are included
  ]
};
export const config = mergeApplicationConfig({ providers: [...appConfig.providers] }, serverConfig);
