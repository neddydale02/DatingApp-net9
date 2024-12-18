import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/members';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient); // Inject del servizio HttpClient
  private accountService = inject(AccountService); // Inject del servizio AccountService
  baseUrl = environment.apiUrl; // URL base dell'API

  // Metodo per ottenere tutti i membri (utenti)
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  // Metodo per ottenere un membro (utente) specifico per nome utente
  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

}
