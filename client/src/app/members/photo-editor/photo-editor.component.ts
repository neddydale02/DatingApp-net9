import { Component, inject, input, OnInit, output } from '@angular/core'; // Importa i moduli e i decoratori necessari da Angular
import { Member } from '../../_models/member'; // Importa il modello Member
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common'; // Importa i moduli comuni di Angular
import { FileUploader, FileUploadModule } from 'ng2-file-upload'; // Importa il modulo e la classe per il caricamento dei file
import { AccountService } from '../../_services/account.service'; // Importa il servizio AccountService
import { environment } from '../../../environments/environment'; // Importa l'ambiente di configurazione
import { MembersService } from '../../_services/members.service'; // Importa il servizio MembersService
import { Photo } from '../../_models/photo'; // Importa il modello Photo

@Component({
  selector: 'app-photo-editor', // Definisce il selettore del componente
  imports: [NgIf, NgFor, NgStyle, NgClass, FileUploadModule, DecimalPipe], // Importa i moduli necessari per il componente
  templateUrl: './photo-editor.component.html', // Specifica il percorso del template HTML
  styleUrl: './photo-editor.component.css' // Specifica il percorso del file CSS
})
export class PhotoEditorComponent implements OnInit { // Definisce la classe del componente PhotoEditorComponent
  private accountService = inject(AccountService); // Inietta il servizio AccountService
  private memberService = inject(MembersService); // Inietta il servizio MembersService
  member = input.required<Member>(); // Definisce un input obbligatorio di tipo Member
  uploader?: FileUploader; // Definisce una variabile opzionale per il caricatore di file
  hasBaseDropZoneOver = false; // Definisce una variabile per gestire lo stato della zona di drop
  baseUrl = environment.apiUrl; // Imposta l'URL di base dall'ambiente di configurazione
  memberChange = output<Member>(); // Definisce un output di tipo Member

  ngOnInit(): void { // Metodo che viene eseguito all'inizializzazione del componente
    this.initializeUploader(); // Inizializza il caricatore di file
  }

  fileOverBase(e: any) { // Metodo per gestire l'evento di drag and drop
    this.hasBaseDropZoneOver = e; // Imposta lo stato della zona di drop
  }

  setMainPhoto(photo: Photo){ // Metodo per impostare la foto principale
    this.memberService.setMainPhoto(photo).subscribe({ // Chiama il servizio per impostare la foto principale
      next: _ => { // Gestisce la risposta positiva
        const user = this.accountService.currentUser(); // Ottiene l'utente corrente
        if(user){ // Se l'utente esiste
          user.photoUrl = photo.url; // Aggiorna l'URL della foto dell'utente
          this.accountService.setCurrentUser(user); // Imposta l'utente corrente aggiornato
        }
        const updatedMember = {...this.member()}; // Crea una copia dell'oggetto member
        updatedMember.photoUrl = photo.url; // Aggiorna l'URL della foto del membro
        updatedMember.photos.forEach(p => { // Aggiorna lo stato delle foto del membro
          if(p.isMain) p.isMain = false; // Imposta isMain a false per la foto precedente
          if(p.id === photo.id) p.isMain = true; // Imposta isMain a true per la nuova foto principale
        });
        this.memberChange.emit(updatedMember); // Emette l'evento memberChange con il membro aggiornato
    }})
  }

  deletePhoto(photo: Photo) { // Metodo per eliminare una foto di un membro
    this.memberService.deletePhoto(photo).subscribe({ // Chiama il servizio per eliminare la foto
      next: _ => { // Gestisce la risposta positiva
        const updatedMember = {...this.member()}; // Crea una copia dell'oggetto member
        updatedMember.photos = updatedMember.photos.filter(p => p !== photo); // Filtra le foto per rimuovere quella eliminata
        this.memberChange.emit(updatedMember); // Emette l'evento memberChange con il membro aggiornato
      }
    });
  }

  initializeUploader(){ // Metodo per inizializzare il caricatore di file
    this.uploader = new FileUploader({ // Crea una nuova istanza di FileUploader
      url: this.baseUrl + 'users/add-photo', // Imposta l'URL per il caricamento dei file
      authToken: 'Bearer ' + (this.accountService.currentUser()?.token), // Imposta il token di autenticazione
      isHTML5: true, // Abilita il supporto HTML5
      allowedFileType: ['image'], // Permette solo file di tipo immagine
      removeAfterUpload: true, // Rimuove il file dopo il caricamento
      autoUpload: false, // Disabilita il caricamento automatico
      maxFileSize: 10 * 1024 * 1024, // Imposta la dimensione massima del file a 10 MB
    });

    this.uploader.onAfterAddingFile = (file) => { // Metodo chiamato dopo l'aggiunta di un file
      file.withCredentials = false; // Disabilita l'invio delle credenziali con il file
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => { // Metodo chiamato dopo il caricamento con successo di un file
      if(response){ // Se c'è una risposta
        const photo = JSON.parse(response); // Parsea la risposta JSON in un oggetto photo
        const updatedMember = {...this.member()}; // Crea una copia dell'oggetto member
        updatedMember.photos.push(photo); // Aggiunge la nuova foto all'array di foto del membro
        this.memberChange.emit(updatedMember); // Emette l'evento memberChange con il membro aggiornato
        if(photo.isMain){
          const user = this.accountService.currentUser(); // Ottiene l'utente corrente
          if(user){ // Se l'utente esiste
            user.photoUrl = photo.url; // Aggiorna l'URL della foto dell'utente
            this.accountService.setCurrentUser(user); // Imposta l'utente corrente aggiornato
          }
          updatedMember.photoUrl = photo.url; // Aggiorna l'URL della foto del membro
          updatedMember.photos.forEach(p => { // Aggiorna lo stato delle foto del membro
            if(p.isMain) p.isMain = false; // Imposta isMain a false per la foto precedente
            if(p.id === photo.id) p.isMain = true; // Imposta isMain a true per la nuova foto principale
          });
          this.memberChange.emit(updatedMember); // Emette l'evento memberChange con il membro aggiornato
        }
      }
    };
  }
}