import { Component, inject, Output, EventEmitter, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ReactiveFormsModule } from '@angular/forms';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService); // Iniezione del servizio AccountService
  private fb = inject(FormBuilder); // Iniezione del servizio FormBuilder
  private router = inject(Router); // Iniezione del servizio Router
  @Output() cancelRegister = new EventEmitter<boolean>(); // Output per l'evento di cancellazione della registrazione
  model: any = {}; // Modello per i dati del form
  registerForm: FormGroup = new FormGroup({}); // FormGroup per il form di registrazione
  maxDate = new Date(); // Data massima per il campo data di nascita
  validationErrors: string[] | undefined; // Array per gli errori di validazione

  ngOnInit() {
    this.initializeForm(); // Inizializzazione del form all'avvio del componente
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18); // Impostazione della data massima a 18 anni fa
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['other'],
      username: ['', Validators.required], // Campo username con validazione richiesta
      knownAs: ['', Validators.required], // Campo knownAs con validazione richiesta
      dateOfBirth: ['', Validators.required], // Campo dateOfBirth con validazione richiesta
      city: ['', Validators.required], // Campo city con validazione richiesta
      country: ['', Validators.required], // Campo country con validazione richiesta
      password: ['', [
        Validators.required, 
        Validators.minLength(4), 
        Validators.maxLength(8),
        this.matchValues('confirmPassword') // Validatore personalizzato per il campo password
      ]],
      confirmPassword: ['', [
        Validators.required, 
        Validators.minLength(4), 
        Validators.maxLength(8),
        this.matchValues('password') // Validatore personalizzato per il campo conferma password
      ]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity(); // Aggiornamento della validità del campo conferma password quando cambia il valore della password
    })
  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true} // Validatore per controllare se i valori dei campi corrispondono
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: false} // Validatore per controllare se i valori dei campi corrispondono
  }
}

  register() {
    const dob = this.getDateOnly(this.registerForm.get("dateOfBirth")?.value);
    this.registerForm.patchValue({dateOfBirth: dob}); // Impostazione della data di nascita nel formato corretto
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/members'), // Reindirizzamento alla pagina dei membri in caso di successo
      error: error => this.validationErrors = error // Impostazione degli errori di validazione in caso di errore
    })
  }

  cancel() {
    this.cancelRegister.emit(false); // Emissione dell'evento di cancellazione della registrazione
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    const date = new Date(dob);
    return date.toISOString().slice(0, 10);
  }
}
