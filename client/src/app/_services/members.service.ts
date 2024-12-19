import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/members';
import { AccountService } from './account.service';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient); // Inject del servizio HttpClient
  private accountService = inject(AccountService); // Inject del servizio AccountService
  baseUrl = environment.apiUrl; // URL base dell'API
  members = signal<Member[]>([]); // Segnale per la lista dei membri (utenti)

  // Metodo per ottenere tutti i membri (utenti)
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').subscribe({
      next: members => this.members.set(members) // Imposta la lista dei membri ricevuti
    });
  }

  // Metodo per ottenere un membro (utente) specifico per nome utente
  getMember(username: string) {
    const member = this.members().find(x => x.userName === username); // Cerca il membro nella lista locale
    if (member !== undefined) return of(member); // Se trovato, restituisce il membro come Observable
    return this.http.get<Member>(this.baseUrl + 'users/' + username); // Altrimenti, effettua una richiesta HTTP
  }

  // Metodo per aggiornare le informazioni di un membro esistente
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      tap(() => {
        this.members.update(members => 
          members.map(m => 
            m.userName === member.userName ? member : m // Aggiorna il membro nella lista locale
          )
        )
      })
    );
  }
}
