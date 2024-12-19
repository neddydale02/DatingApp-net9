import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

/*Servizio che gestisce lo stato di "busy" (occupato) dell'applicazione.
  Utilizza il servizio NgxSpinnerService per mostrare e nascondere uno spinner di caricamento.*/
@Injectable({
  providedIn: 'root'
})
export class BusyService {

  //Contatore delle richieste in corso.
  busyRequestCount = 0;
  private spinnerService = inject(NgxSpinnerService)

  //Incrementa il contatore delle richieste in corso e mostra lo spinner di caricamento.
  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined,{
      type:'pacman',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    });
  }

  /*Decrementa il contatore delle richieste in corso e nasconde lo spinner di caricamento
    se non ci sono più richieste in corso.*/
  idle(){
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0){
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
