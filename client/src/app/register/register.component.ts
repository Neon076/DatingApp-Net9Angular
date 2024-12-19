import {
  Component,
  EventEmitter,
  inject,
  input,
  OnInit,
  Output,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { JsonPipe } from '@angular/common';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, JsonPipe, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  isDestroyed = false;
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors : string[] | undefined;

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male', Validators.required],
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(10),
        ],
      ],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(10),
        ],
      ],
      confirmPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          this.matchValues('password'),
        ],
      ],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      },
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }
  register() {
    const dob = this.geDateOnly(this.registerForm.get('dateOfBirth')?.value)
    this.registerForm.patchValue({dateOfBirth : dob});
    console.log(this.registerForm.value);
    
    this.accountService.register(this.registerForm.value).subscribe({
      next : () =>{
        this.router.navigateByUrl('/members')
      },
      error : error => console.log(error)

    })
  }

  cancel() {
    if (!this.isDestroyed) {
      this.cancelRegister.emit(false); // Emit event safely
    }
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
  }

  private geDateOnly(dob : string | undefined){
    if(!dob) return
    return new Date(dob).toISOString().slice(0,10);
  }
}
