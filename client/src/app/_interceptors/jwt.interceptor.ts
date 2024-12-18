import { HttpInterceptorFn } from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';

// Definisce un interceptor JWT per le richieste HTTP
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  // Inietta il servizio AccountService
  const accountService = inject(AccountService);

  // Controlla se l'utente corrente è autenticato
  if (accountService.currentUser()) {
    // Clona la richiesta e aggiunge l'intestazione Authorization con il token JWT
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accountService.currentUser()?.token}`
      }
    });
  }

  // Passa la richiesta al prossimo handler nella catena
  return next(req);
};
