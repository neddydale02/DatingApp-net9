import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideToastr } from 'ngx-toastr';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';

// Configurazione dell'applicazione
export const appConfig: ApplicationConfig = {
  providers: [
    // Fornisce il router con le rotte definite
    provideRouter(routes),
    // Fornisce il client HTTP con gli interceptor per la gestione degli errori e dei token JWT
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor])),
    // Abilita le animazioni
    provideAnimations(),
    // Configura Toastr per le notifiche, posizionandole in basso a destra
    provideToastr({
      positionClass: 'toast-bottom-right'
    })
  ]
};
